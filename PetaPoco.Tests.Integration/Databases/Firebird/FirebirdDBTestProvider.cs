using System.IO;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Isql;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    public class FirebirdDBTestProvider : DBTestProvider
    {
        protected override string ConnectionName => "Firebird";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.FirebirdDbBuildDatabase.sql";

        public override IDatabase Execute()
        {
            var db = Database;
            FbScript script;

            using (var s = GetType().Assembly.GetManifestResourceStream(ScriptResourceName))
            {
                using (var r = new StreamReader(s, Encoding.UTF8))
                {
                    script = new FbScript(r.ReadToEnd());
                    script.Parse();
                }
            }

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

        public override void AddColumnToTable(string table, string column, string columnType)
        {
            Database.Execute($"ALTER TABLE {Database.Provider.EscapeSqlIdentifier(table)} ADD {Database.Provider.EscapeSqlIdentifier(column)} {columnType} DEFAULT NULL");
        }

        public override void DropColumnFromTable(string table, string column)
        {
            Database.Execute($"ALTER TABLE {Database.Provider.EscapeSqlIdentifier(table)} DROP {Database.Provider.EscapeSqlIdentifier(column)}");
        }
    }
}
