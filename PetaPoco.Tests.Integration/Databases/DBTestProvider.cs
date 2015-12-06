// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/06</date>

using System;
using System.IO;
using System.Text;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class DBTestProvider : IDisposable
    {
        protected abstract Database Database { get; }

        protected abstract string ScriptResourceName { get; }

        public virtual Database Execute()
        {
            var db = Database;
            using (var s = this.GetType().Assembly.GetManifestResourceStream(ScriptResourceName))
            {
                using (var r = new StreamReader(s, Encoding.UTF8))
                {
                    ExecuteBuildScript(db, r.ReadToEnd());
                }
            }
            return db;
        }

        public virtual void ExecuteBuildScript(Database database, string script)
        {
            database.Execute(script);
        }

        public virtual void Dispose()
        {
        }
    }
}