// <copyright file="SqlServerCEDatabaseType.cs" company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/05</date>

using System.Data.Common;
using System.Linq;
using PetaPoco.Core;
using PetaPoco.Utilities;

namespace PetaPoco.Providers
{
    public class SqlServerCEDatabaseProviders : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
        {
            return GetFactory("System.Data.SqlServerCe.SqlCeProviderFactory, System.Data.SqlServerCe, Culture=neutral, PublicKeyToken=89845dcd8080cc91");
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
        {
            if (string.IsNullOrEmpty(parts.SqlOrderBy))
                parts.Sql += " ORDER BY ABS(1)";
            var sqlPage = string.Format("{0}\nOFFSET @{1} ROWS FETCH NEXT @{2} ROWS ONLY", parts.Sql, args.Length, args.Length + 1);
            args = args.Concat(new object[] {skip, take}).ToArray();
            return sqlPage;
        }

        public override object ExecuteInsert(Database db, System.Data.IDbCommand cmd, string primaryKeyName)
        {
            ExecuteNonQueryHelper(db, cmd);
            return db.ExecuteScalar<object>("SELECT @@@IDENTITY AS NewID;");
        }
    }
}