// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    public class MssqlDBTestProvider : DBTestProvider
    {
        private string _connectionName = "mssql";

        protected override IDatabase Database => LoadFromConnectionName(_connectionName);

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSSQLBuildDatabase.sql";

        public override IDatabase Execute()
        {
            _connectionName = "mssql_builder";
            base.Execute();
            _connectionName = "mssql";
            return Database;
        }

        public override void ExecuteBuildScript(IDatabase database, string script)
        {
            script.Split(new [] { "GO"}, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(s =>
            {
                base.ExecuteBuildScript(database, s);
            });
        }
    }
}