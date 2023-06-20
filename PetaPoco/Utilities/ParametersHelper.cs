using PetaPoco.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PetaPoco.Internal
{
    /// <summary>
    /// Provides static utility methods and extensions for handling SQL parameters.
    /// </summary>
    /// <remarks>
    /// This class includes extensions for validating and replacing parameter prefixes, as well as static methods used for processing parameters for queries and stored procedures.
    /// </remarks>
    internal static class ParametersHelper
    {
        private static Regex ParamPrefixRegex = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
        private static Regex NonWordStartRegex = new Regex(@"^\W*", RegexOptions.Compiled);

        /// <summary>
        /// Replaces the parameter prefix in the given SQL query with the specified prefix.
        /// </summary>
        /// <param name="sql">The SQL query in which to replace the parameter prefix.</param>
        /// <param name="paramPrefix">The prefix to replace the parameter prefix with.</param>
        /// <returns>The SQL query with the parameter prefix replaced.</returns>
        public static string ReplaceParamPrefix(this string sql, string paramPrefix)
            => ParamPrefixRegex.Replace(sql, m => paramPrefix + m.Value.Substring(1));

        /// <summary>
        /// Ensures that the given SQL parameter number has the correct prefix.
        /// </summary>
        /// <param name="input">The parameter number.</param>
        /// <param name="paramPrefix">The prefix string.</param>
        /// <returns>The input as a string, prefixed with the given prefix.</returns>
        public static string EnsureParamPrefix(this int input, string paramPrefix)
            => $"{paramPrefix}{input}";

        /// <summary>
        /// Ensures that the given SQL parameter string has the correct prefix.
        /// </summary>
        /// <param name="input">The parameter name.</param>
        /// <param name="paramPrefix">The prefix string.</param>
        /// <returns>The input string prefixed with the given prefix.</returns>
        public static string EnsureParamPrefix(this string input, string paramPrefix)
            => input.StartsWith(paramPrefix) ? input : NonWordStartRegex.Replace(input, paramPrefix);

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
                        throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", paramIndex, args_src.Length,
                            sql));
                    arg_val = args_src[paramIndex];
                }
                else
                {
                    bool found = false;
                    arg_val = null;

                    foreach (var o in args_src)
                    {
                        if (o is IDictionary dict)
                        {
                            Type[] arguments = dict.GetType().GetGenericArguments();

                            if (arguments[0] == typeof(string) && dict.Contains(param))
                            {
                                arg_val = dict[param];
                                found = true;
                                break;                                
                            }
                        }

                        var pi = o.GetType().GetProperty(param);
                        if (pi != null)
                        {
                            arg_val = pi.GetValue(o, null);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        throw new ArgumentException(string.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", param,
                            sql));
                }

                // Expand collections to parameter lists
                if (arg_val.IsEnumerable())
                {
                    var sb = new StringBuilder();
                    foreach (var i in (arg_val as System.Collections.IEnumerable))
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
            });
        }

        private static bool IsEnumerable(this object input)
        {
            return (input as System.Collections.IEnumerable) != null && (input as string) == null && (input as byte[]) == null;
        }

        /// <summary>
        /// Processes the parameters for a stored procedure command.
        /// </summary>
        /// <param name="cmd">The command representing the stored procedure.</param>
        /// <param name="args">The arguments to be processed.</param>
        /// <param name="setParameterProperties">An action delegate to set properties of the database parameters.</param>
        /// <returns>An array of database parameters processed from the input arguments.</returns>
        /// <exception cref="ArgumentException">Throws when a value type or string is passed as a stored procedure argument.</exception>
        public static object[] ProcessStoredProcParams(IDbCommand cmd, object[] args, Action<IDbDataParameter, object, PocoColumn> setParameterProperties)
        {
            // For a stored proc, we assume that we're only getting POCOs or parameters
            var result = new List<IDbDataParameter>();

            void ProcessArg(object arg)
            {
                if (arg is IDictionary dict)
                {
                    Type[] arguments = dict.GetType().GetGenericArguments();

                    if (arguments[0] == typeof(string))
                    {
                        foreach (string key in dict.Keys)
                        {
                            AddParameter(key, dict[key]);
                        }
                    }
                }
                else if (arg.IsEnumerable())
                {
                    foreach (var singleArg in (arg as System.Collections.IEnumerable))
                    {
                        ProcessArg(singleArg);
                    }
                }
                else if (arg is IDbDataParameter)
                    result.Add((IDbDataParameter)arg);
                else
                {
                    var type = arg.GetType();
                    if (type.IsValueType || type == typeof(string))
                        throw new ArgumentException($"Value type or string passed as stored procedure argument: {arg}");
                    var readableProps = type.GetProperties().Where(p => p.CanRead);
                    foreach (var prop in readableProps)
                    {
                        AddParameter(prop.Name, prop.GetValue(arg, null));
                    }
                }
            }

            void AddParameter(string name, object value)
            {
                var param = cmd.CreateParameter();
                param.ParameterName = name;
                setParameterProperties(param, value, null);
                result.Add(param);
            }

            foreach (var arg in args)
            {
                ProcessArg(arg);
            }

            return result.ToArray();
        }
    }
}
