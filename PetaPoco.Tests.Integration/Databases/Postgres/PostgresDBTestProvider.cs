// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    public class PostgresDBTestProvider : DBTestProvider
    {
        protected override IDatabase Database => LoadFromConnectionName("Postgres");

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.PostgresBuildDatabase.sql";
    }
}