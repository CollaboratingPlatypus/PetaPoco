﻿using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using PetaPoco.Core;
using PetaPoco.Utilities;

namespace PetaPoco.Providers
{
    public class MsAccessDbDatabaseProvider : DatabaseProvider
    {
        public override DbProviderFactory GetFactory()
            => GetFactory("System.Data.OleDb.OleDbFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

        public override object ExecuteInsert(Database database, IDbCommand cmd, string primaryKeyName)
        {
            ExecuteNonQueryHelper(database, cmd);
            cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
            return ExecuteScalarHelper(database, cmd);
        }

        public override async Task<object> ExecuteInsertAsync(CancellationToken cancellationToken, Database database, IDbCommand cmd, string primaryKeyName)
        {
            await ExecuteNonQueryHelperAsync(cancellationToken, database, cmd).ConfigureAwait(false);
            cmd.CommandText = "SELECT @@IDENTITY AS NewID;";
            return await ExecuteScalarHelperAsync(cancellationToken, database, cmd).ConfigureAwait(false);
        }

        public override string BuildPageQuery(long skip, long take, SQLParts parts, ref object[] args)
            => throw new NotSupportedException("The Access provider does not support paging.");
    }
}