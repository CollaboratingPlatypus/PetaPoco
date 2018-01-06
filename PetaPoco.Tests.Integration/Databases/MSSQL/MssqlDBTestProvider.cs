// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

namespace PetaPoco.Tests.Integration.Databases.MSSQL
{
    public class MssqlDBTestProvider : DBTestProvider
    {
#if NETFULL
        protected override IDatabase Database => DatabaseConfiguration.Build().UsingConnectionStringName("mssql").Create();
#else
        protected override IDatabase Database => CreateConfiguration("MsSQL").Create();
#endif
        //protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSSQLBuildDatabase.sql";
        protected override string ScriptResourceName => @"Scripts\MSSQLBuildDatabase.sql";
    }
}