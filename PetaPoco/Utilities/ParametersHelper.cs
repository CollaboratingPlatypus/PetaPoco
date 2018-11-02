// <copyright file="ParametersHelper.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using PetaPoco.Core;
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
        private static Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
        // Helper to handle named parameters from object properties
        public static string ProcessQueryParams(string sql, object[] args_src, List<object> args_dest)
        {
            return rxParams.Replace(sql, m =>
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

        public static object[] ProcessStoredProcParams(IDbCommand cmd, object[] args)
        {
            // TODO: args could be POCOs, or params, or lists of params
            // If it's a param, we assume it doesn't need fixing up.
            // TODO: This needs provider and pi in order to use SetParameterValue!            
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
                        throw new Exception();
                    var readableProps = type.GetProperties().Where(p => p.CanRead);
                    foreach (var prop in readableProps)
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = prop.Name;
                        param.Value = prop.GetValue(arg, null);
                        result.Add(param);
                    }
                }
            }

            return result.ToArray();
        }

        public static void SetParameterValue(IDbDataParameter p, object value, IProvider provider, PropertyInfo pi)
        {
            // Assign the parameter value
            if (value == null)
            {
                p.Value = DBNull.Value;

                if (pi?.PropertyType.Name == "Byte[]")
                {
                    p.DbType = DbType.Binary;
                }
            }
            else
            {
                // Give the database type first crack at converting to DB required type
                value = provider.MapParameterValue(value);

                var t = value.GetType();
                if (t.IsEnum) // PostgreSQL .NET driver wont cast enum to int
                {
                    p.Value = Convert.ChangeType(value, ((Enum)value).GetTypeCode());
                }
                else if (t == typeof(Guid) && !provider.HasNativeGuidSupport)
                {
                    p.Value = value.ToString();
                    p.DbType = DbType.String;
                    p.Size = 40;
                }
                else if (t == typeof(string))
                {
                    // out of memory exception occurs if trying to save more than 4000 characters to SQL Server CE NText column. Set before attempting to set Size, or Size will always max out at 4000
                    if ((value as string).Length + 1 > 4000 && p.GetType().Name == "SqlCeParameter")
                        p.GetType().GetProperty("SqlDbType").SetValue(p, SqlDbType.NText, null);

                    p.Size = Math.Max((value as string).Length + 1, 4000); // Help query plan caching by using common size
                    p.Value = value;
                }
                else if (t == typeof(AnsiString))
                {
                    // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                    p.Size = Math.Max((value as AnsiString).Value.Length + 1, 4000);
                    p.Value = (value as AnsiString).Value;
                    p.DbType = DbType.AnsiString;
                }
                else if (value.GetType().Name == "SqlGeography") //SqlGeography is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                }
                else if (value.GetType().Name == "SqlGeometry") //SqlGeometry is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                }
                else
                {
                    p.Value = value;
                }
            }
        }
    }
}