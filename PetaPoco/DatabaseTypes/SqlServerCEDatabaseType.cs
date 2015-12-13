// <copyright file="SqlServerCEDatabaseType.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System.Linq;
using PetaPoco.Core;
using PetaPoco.Internal;
using PetaPoco.Utilities;

namespace PetaPoco.DatabaseTypes
{
    public class SqlServerCEDatabaseType : DatabaseType
    {
        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            var sqlPage = string.Format("{0}\nOFFSET @{1} ROWS FETCH NEXT @{2} ROWS ONLY", parts.Sql, args.Length, args.Length + 1);
            args = args.Concat(new object[] {skip, take}).ToArray();
            return sqlPage;
        }

        public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string primaryKeyName)
        {
            db.ExecuteNonQueryHelper(cmd);
            return db.ExecuteScalar<object>("SELECT @@@IDENTITY AS NewID;");
        }
    }
}