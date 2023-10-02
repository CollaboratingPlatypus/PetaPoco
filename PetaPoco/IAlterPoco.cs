using System.Collections.Generic;

namespace PetaPoco
{
    /// <summary>
    /// Specifies a set of methods for performing SQL operations on POCOs such as Insert, Update, Delete, and Save.
    /// </summary>
    public interface IAlterPoco
    {
        #region Insert

        /// <summary>
        /// Inserts a new record and returns the primary key of the newly inserted record.
        /// </summary>
        /// <inheritdoc cref="Insert(string, object)"/>
        object Insert(object poco);

        /// <remarks>
        /// If a mapped primary key column is auto-incrementing and <see cref="TableInfo.AutoIncrement"/> is <see langword="true"/>, the
        /// primary key property of the POCO will be updated with the new record's auto-incremented ID.
        /// </remarks>
        /// <inheritdoc cref="Insert(string, string, bool, object)"/>
        object Insert(string tableName, object poco);

        /// <remarks>
        /// If <paramref name="primaryKeyName"/> represents an auto-incrementing column and <see cref="TableInfo.AutoIncrement"/> is <see
        /// langword="true"/>, the primary key property of the POCO will be updated with the new record's auto-incremented ID.
        /// </remarks>
        /// <inheritdoc cref="Insert(string, string, bool, object)"/>
        object Insert(string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Inserts a new record into the specified table and returns the primary key of the newly inserted record.
        /// </summary>
        /// <remarks>
        /// If <paramref name="autoIncrement"/> is <see langword="true"/>, the primary key property of the POCO will be updated with the new
        /// record's auto-incremented ID.
        /// </remarks>
        /// <param name="tableName">The name of the table where the record will be inserted.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="autoIncrement">Specifies whether the primary key column in the database is auto-incrementing.</param>
        /// <param name="poco">The POCO instance to insert.</param>
        /// <returns>The primary key of the new record if the table has a primary key column; otherwise, <see langword="null"/>.</returns>
        object Insert(string tableName, string primaryKeyName, bool autoIncrement, object poco);

        #endregion

        #region Update

        /// <summary>
        /// Updates a record and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="Update(string, string, object, object, IEnumerable{string})"/>
        int Update(object poco);

        /// <summary>
        /// Updates the specified columns of a record and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="Update(string, string, object, object, IEnumerable{string})"/>
        int Update(object poco, IEnumerable<string> columns);

        /// <summary>
        /// Updates a record with the given ID and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="Update(string, string, object, object, IEnumerable{string})"/>
        int Update(object poco, object primaryKeyValue);

        /// <summary>
        /// Updates the specified columns of a record with the given ID and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="Update(string, string, object, object, IEnumerable{string})"/>
        int Update(object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        /// Updates a record in the provided table and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="Update(string, string, object, object, IEnumerable{string})"/>
        int Update(string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Updates the specified columns of a record in the provided table and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="Update(string, string, object, object, IEnumerable{string})"/>
        int Update(string tableName, string primaryKeyName, object poco, IEnumerable<string> columns);

        /// <summary>
        /// Updates a record with the given ID in the provided table and returns the number of rows affected by the update operation.
        /// </summary>
        /// <inheritdoc cref="Update(string, string, object, object, IEnumerable{string})"/>
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        /// Updates the specified columns of a record with the given ID in the provided table and returns the number of rows affected by the
        /// update operation.
        /// </summary>
        /// <param name="tableName">The name of the table to update.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="poco">The POCO instance containing the column values to update.</param>
        /// <param name="primaryKeyValue">The primary key value identifying the record to update.</param>
        /// <param name="columns">A list of column names to update, or <see langword="null"/> to update all columns.</param>
        /// <returns>The number of rows affected by the update operation.</returns>
        int Update(string tableName, string primaryKeyName, object poco, object primaryKeyValue, IEnumerable<string> columns);

        /// <summary>
        /// Executes an SQL update and returns the number of rows affected by the update operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to update.</typeparam>
        /// <param name="sql">An SQL builder instance representing the condition portion of the WHERE clause identifying the row to update
        /// (everything after <c>UPDATE tablename</c>) and its parameters.</param>
        /// <returns>The number of rows affected by the update operation.</returns>
        int Update<T>(Sql sql);

        /// <summary>
        /// Executes an SQL update and returns the number of rows affected by the update operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to update.</typeparam>
        /// <param name="sql">The SQL string representing the condition portion of the WHERE clause identifying the row to update
        /// (everything after <c>UPDATE tablename</c>).</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>The number of rows affected by the update operation.</returns>
        int Update<T>(string sql, params object[] args);

        #endregion

        #region Delete

        /// <summary>
        /// Deletes a record and returns the number of rows affected by the update operation.
        /// </summary>
        /// <param name="poco">The POCO instance representing the record to delete.</param>
        /// <returns>The number of rows affected by the delete operation.</returns>
        int Delete(object poco);

        /// <summary>
        /// Deletes a record in the provided table and returns the number of rows affected by the update operation.
        /// </summary>
        /// <param name="tableName">The name of the table containing the record to delete.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="poco">The POCO instance representing the record to delete.</param>
        /// <returns>The number of rows affected by the delete operation.</returns>
        int Delete(string tableName, string primaryKeyName, object poco);

        /// <summary>
        /// Deletes a record with the given ID in the provided table and returns the number of rows affected by the update operation.
        /// </summary>
        /// <param name="tableName">The name of the table containing the record to delete.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="poco">The POCO instance representing the record to delete, or <see langword="null"/> to use the provided <paramref
        /// name="primaryKeyValue"/>.</param>
        /// <param name="primaryKeyValue">The primary key value identifying the record to delete, used if <paramref name="poco"/> is <see
        /// langword="null"/>.</param>
        /// <returns>The number of rows affected by the delete operation.</returns>
        int Delete(string tableName, string primaryKeyName, object poco, object primaryKeyValue);

        /// <summary>
        /// Deletes a record and returns the number of rows affected by the update operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to delete.</typeparam>
        /// <param name="pocoOrPrimaryKeyValue">The primary key value, or a POCO containing an assigned primary key value.</param>
        /// <returns>The number of rows affected by the delete operation.</returns>
        int Delete<T>(object pocoOrPrimaryKeyValue);

        /// <summary>
        /// Executes an SQL delete and returns the number of rows affected by the delete operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to delete.</typeparam>
        /// <param name="sql">An SQL builder instance representing the condition portion of the WHERE clause identifying the row to delete
        /// (everything after <c>DELETE FROM tablename</c>) and its parameters.</param>
        /// <returns>The number of rows affected by the delete operation.</returns>
        int Delete<T>(Sql sql);

        /// <summary>
        /// Executes an SQL delete and returns the number of rows affected by the delete operation.
        /// </summary>
        /// <typeparam name="T">The POCO type associated with the table to delete.</typeparam>
        /// <param name="sql">The SQL string representing the condition portion of the WHERE clause identifying the row to delete
        /// (everything after <c>DELETE FROM tablename</c>).</param>
        /// <param name="args">The parameters to embed in the SQL string.</param>
        /// <returns>The number of rows affected by the delete operation.</returns>
        int Delete<T>(string sql, params object[] args);

        #endregion

        #region IsNew

        /// <remarks>
        /// A POCO instance is considered "new" if the <paramref name="poco"/> property that maps to the associated table's primary key
        /// column contains a default value.
        /// </remarks>
        /// <inheritdoc cref="IsNew(string, object)"/>
        bool IsNew(object poco);

        /// <summary>
        /// Determines whether the specified POCO represents a new record that has not yet been saved to the database.
        /// </summary>
        /// <remarks>
        /// A POCO instance is considered "new" if the <paramref name="poco"/> property that maps to the associated table's provided column
        /// name contains a default value.
        /// </remarks>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="poco">The POCO instance to check.</param>
        /// <returns><see langword="true"/> if the POCO represents a new record; otherwise, <see langword="false"/>.</returns>
        bool IsNew(string primaryKeyName, object poco);

        #endregion

        #region Save

        /// <remarks>
        /// Performs an <see cref="Insert(object)"/> operation if the POCO is new (as determined by <see cref="IsNew(object)"/>), and an
        /// <see cref="Update(object)"/> operation otherwise.
        /// <para>If an Insert operation is performed, and a mapped primary key column is auto-incrementing, the primary key property of the
        /// POCO will be updated with the new record's auto-incremented ID.</para>
        /// </remarks>
        /// <inheritdoc cref="Save(string, string, object)"/>
        void Save(object poco);

        /// <summary>
        /// Saves the specified POCO to the database by performing either an insert or an update operation, as appropriate.
        /// </summary>
        /// <remarks>
        /// Performs an <see cref="Insert(string, string, object)"/> operation if the POCO is new (as determined by <see cref="IsNew(string,
        /// object)"/>), and an <see cref="Update(string, string, object)"/> operation otherwise.
        /// <para>If an Insert operation is performed, and <paramref name="primaryKeyName"/> represents an auto-incrementing column, the
        /// primary key property of the POCO will be updated with the new record's auto-incremented ID.</para>
        /// </remarks>
        /// <param name="tableName">The name of the table where the POCO will be saved.</param>
        /// <param name="primaryKeyName">The table's primary key column name.</param>
        /// <param name="poco">The POCO instance to save.</param>
        void Save(string tableName, string primaryKeyName, object poco);

        #endregion
    }
}
