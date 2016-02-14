// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/12</date>

using System.Collections.Generic;

namespace PetaPoco
{
    public interface IAlterPoco
    {
        /// <summary>
        ///     Performs an SQL Insert.
        /// </summary>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables.</returns>
        object Insert(string tableName, object poco);

        /// <summary>
        ///     Performs an SQL Insert.
        /// </summary>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables.</returns>
        object Insert(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Performs an SQL Insert.
        /// </summary>
        /// <param name="tableName">The name of the table to insert into.</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table.</param>
        /// <param name="autoIncrement">True if the primary key is automatically allocated by the DB.</param>
        /// <param name="poco">The POCO object that specifies the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables.</returns>
        /// <remarks>
        ///     Inserts a poco into a table. If the poco has a property with the same name
        ///     as the primary key, the id of the new record is assigned to it. Either way,
        ///     the new id is returned.
        /// </remarks>
        object Insert(string tableName, string primaryKeyName, bool autoIncrement, object poco);

        /// <summary>
        ///     Performs an SQL Insert.
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be inserted.</param>
        /// <returns>The auto allocated primary key of the new record, or null for non-auto-increment tables.</returns>
        /// <remarks>
        ///     The name of the table, it's primary key and whether it's an auto-allocated primary key are retrieved
        ///     from the POCO's attributes
        /// </remarks>
        object Insert(object poco);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <returns>The number of affected records</returns>
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        int Update(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="tableName">The name of the table to update</param>
        /// <param name="primaryKeyName">The name of the primary key column of the table</param>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        int Update(object poco, IEnumerable<string> columns);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <returns>The number of affected rows</returns>
        int Update(object poco);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <returns>The number of affected rows</returns>
        int Update(object poco, object primaryKeyValue);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <param name="poco">The POCO object that specifies the column values to be updated</param>
        /// <param name="primaryKeyValue">The primary key of the record to be updated</param>
        /// <param name="columns">The column names of the columns to be updated, or null for all</param>
        /// <returns>The number of affected rows</returns>
        int Update(object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">The SQL update and condition clause (ie: everything after "UPDATE tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        int Update<T>(string sql, params object[] args);

        /// <summary>
        ///     Performs an SQL update
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to update</typeparam>
        /// <param name="sql">
        ///     An SQL builder object representing the SQL update and condition clause (ie: everything after "UPDATE
        ///     tablename"
        /// </param>
        /// <returns>The number of affected rows</returns>
        int Update<T>(Sql sql);

        /// <summary>
        ///     Performs and SQL Delete
        /// </summary>
        /// <param name="tableName">The name of the table to delete from</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The POCO object whose primary key value will be used to delete the row</param>
        /// <returns>The number of rows affected</returns>
        int Delete(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Performs and SQL Delete
        /// </summary>
        /// <param name="tableName">The name of the table to delete from</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">
        ///     The POCO object whose primary key value will be used to delete the row (or null to use the supplied
        ///     primary key value)
        /// </param>
        /// <param name="primaryKeyValue">
        ///     The value of the primary key identifing the record to be deleted (or null, or get this
        ///     value from the POCO instance)
        /// </param>
        /// <returns>The number of rows affected</returns>
        int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <param name="poco">The POCO object specifying the table name and primary key value of the row to be deleted</param>
        /// <returns>The number of rows affected</returns>
        int Delete(object poco);

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class whose attributes identify the table and primary key to be used in the delete</typeparam>
        /// <param name="pocoOrPrimaryKey">The value of the primary key of the row to delete</param>
        /// <returns></returns>
        int Delete<T>(object pocoOrPrimaryKey);

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">The SQL condition clause identifying the row to delete (ie: everything after "DELETE FROM tablename"</param>
        /// <param name="args">Arguments to any embedded parameters in the SQL</param>
        /// <returns>The number of affected rows</returns>
        int Delete<T>(string sql, params object[] args);

        /// <summary>
        ///     Performs an SQL Delete
        /// </summary>
        /// <typeparam name="T">The POCO class who's attributes specify the name of the table to delete from</typeparam>
        /// <param name="sql">
        ///     An SQL builder object representing the SQL condition clause identifying the row to delete (ie:
        ///     everything after "UPDATE tablename"
        /// </param>
        /// <returns>The number of affected rows</returns>
        int Delete<T>(Sql sql);

        /// <summary>
        ///     Check if a poco represents a new row
        /// </summary>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The object instance whose "newness" is to be tested</param>
        /// <returns>True if the POCO represents a record already in the database</returns>
        /// <remarks>This method simply tests if the POCO's primary key column property has been set to something non-zero.</remarks>
        bool IsNew(string primaryKeyName, object poco);

        /// <summary>
        ///     Check if a poco represents a new row
        /// </summary>
        /// <param name="poco">The object instance whose "newness" is to be tested</param>
        /// <returns>True if the POCO represents a record already in the database</returns>
        /// <remarks>This method simply tests if the POCO's primary key column property has been set to something non-zero.</remarks>
        bool IsNew(object poco);

        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="tableName">The name of the table to be updated</param>
        /// <param name="primaryKeyName">The name of the primary key column</param>
        /// <param name="poco">The POCO object to be saved</param>
        void Save(string tableName, string primaryKeyName, object poco);

        /// <summary>
        ///     Saves a POCO by either performing either an SQL Insert or SQL Update
        /// </summary>
        /// <param name="poco">The POCO object to be saved</param>
        void Save(object poco);
    }
}