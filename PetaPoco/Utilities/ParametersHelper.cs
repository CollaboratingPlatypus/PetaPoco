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
    /// This class includes extensions for validating and replacing parameter prefixes, as well as static methods used for processing
    /// parameters for queries and stored procedures.
    /// </remarks>
    internal static class ParametersHelper
    {
        private static Regex ParamPrefixRegex = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
        private static Regex NonWordStartRegex = new Regex(@"^\W*", RegexOptions.Compiled);

        /// <summary>
        /// Replaces all parameter prefixes in the provided SQL statement with the specified replacement string.
        /// </summary>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="replacementPrefix">The replacement parameter prefix.</param>
        /// <returns>The SQL statement with the parameter prefixes replaced.</returns>
        public static string ReplaceParamPrefix(this string sql, string replacementPrefix)
            => ParamPrefixRegex.Replace(sql, m => replacementPrefix + m.Value.Substring(1));

        /// <summary>
        /// Ensures that the provided SQL parameter number is prefixed with the specified prefix string.
        /// </summary>
        /// <param name="value">The parameter number.</param>
        /// <param name="paramPrefix">The prefix string.</param>
        /// <returns>The parameter number, converted to a string and appended to the specified prefix.</returns>
        public static string EnsureParamPrefix(this int value, string paramPrefix)
            => $"{paramPrefix}{value}";

        /// <summary>
        /// Ensures that the provided SQL parameter string is prefixed with the specified prefix string.
        /// </summary>
        /// <param name="value">The parameter name.</param>
        /// <param name="paramPrefix">The prefix string.</param>
        /// <returns>The parameter name appended to the specified prefix string.</returns>
        public static string EnsureParamPrefix(this string value, string paramPrefix)
            => value.StartsWith(paramPrefix) ? value : NonWordStartRegex.Replace(value, paramPrefix);

        /// <summary>
        /// Processes the parameters for an SQL statement.
        /// </summary>
        /// <remarks>
        /// Helper method for processing named parameters from object properties.
        /// </remarks>
        /// <param name="sql">The SQL statement.</param>
        /// <param name="srcArgs">The source arguments to be processed.</param>
        /// <param name="destArgs">The destination list to store the processed arguments.</param>
        /// <returns>The SQL statement with the parameters processed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The number of parameters is less than the count of numbered parameters in the SQL
        /// string.</exception>
        /// <exception cref="ArgumentException">None of the passed parameters have a property with the name used as a named
        /// parameter.</exception>
        public static string ProcessQueryParams(string sql, object[] srcArgs, List<object> destArgs)
        {
            // TODO: Use same collection type for srcArgs and destArgs (`object[]` vs `List<object>`)
            return ParamPrefixRegex.Replace(sql, m =>
            {
                string param = m.Value.Substring(1);

                object arg_val;

                int paramIndex;
                if (int.TryParse(param, out paramIndex))
                {
                    // Numbered parameter
                    if (paramIndex < 0 || paramIndex >= srcArgs.Length)
                        throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", paramIndex, srcArgs.Length, sql));
                    arg_val = srcArgs[paramIndex];
                }
                else
                {
                    bool found = false;
                    arg_val = null;

                    foreach (var o in srcArgs)
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
                        throw new ArgumentException(string.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", param, sql));
                }

                // Expand collections to parameter lists
                if (arg_val.IsEnumerable())
                {
                    var sb = new StringBuilder();
                    foreach (var i in (arg_val as System.Collections.IEnumerable))
                    {
                        sb.Append((sb.Length == 0 ? "@" : ",@") + destArgs.Count.ToString());
                        destArgs.Add(i);
                    }

                    return sb.ToString();
                }
                else
                {
                    destArgs.Add(arg_val);
                    return "@" + (destArgs.Count - 1).ToString();
                }
            });
        }

        /// <summary>
        /// Processes the parameters for a stored procedure command.
        /// </summary>
        /// <param name="cmd">The SQL command representing the stored procedure.</param>
        /// <param name="args">The arguments to be processed.</param>
        /// <param name="setPropertiesAction">An action used to set the database parameter properties.</param>
        /// <returns>An array of database parameters processed from the input arguments.</returns>
        /// <exception cref="ArgumentException">Value type or string passed as stored procedure argument.</exception>
        public static object[] ProcessStoredProcParams(IDbCommand cmd, object[] args, Action<IDbDataParameter, object, PocoColumn> setPropertiesAction)
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
                        throw new ArgumentException($"Value type or string passed as stored procedure argument: {arg}", nameof(args));
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
                setPropertiesAction(param, value, null);
                result.Add(param);
            }

            foreach (var arg in args)
            {
                ProcessArg(arg);
            }

            return result.ToArray();
        }

        private static bool IsEnumerable(this object value)
            => (value as IEnumerable) != null && (value as string) == null && (value as byte[]) == null;
    }
}
