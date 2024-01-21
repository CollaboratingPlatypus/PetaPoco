using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Providers
{
    public class SQLiteSystemDataTestProvider : SQLiteTestProvider
    {
        protected override string ConnectionName => "SQLite";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.SQLiteBuildDatabase.sql";
    }
}
