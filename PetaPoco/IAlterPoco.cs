using System.Collections.Generic;

namespace PetaPoco
{
    public interface IAlterPoco
    {
        #region Insert

        /// <summary>
        /// Performs an SQL Insert.
        /// </summary>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="poco">A POCO object containing the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or <see langword="null"/> for non-auto-increment tables.</returns>
        object Insert(string tableName, object poco);

        /// <summary>
        /// Performs an SQL Insert.
        /// </summary>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="poco">A POCO object containing the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or <see langword="null"/> for non-auto-increment tables.</returns>
        object Insert(string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Performs an SQL Insert.
        /// </summary>
        /// <remarks>
        /// Inserts a POCO into a table. If the POCO has a property with the same name as the primary key, the id of the new record is assigned to it. Either way, the new id is returned.
        /// </remarks>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="autoIncrement"><see langword="true"/> if the primary key is automatically allocated by the DB.</param>
        /// <param name="poco">A POCO object containing the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or <see langword="null"/> for non-auto-increment tables.</returns>
        object Insert(string tableName, string primaryKeyName, bool autoIncrement, object poco);

        /// <summary>
        /// Performs an SQL Insert.
        /// </summary>
        /// <remarks>
        /// The name of the table, its primary key and whether it's an auto-allocated primary key are retrieved from the POCO's attributes
        /// </remarks>
        /// <param name="poco">A POCO object containing the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or <see langword="null"/> for non-auto-increment tables.</returns>
        object Insert(object poco);

        #endregion

        #region Update

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <param name="tableName">The name of the table to update.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="poco">A POCO object containing the column values to be updated.</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated.</param>
        /// <returns>The number of affected records.</returns>
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <param name="tableName">The name of the table to update.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="poco">A POCO object containing the column values to be updated.</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated.</param>
        /// <param name="columns">The column names of the columns to be updated, or <see langword="null"/> for all.</param>
        /// <returns>The number of affected rows.</returns>
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <param name="tableName">The name of the table to update.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="poco">A POCO object containing the column values to be updated.</param>
        /// <returns>The number of affected rows.</returns>
        int Update(string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <param name="tableName">The name of the table to update.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="poco">A POCO object containing the column values to be updated.</param>
        /// <param name="columns">The column names of the columns to be updated, or <see langword="null"/> for all.</param>
        /// <returns>The number of affected rows.</returns>
        int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns);

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <param name="poco">A POCO object containing the column values to be updated.</param>
        /// <param name="columns">The column names of the columns to be updated, or <see langword="null"/> for all.</param>
        /// <returns>The number of affected rows.</returns>
        int Update(object poco, IEnumerable<string> columns);

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <param name="poco">A POCO object containing the column values to be updated.</param>
        /// <returns>The number of affected rows.</returns>
        int Update(object poco);

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <param name="poco">A POCO object containing the column values to be updated.</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated.</param>
        /// <returns>The number of affected rows.</returns>
        int Update(object poco, object primaryKeyValue);

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <param name="poco">A POCO object containing the column values to be updated.</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated.</param>
        /// <param name="columns">The column names of the columns to be updated, or <see langword="null"/> for all.</param>
        /// <returns>The number of affected rows.</returns>
        int Update(object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes specify the name of the table to update.</typeparam>
        /// <param name="sql">The SQL update and condition clause (everything after <c>UPDATE tablename</c>).</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>The number of affected rows.</returns>
        int Update<T>(string sql, params object[] args);

        /// <summary>
        /// Performs an SQL update.
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes specify the name of the table to update.</typeparam>
        /// <param name="sql">An SQL builder object representing the SQL update and condition clause (everything after <c>UPDATE tablename</c>).</param>
        /// <returns>The number of affected rows.</returns>
        int Update<T>(Sql sql);

        #endregion

        #region Delete

        /// <summary>
        /// Performs an SQL Delete.
        /// </summary>
        /// <param name="tableName">The name of the table to delete from.</param>
        /// <param name="primaryKeyName">The name of the primary key column.</param>
        /// <param name="poco">A POCO object whose primary key value will be used to delete the row.</param>
        /// <returns>The number of rows affected.</returns>
        int Delete(string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Performs an SQL Delete.
        /// </summary>
        /// <param name="tableName">The name of the table to delete from.</param>
        /// <param name="primaryKeyName">The name of the primary key column.</param>
        /// <param name="poco">A POCO object whose primary key value will be used to delete the row, or <see langword="null"/> to use the supplied primary key value.</param>
        /// <param name="primaryKeyValue">The value of the primary key identifying the record to be deleted. If <see langword="null"/>, the primary key will be obtained from the provided <paramref name="poco"/>.</param>
        /// <returns>The number of rows affected.</returns>
        int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        /// Performs an SQL Delete.
        /// </summary>
        /// <param name="poco">A POCO object specifying the table name and primary key value of the row to be deleted.</param>
        /// <returns>The number of rows affected.</returns>
        int Delete(object poco);

        /// <summary>
        /// Performs an SQL Delete.
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes identify the table and primary key to be used in the delete.</typeparam>
        /// <param name="pocoOrPrimaryKey">The value of the primary key of the row to delete.</param>
        /// <returns>The number of affected rows.</returns>
        int Delete<T>(object pocoOrPrimaryKey);

        /// <summary>
        /// Performs an SQL Delete.
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes specify the name of the table to delete from.</typeparam>
        /// <param name="sql">The SQL condition clause identifying the row to delete (everything after <c>DELETE FROM tablename</c>).</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL.</param>
        /// <returns>The number of affected rows.</returns>
        int Delete<T>(string sql, params object[] args);

        /// <summary>
        /// Performs an SQL Delete.
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes specify the name of the table to delete from.</typeparam>
        /// <param name="sql">An SQL builder object representing the SQL condition clause identifying the row to delete (everything after <c>DELETE FROM tablename</c>).</param>
        /// <returns>The number of affected rows.</returns>
        int Delete<T>(Sql sql);

        #endregion

        #region IsNew

        /// <summary>
        /// Checks if a poco represents a new row.
        /// </summary>
        /// <remarks>
        /// This method simply tests if the POCO's primary key column property has a non-default value.
        /// </remarks>
        /// <param name="primaryKeyName">The name of the primary key column.</param>
        /// <param name="poco">The object instance whose "newness" is to be tested.</param>
        /// <returns><see langword="true"/> if the POCO represents a record already in the database.</returns>
        bool IsNew(string primaryKeyName, object poco);

        /// <summary>
        /// Checks if a poco represents a new row.
        /// </summary>
        /// <remarks>
        /// This method simply tests if the POCO's primary key column property has a non-default value.
        /// </remarks>
        /// <param name="poco">The object instance whose "newness" is to be tested.</param>
        /// <returns><see langword="true"/> if the POCO represents a record already in the database.</returns>
        bool IsNew(object poco);

        #endregion

        #region Save

        /// <summary>
        /// Saves a POCO by performing either an INSERT or UPDATE operation.
        /// </summary>
        /// <param name="tableName">The name of the table to be updated.</param>
        /// <param name="primaryKeyName">The name of the primary key column.</param>
        /// <param name="poco">The POCO object to be saved.</param>
        void Save(string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Saves a POCO by performing either an INSERT or UPDATE operation.
        /// </summary>
        /// <param name="poco">The POCO object to be saved.</param>
        void Save(object poco);

        #endregion
    }
}
