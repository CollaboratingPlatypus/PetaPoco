using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    public class MsAccessDBTestProvider : DBTestProvider
    {
        protected override string ConnectionName => "msaccess";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSAccessBuildDatabase.sql";

        public override void ExecuteBuildScript(IDatabase database, string script)
        {
            script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList().ForEach(s =>
            {
                if (String.IsNullOrEmpty(s) || s.StartsWith("--"))
                    return;

                if (s.StartsWith("DROP"))
                {
                    try
                    {
                        base.ExecuteBuildScript(database, s);
                    }
                    catch
                    {
                    }

                    return;
                }

                base.ExecuteBuildScript(database, s);
            });
        }
    }
}
