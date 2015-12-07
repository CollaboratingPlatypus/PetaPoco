// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/07</date>

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    public class MssqlDBTestProvider : DBTestProvider
    {
        protected override Database Database => new Database("mssql");

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSSQLBuildDatabase.sql";
    }
}