// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/31</date>

using System.IO;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    public class FirebirdDBTestProvider : DBTestProvider
    {
#if NETFULL
        protected override IDatabase Database => DatabaseConfiguration.Build().UsingConnectionStringName("firebird").Create();
#else
        protected override IDatabase Database => CreateConfiguration("Firebird").Create();
#endif
        //protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.FirebirdDbBuildDatabase.sql";
        protected override string ScriptResourceName => @"Scripts\FirebirdDbBuildDatabase.sql";

        public override IDatabase Execute()
        {
            var db = Database;
            FbScript script;

            //using (var s = GetType().Assembly.GetManifestResourceStream(ScriptResourceName))
            //{
            //    using (var r = new StreamReader(s, Encoding.UTF8))
            //    {
            //        script = new FbScript(r.ReadToEnd());
            //        script.Parse();
            //    }
            //}
            var sql = File.ReadAllText(ScriptResourceName, Encoding.UTF8);
            script = new FbScript(sql);
            script.Parse();

            try
            {
                using (var con = new FbConnection(db.ConnectionString))
                {
                    con.Open();
                }
            }
            catch
            {
                FbConnection.CreateDatabase(db.ConnectionString);
            }

            using (var con = new FbConnection(db.ConnectionString))
            {
                var be = new FbBatchExecution(con);
                be.AppendSqlStatements(script);
                be.Execute();
            }

            return db;
        }
    }
}