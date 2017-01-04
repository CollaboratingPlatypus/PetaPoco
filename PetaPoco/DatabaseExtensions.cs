using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetaPoco
{
    /// <summary>
    /// Extensions of IDatabase
    /// </summary>
    public static class DatabaseExtensions
    {
        #region IExecute

        public static Task<int> ExecuteAsync(this IExecute execute, string sql, params object[] args)
        {
            return Task.Run(() => execute.Execute(sql, args));
        }

        public static Task<int> ExecuteAsync(this IExecute execute, Sql sql)
        {
            return Task.Run(() => execute.Execute(sql));
        }

        public static Task<T> ExecuteScalarAsync<T>(this IExecute execute, string sql, params object[] args)
        {
            return Task.Run(() => execute.ExecuteScalar<T>(sql, args));
        }

        public static Task<T> ExecuteScalarAsync<T>(this IExecute execute, Sql sql)
        {
            return Task.Run(() => execute.ExecuteScalar<T>(sql));
        }

        #endregion

        #region IAlterPoco

        public static Task<object> InsertAsync(this IAlterPoco alertPoco, string tableName, object poco)
        {
            return Task.Run(() => alertPoco.Insert(tableName, poco));
        }
        public static Task<object> InsertAsync(this IAlterPoco alertPoco, string tableName, string primaryKeyName, object poco)
        {
            return Task.Run(() => alertPoco.Insert(tableName, primaryKeyName, poco));
        }
        public static Task<object> InsertAsync(this IAlterPoco alertPoco, string tableName, string primaryKeyName, bool autoIncrement, object poco)
        {
            return Task.Run(() => alertPoco.Insert(tableName, primaryKeyName, autoIncrement, poco));
        }
        public static Task<object> InsertAsync(this IAlterPoco alertPoco, object poco)
        {
            return Task.Run(() => alertPoco.Insert(poco));
        }
        public static Task<int> UpdateAsync(this IAlterPoco alertPoco, string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            return Task.Run(() => alertPoco.Update(tableName, primaryKeyName, poco, primaryKeyValue));
        }
        public static Task<int> UpdateAsync(this IAlterPoco alertPoco, string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            return Task.Run(() => alertPoco.Update(tableName, primaryKeyName, poco, primaryKeyValue, columns));
        }
        public static Task<int> UpdateAsync(this IAlterPoco alertPoco, string tableName, string primaryKeyName, object poco)
        {
            return Task.Run(() => alertPoco.Update(tableName, primaryKeyName, poco));
        }
        public static Task<int> UpdateAsync(this IAlterPoco alertPoco, string tableName, string primaryKeyName, object poco, IEnumerable<string> columns)
        {
            return Task.Run(() => alertPoco.Update(tableName, primaryKeyName, poco, columns));
        }
        public static Task<int> UpdateAsync(this IAlterPoco alertPoco, object poco, IEnumerable<string> columns)
        {
            return Task.Run(() => alertPoco.Update(poco, columns));
        }
        public static Task<int> UpdateAsync(this IAlterPoco alertPoco, object poco)
        {
            return Task.Run(() => alertPoco.Update(poco));
        }
        public static Task<int> UpdateAsync(this IAlterPoco alertPoco, object poco, object primaryKeyValue)
        {
            return Task.Run(() => alertPoco.Update(poco, primaryKeyValue));
        }
        public static Task<int> UpdateAsync(this IAlterPoco alertPoco, object poco, object primaryKeyValue, IEnumerable<string> columns)
        {
            return Task.Run(() => alertPoco.Update(poco, primaryKeyValue, columns));
        }
        public static Task<int> UpdateAsync<T>(this IAlterPoco alertPoco, string sql, params object[] args)
        {
            return Task.Run(() => alertPoco.Update<T>(sql, args));
        }
        public static Task<int> UpdateAsync<T>(this IAlterPoco alertPoco, Sql sql)
        {
            return Task.Run(() => alertPoco.Update<T>(sql));
        }
        public static Task<int> DeleteAsync(this IAlterPoco alertPoco, string tableName, string primaryKeyName, object poco)
        {
            return Task.Run(() => alertPoco.Delete(tableName, primaryKeyName, poco));
        }
        public static Task<int> DeleteAsync(this IAlterPoco alertPoco, string tableName, string primaryKeyName, object poco, object primaryKeyValue)
        {
            return Task.Run(() => alertPoco.Delete(tableName, primaryKeyName, poco, primaryKeyValue));
        }
        public static Task<int> DeleteAsync(this IAlterPoco alertPoco, object poco)
        {
            return Task.Run(() => alertPoco.Delete(poco));
        }
        public static Task<int> DeleteAsync<T>(this IAlterPoco alertPoco, object pocoOrPrimaryKey)
        {
            return Task.Run(() => alertPoco.Delete<T>(pocoOrPrimaryKey));
        }
        public static Task<int> DeleteAsync<T>(this IAlterPoco alertPoco, string sql, params object[] args)
        {
            return Task.Run(() => alertPoco.Delete<T>(sql, args));
        }
        public static Task<int> DeleteAsync<T>(this IAlterPoco alertPoco, Sql sql)
        {
            return Task.Run(() => alertPoco.Delete<T>(sql));
        }
        public static Task<bool> IsNewAsync(this IAlterPoco alertPoco, string primaryKeyName, object poco)
        {
            return Task.Run(() => alertPoco.IsNew(primaryKeyName, poco));
        }
        public static Task<bool> IsNewAsync(this IAlterPoco alertPoco, object poco)
        {
            return Task.Run(() => alertPoco.IsNew(poco));
        }
        public static Task SaveAsync(this IAlterPoco alertPoco, string tableName, string primaryKeyName, object poco)
        {
            return Task.Run(() => alertPoco.Save(tableName, primaryKeyName, poco));
        }
        public static Task SaveAsync(this IAlterPoco alertPoco, object poco)
        {
            return Task.Run(() => alertPoco.Save(poco));
        }

        #endregion

        #region IQuery

        public static Task<IEnumerable<T>> QueryAsync<T>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.Query<T>(sql, args));
        }
        public static Task<IEnumerable<T>> QueryAsync<T>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.Query<T>(sql));
        }
        public static Task<IEnumerable<TRet>> QueryAsync<T1, T2, TRet>(this IQuery query, Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            return Task.Run(() => query.Query(cb, sql, args));
        }
        public static Task<IEnumerable<TRet>> QueryAsync<T1, T2, T3, TRet>(this IQuery query, Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
        {
            return Task.Run(() => query.Query(cb, sql, args));
        }
        public static Task<IEnumerable<TRet>> QueryAsync<T1, T2, T3, T4, TRet>(this IQuery query, Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
        {
            return Task.Run(() => query.Query(cb, sql, args));
        }
        public static Task<IEnumerable<TRet>> QueryAsync<T1, T2, TRet>(this IQuery query, Func<T1, T2, TRet> cb, Sql sql)
        {
            return Task.Run(() => query.Query(cb, sql));
        }
        public static Task<IEnumerable<TRet>> QueryAsync<T1, T2, T3, TRet>(this IQuery query, Func<T1, T2, T3, TRet> cb, Sql sql)
        {
            return Task.Run(() => query.Query(cb, sql));
        }
        public static Task<IEnumerable<TRet>> QueryAsync<T1, T2, T3, T4, TRet>(this IQuery query, Func<T1, T2, T3, T4, TRet> cb, Sql sql)
        {
            return Task.Run(() => query.Query(cb, sql));
        }
        public static Task<IEnumerable<T1>> QueryAsync<T1, T2>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.Query<T1, T2>(sql, args));
        }
        public static Task<IEnumerable<T1>> QueryAsync<T1, T2, T3>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.Query<T1, T2, T3>(sql, args));
        }
        public static Task<IEnumerable<T1>> QueryAsync<T1, T2, T3, T4>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.Query<T1, T2, T3, T4>(sql, args));
        }
        public static Task<IEnumerable<T1>> QueryAsync<T1, T2>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.Query<T1, T2>(sql));
        }
        public static Task<IEnumerable<T1>> QueryAsync<T1, T2, T3>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.Query<T1, T2, T3>(sql));
        }
        public static Task<IEnumerable<T1>> QueryAsync<T1, T2, T3, T4>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.Query<T1, T2, T3, T4>(sql));
        }
        public static Task<IEnumerable<TRet>> QueryAsync<TRet>(this IQuery query, Type[] types, object cb, string sql, params object[] args)
        {
            return Task.Run(() => query.Query<TRet>(types, cb, sql, args));
        }
        public static Task<List<T>> FetchAsync<T>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.Fetch<T>(sql, args));
        }
        public static Task<List<T>> FetchAsync<T>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.Fetch<T>(sql));
        }
        public static Task<Page<T>> PageAsync<T>(this IQuery query, long page, long itemsPerPage, string sqlCount, object[] countArgs, string sqlPage, object[] pageArgs)
        {
            return Task.Run(() => query.Page<T>(page, itemsPerPage, sqlCount, countArgs, sqlPage, pageArgs));
        }
        public static Task<Page<T>> PageAsync<T>(this IQuery query, long page, long itemsPerPage, string sql, params object[] args)
        {
            return Task.Run(() => query.Page<T>(page, itemsPerPage, sql, args));
        }
        public static Task<Page<T>> PageAsync<T>(this IQuery query, long page, long itemsPerPage, Sql sql)
        {
            return Task.Run(() => query.Page<T>(page, itemsPerPage, sql));
        }
        public static Task<Page<T>> PageAsync<T>(this IQuery query, long page, long itemsPerPage, Sql sqlCount, Sql sqlPage)
        {
            return Task.Run(() => query.Page<T>(page, itemsPerPage, sqlCount, sqlPage));
        }
        public static Task<List<T>> FetchAsync<T>(this IQuery query, long page, long itemsPerPage, string sql, params object[] args)
        {
            return Task.Run(() => query.Fetch<T>(page, itemsPerPage, sql, args));
        }
        public static Task<List<T>> FetchAsync<T>(this IQuery query, long page, long itemsPerPage, Sql sql)
        {
            return Task.Run(() => query.Fetch<T>(page, itemsPerPage, sql));
        }
        public static Task<List<T>> SkipTakeAsync<T>(this IQuery query, long skip, long take, string sql, params object[] args)
        {
            return Task.Run(() => query.SkipTake<T>(skip, take, sql, args));
        }
        public static Task<List<T>> SkipTakeAsync<T>(this IQuery query, long skip, long take, Sql sql)
        {
            return Task.Run(() => query.SkipTake<T>(skip, take, sql));
        }
        public static Task<bool> ExistsAsync<T>(this IQuery query, object primaryKey)
        {
            return Task.Run(() => query.Exists<T>(primaryKey));
        }
        public static Task<bool> ExistsAsync<T>(this IQuery query, string sqlCondition, params object[] args)
        {
            return Task.Run(() => query.Exists<T>(sqlCondition, args));
        }
        public static Task<T> SingleAsync<T>(this IQuery query, object primaryKey)
        {
            return Task.Run(() => query.Single<T>(primaryKey));
        }
        public static Task<T> SingleAsync<T>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.Single<T>(sql, args));
        }
        public static Task<T> SingleAsync<T>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.Single<T>(sql));
        }
        public static Task<T> SingleOrDefaultAsync<T>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.SingleOrDefault<T>(sql));
        }
        public static Task<T> SingleOrDefaultAsync<T>(this IQuery query, object primaryKey)
        {
            return Task.Run(() => query.SingleOrDefault<T>(primaryKey));
        }
        public static Task<T> SingleOrDefaultAsync<T>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.SingleOrDefault<T>(sql, args));
        }
        public static Task<T> FirstAsync<T>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.First<T>(sql, args));
        }
        public static Task<T> FirstAsync<T>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.First<T>(sql));
        }
        public static Task<T> FirstOrDefaultAsync<T>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.FirstOrDefault<T>(sql, args));
        }
        public static Task<T> FirstOrDefaultAsync<T>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.FirstOrDefault<T>(sql));
        }
        public static Task<List<TRet>> FetchAsync<T1, T2, TRet>(this IQuery query, Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            return Task.Run(() => query.Fetch(cb, sql, args));
        }
        public static Task<List<TRet>> FetchAsync<T1, T2, T3, TRet>(this IQuery query, Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
        {
            return Task.Run(() => query.Fetch(cb, sql, args));
        }
        public static Task<List<TRet>> FetchAsync<T1, T2, T3, T4, TRet>(this IQuery query, Func<T1, T2, T3, T4, TRet> cb, string sql, params object[] args)
        {
            return Task.Run(() => query.Fetch(cb, sql, args));
        }
        public static Task<List<TRet>> FetchAsync<T1, T2, TRet>(this IQuery query, Func<T1, T2, TRet> cb, Sql sql)
        {
            return Task.Run(() => query.Fetch(cb, sql));
        }
        public static Task<List<TRet>> FetchAsync<T1, T2, T3, TRet>(this IQuery query, Func<T1, T2, T3, TRet> cb, Sql sql)
        {
            return Task.Run(() => query.Fetch(cb, sql));
        }
        public static Task<List<TRet>> FetchAsync<T1, T2, T3, T4, TRet>(this IQuery query, Func<T1, T2, T3, T4, TRet> cb, Sql sql)
        {
            return Task.Run(() => query.Fetch(cb, sql));
        }
        public static Task<List<T1>> FetchAsync<T1, T2>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.Fetch<T1, T2>(sql, args));
        }
        public static Task<List<T1>> FetchAsync<T1, T2, T3>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.Fetch<T1, T2, T3>(sql, args));
        }
        public static Task<List<T1>> FetchAsync<T1, T2, T3, T4>(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.Fetch<T1, T2, T3, T4>(sql, args));
        }
        public static Task<List<T1>> FetchAsync<T1, T2>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.Fetch<T1, T2>(sql));
        }
        public static Task<List<T1>> FetchAsync<T1, T2, T3>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.Fetch<T1, T2, T3>(sql));
        }
        public static Task<List<T1>> FetchAsync<T1, T2, T3, T4>(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.Fetch<T1, T2, T3, T4>(sql));
        }
        public static Task<IGridReader> QueryMultipleAsync(this IQuery query, Sql sql)
        {
            return Task.Run(() => query.QueryMultiple(sql));
        }
        public static Task<IGridReader> QueryMultipleAsync(this IQuery query, string sql, params object[] args)
        {
            return Task.Run(() => query.QueryMultiple(sql, args));
        }

        #endregion
    }
}