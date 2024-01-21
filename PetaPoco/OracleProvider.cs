using System;
using System.Data.Common;
using System.Reflection;

namespace PetaPoco
{
    /// <summary>
    /// Provides an implementation of <see cref="DbProviderFactory"/> for Oracle databases using the unmanaged Oracle Data Provider.
    /// </summary>
    /// <remarks>
    /// This provider uses the "Oracle.DataAccess.Client" ADO.NET driver for data access. For later versions of Oracle, the managed <see
    /// cref="Providers.OracleDatabaseProvider"/> class should work fine. Uses reflection to load "Oracle.DataAccess" assembly and in-turn
    /// create connections and commands.
    /// <para>Thanks to Adam Schroder (@schotime) for this. <i>Currently untested.</i></para>
    /// </remarks>
    /// <example>
    /// <code language="cs" title="OracleProvider Usage">
    /// <![CDATA[
    /// var db = new PetaPoco.Database("CONNECTION_STRING", new PetaPoco.OracleProvider());
    /// ]]>
    /// </code>
    /// Or in your app/web config (be sure to change <c>ASSEMBLY_NAME</c> to the name of your assembly containing OracleProvider.cs):
    /// <code language="xml" title="OracleProvider web/app.config Usage">
    /// <![CDATA[
    /// <connectionStrings>
    ///     <add name="oracle"
    ///          connectionString="CONNECTION_STRING"
    ///          providerName="Oracle" />
    /// </connectionStrings>
    /// <system.data>
    ///     <DbProviderFactories>
    ///         <add name="PetaPoco Oracle Provider"
    ///              invariant="Oracle"
    ///              description="PetaPoco Oracle Provider"
    ///              type="PetaPoco.OracleProvider, ASSEMBLY_NAME" />
    ///     </DbProviderFactories>
    /// </system.data>
    /// ]]>
    /// </code>
    /// </example>
    public class OracleProvider : DbProviderFactory
    {
        private const string _assemblyName = "Oracle.DataAccess";
        private const string _connectionTypeName = "Oracle.DataAccess.Client.OracleConnection";
        private const string _commandTypeName = "Oracle.DataAccess.Client.OracleCommand";
        private static Type _connectionType;
        private static Type _commandType;

        /// <summary>
        /// Singleton instance of OracleProvider. Required for DbProviderFactories.GetFactory() to work.
        /// </summary>
        public static OracleProvider Instance = new OracleProvider();

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleProvider"/> class.
        /// </summary>
        /// <exception cref="InvalidOperationException">Unable to find the connection type from the assembly.</exception>
        public OracleProvider()
        {
            _connectionType = TypeFromAssembly(_connectionTypeName, _assemblyName);
            _commandType = TypeFromAssembly(_commandTypeName, _assemblyName);
            if (_connectionType == null)
                throw new InvalidOperationException("Can't find Connection type: " + _connectionTypeName);
        }

        /// <summary>
        /// Creates a new instance of an OracleConnection.
        /// </summary>
        /// <returns>A new <see cref="DbConnection"/>.</returns>
        public override DbConnection CreateConnection()
        {
            return (DbConnection)Activator.CreateInstance(_connectionType);
        }

        /// <summary>
        /// Creates a new instance of an OracleCommand.
        /// </summary>
        /// <returns>A new <see cref="DbCommand"/>.</returns>
        public override DbCommand CreateCommand()
        {
            DbCommand command = (DbCommand)Activator.CreateInstance(_commandType);

            var oracleCommandBindByName = _commandType.GetProperty("BindByName");
            oracleCommandBindByName.SetValue(command, true, null);

            return command;
        }

        /// <summary>
        /// Returns the Type for the specified <paramref name="typeName"/> from the provided <paramref name="assemblyName"/>.
        /// </summary>
        /// <param name="typeName">The name of the type to get.</param>
        /// <param name="assemblyName">The name of the assembly to get the type from.</param>
        /// <returns>The Type, or <see langword="null"/> if unable to locate it.</returns>
        /// <exception cref="TypeLoadException">Unable to load <paramref name="typeName"/>.</exception>
        /// <exception cref="InvalidOperationException">Unable to find the <paramref name="assemblyName"/>.</exception>
        public static Type TypeFromAssembly(string typeName, string assemblyName)
        {
            try
            {
                // Try to get the type from an already loaded assembly
                Type type = Type.GetType(typeName);

                if (type != null)
                {
                    return type;
                }

                if (assemblyName == null)
                {
                    // No assembly was specified for the type, so just fail
                    string message = "Could not load type " + typeName + ". Possible cause: no assembly name specified.";
                    throw new TypeLoadException(message);
                }

                Assembly assembly = Assembly.Load(assemblyName);

                if (assembly == null)
                {
                    throw new InvalidOperationException("Cannot find assembly: " + assemblyName);
                }

                type = assembly.GetType(typeName);

                if (type == null)
                {
                    return null;
                }

                return type;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
