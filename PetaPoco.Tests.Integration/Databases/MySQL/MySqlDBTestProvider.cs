// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/07</date>

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    public class MySqlDBTestProvider : DBTestProvider
    {
        protected override Database Database => new Database("mysql");

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MySqlBuildDatabase.sql";
    }
}