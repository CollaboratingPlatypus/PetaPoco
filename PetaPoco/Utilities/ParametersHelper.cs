// <copyright file="ParametersHelper.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PetaPoco.Internal
{
    internal static class ParametersHelper
    {
        private static Regex ParamPrefixRegex = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
        private static Regex NonWordStartRegex = new Regex(@"^\W*", RegexOptions.Compiled);

        public static string ReplaceParamPrefix(this string sql, string paramPrefix)
        {
            return ParamPrefixRegex.Replace(sql, m => paramPrefix + m.Value.Substring(1));
        }

        public static string EnsureParamPrefix(this int input, string paramPrefix) => $"{paramPrefix}{input}";

        public static string EnsureParamPrefix(this string input, string paramPrefix)
        {
            if (input.StartsWith(paramPrefix))
                return input;
            else
                return NonWordStartRegex.Replace(input, paramPrefix);
        }

        // Helper to handle named parameters from object properties
        public static string ProcessQueryParams(string sql, object[] args_src, List<object> args_dest)
        {   
            return ParamPrefixRegex.Replace(sql, m =>
            {
                string param = m.Value.Substring(1);

                object arg_val;

                int paramIndex;
                if (int.TryParse(param, out paramIndex))
                {
                    // Numbered parameter
                    if (paramIndex < 0 || paramIndex >= args_src.Length)
                        throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", paramIndex,
                            args_src.Length, sql));
                    arg_val = args_src[paramIndex];
                }
                else
                {
                    // Look for a property on one of the arguments with this name
                    bool found = false;
                    arg_val = null;
                    foreach (var o in args_src)
                    {
                        var pi = o.GetType().GetProperty(param);
                        if (pi != null)
                        {
                            arg_val = pi.GetValue(o, null);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        throw new ArgumentException(
                            string.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", param, sql));
                }

                // Expand collections to parameter lists
                if ((arg_val as System.Collections.IEnumerable) != null &&
                    (arg_val as string) == null &&
                    (arg_val as byte[]) == null)
                {
                    var sb = new StringBuilder();
                    foreach (var i in arg_val as System.Collections.IEnumerable)
                    {
                        sb.Append((sb.Length == 0 ? "@" : ",@") + args_dest.Count.ToString());
                        args_dest.Add(i);
                    }
                    return sb.ToString();
                }
                else
                {
                    args_dest.Add(arg_val);
                    return "@" + (args_dest.Count - 1).ToString();
                }
            }
                );
        }

        public static object[] ProcessStoredProcParams(IDbCommand cmd, object[] args, Action<IDbDataParameter, object, PropertyInfo> setParameterProperties)
        {
            // For a stored proc, we assume that we're only getting POCOs or parameters
            var result = new List<IDbDataParameter>();

            foreach (var arg in args)
            {
                if (arg is IDbDataParameter)
                    result.Add((IDbDataParameter)arg);
                else if (arg is IEnumerable<IDbDataParameter> paramList)
                    result.AddRange(paramList);
                else
                {
                    var type = arg.GetType();
                    if (type.IsValueType || type == typeof(string))
                        throw new ArgumentException($"Value type or string passed as stored procedure argument: {arg}");
                    var readableProps = type.GetProperties().Where(p => p.CanRead);
                    foreach (var prop in readableProps)
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = prop.Name;
                        setParameterProperties(param, prop.GetValue(arg, null), null);
                        result.Add(param);
                    }
                }
            }

            return result.ToArray();
        }
    }
}