using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data;
using System.Text;
using System.Reflection;

namespace PetaPoco
{
	/* 
	Thanks to Adam Schroder (@schotime) for this.
	
	This extra file provides an implementation of DbProviderFactory for early versions of the Oracle
	drivers that don't include include it.  For later versions of Oracle, the standard OracleProviderFactory
	class should work fine
	
	Uses reflection to load Oracle.DataAccess assembly and in-turn create connections and commands
	
	Currently untested.
	
	Usage:   
		
			new PetaPoco.Database("<connstring>", new PetaPoco.OracleProvider())
	
	Or in your app/web config (be sure to change ASSEMBLYNAME to the name of your 
	assembly containing OracleProvider.cs)
	
		<connectionStrings>
			<add
				name="oracle"
				connectionString="WHATEVER"
				providerName="Oracle"
				/>
		</connectionStrings>

		<system.data>
			<DbProviderFactories>
				<add name="PetaPoco Oracle Provider" invariant="Oracle" description="PetaPoco Oracle Provider" 
								type="PetaPoco.OracleProvider, ASSEMBLYNAME" />
			</DbProviderFactories>
		</system.data>
	 
	

	 */




	public class OracleProvider : DbProviderFactory
	{
		private const string _assemblyName = "Oracle.DataAccess";
		private const string _connectionTypeName = "Oracle.DataAccess.Client.OracleConnection";
		private const string _commandTypeName = "Oracle.DataAccess.Client.OracleCommand";
		private static Type _connectionType;
		private static Type _commandType;

		// Required for DbProviderFactories.GetFactory() to work.
		public static OracleProvider Instance = new OracleProvider();

		public OracleProvider()
		{
			_connectionType = ReflectHelper.TypeFromAssembly(_connectionTypeName, _assemblyName);
			_commandType = ReflectHelper.TypeFromAssembly(_commandTypeName, _assemblyName);
			if (_connectionType == null)
				throw new InvalidOperationException("Can't find Connection type: " + _connectionTypeName);
		}

		public override DbConnection CreateConnection()
		{
            return (DbConnection)Activator.CreateInstance(_connectionType);
		}

		public override DbCommand CreateCommand()
		{
			DbCommand command = (DbCommand)Activator.CreateInstance(_commandType);

			var oracleCommandBindByName = _commandType.GetProperty("BindByName");
			oracleCommandBindByName.SetValue(command, true, null);

			return command;
		}
	}
	
	
	public class ReflectHelper
    {
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
                    throw new InvalidOperationException("Can't find assembly: " + assemblyName);
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
