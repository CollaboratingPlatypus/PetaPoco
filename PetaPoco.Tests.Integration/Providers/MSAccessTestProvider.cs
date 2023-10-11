using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace PetaPoco.Tests.Integration.Providers
{
    public class MSAccessTestProvider : TestProvider
    {
        protected override string ConnectionName => "MSAccess";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSAccessBuildDatabase.sql";

        public override IDatabase Execute()
        {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "PetaPoco.accdb")))
            {
                var catalog = new ADOX.Catalog();

                try
                {
                    catalog.Create(ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
                }
                finally
                {
                    Marshal.ReleaseComObject(catalog);
                }
            }

            return base.Execute();
        }

        public override void ExecuteBuildScript(IDatabase database, string script)
        {
            script.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList().ForEach(s =>
            {
                if (string.IsNullOrEmpty(s) || s.StartsWith("--"))
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
