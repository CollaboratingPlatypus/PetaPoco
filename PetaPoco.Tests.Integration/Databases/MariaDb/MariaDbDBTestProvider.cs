// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/11</date>

namespace PetaPoco.Tests.Integration.Databases.MariaDb
{
    public class MariaDbDBTestProvider : DBTestProvider
    {
        protected override IDatabase Database => DatabaseConfiguration.Build().UsingConnectionStringName("MariaDb").Create();

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MariaDbBuildDatabase.sql";
    }
}