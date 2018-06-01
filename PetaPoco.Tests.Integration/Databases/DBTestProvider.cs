// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;
using System.IO;
using System.Text;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract partial class DBTestProvider : IDisposable
    {
        protected abstract IDatabase Database { get; }

        protected abstract string ScriptResourceName { get; }

        public virtual void Dispose()
        {
        }

        public virtual IDatabase Execute()
        {
            var db = Database;
            //using (var s = this.GetType().Assembly.GetManifestResourceStream(ScriptResourceName))
            //{
            //    using (var r = new StreamReader(s, Encoding.UTF8))
            //    {
            //        ExecuteBuildScript(db, r.ReadToEnd());
            //    }
            //}
            var sql = File.ReadAllText(ScriptResourceName, Encoding.UTF8);
            ExecuteBuildScript(db, sql);
            return db;
        }

        public virtual void ExecuteBuildScript(IDatabase database, string script)
        {
            database.Execute(script);
        }
    }
}