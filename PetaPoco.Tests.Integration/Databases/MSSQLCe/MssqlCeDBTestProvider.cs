using System;
using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.MSSQLCe
{
    public class MssqlCeDBTestProvider : DBTestProvider
    {
        protected override string ConnectionName => "mssqlce";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.MSSQLCeBuildDatabase.sql";

        public MssqlCeDBTestProvider()
        {
            // Hack: Nuget package is old and doesn't support newer content
            // ReSharper disable AssignNullToNotNullAttribute
            var codeBase = typeof(SqlCeConnection).Assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var dllPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            var nativeBinaryPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                $".nuget\\packages\\microsoft.sqlserver.compact\\4.0.8876.1\\NativeBinaries\\{(Environment.Is64BitProcess ? "amd64" : "x86")}"));
            Directory.GetFiles(nativeBinaryPath, "*", SearchOption.AllDirectories).ToList().ForEach(f =>
            {
                var destFilePath = Path.Combine(dllPath, Path.GetFileName(f));
                if (!File.Exists(destFilePath))
                    File.Copy(f, destFilePath);
            });
            // ReSharper restore AssignNullToNotNullAttribute
        }

        public override IDatabase Execute()
        {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "petapoco.sdf")))
            {
                using (var engine = new SqlCeEngine(ConfigurationManager.ConnectionStrings["mssqlce"].ConnectionString))
                {
                    engine.CreateDatabase();
                }
            }

            return base.Execute();
        }

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
