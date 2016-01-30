// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/01/29</date>

using System.Linq;
using PetaPoco.Tests.Integration.Databases;

namespace PetaPoco.Tests.Integration.x86.Databases.MSAccess
{
    public class MsAccessDBTestProvider : DBTestProvider
    {
        protected override IDatabase Database => DatabaseConfiguration.Build().UsingConnectionStringName("msaccess").Create();

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.x86.Scripts.MSAccessBuildDatabase.sql";

        public override void ExecuteBuildScript(IDatabase database, string script)
        {
            script.Split(';').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList().ForEach(s =>
            {
                if (s.StartsWith("DROP"))
                {
                    try
                    {
                        base.ExecuteBuildScript(database, s);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    base.ExecuteBuildScript(database, s);
                }
            });
        }
    }
}