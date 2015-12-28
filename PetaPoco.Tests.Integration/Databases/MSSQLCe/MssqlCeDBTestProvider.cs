// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;
using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    public class MssqlCeDBTestProvider : DBTestProvider
    {
        protected override IDatabase Database => DatabaseConfiguration.Build().UsingConnectionStringName("mssqlce").Create();

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSSQLCeBuildDatabase.sql";

        public override IDatabase Execute()
        {
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "petapoco.sdf")))
            {
                File.Delete(Path.Combine(Environment.CurrentDirectory, "petapoco.sdf"));
            }

            using (var engine = new SqlCeEngine(ConfigurationManager.ConnectionStrings["mssqlce"].ConnectionString))
            {
                engine.CreateDatabase();
            }

            return base.Execute();
        }

        public override void ExecuteBuildScript(IDatabase database, string script)
        {
            script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(s => database.Execute(s));
        }
    }
}