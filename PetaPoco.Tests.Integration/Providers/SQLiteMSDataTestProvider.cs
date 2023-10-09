using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Providers
{
    public class SQLiteMSDataTestProvider : SQLiteTestProvider
    {
        protected override string ConnectionName => "SQLiteMSData";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.SQLiteBuildDatabase.sql";
    }
}
