// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

namespace PetaPoco.Tests.Integration.Databases.Postgres
{
    public class PostgresDBTestProvider : DBTestProvider
    {
        protected override Database Database => new Database("postgres");

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.PostgresBuildDatabase.sql";
    }
}