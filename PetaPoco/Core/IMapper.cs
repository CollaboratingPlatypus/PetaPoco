using System;
using System.Reflection;

namespace PetaPoco
{
    /// <summary>
    ///     IMapper provides a way to hook into PetaPoco's Database to POCO mapping mechanism to either
    ///     customize or completely replace it.
    /// </summary>
    /// <remarks>
    ///     To use this functionality, instantiate a class that implements IMapper and then pass it to
    ///     PetaPoco through the static method Mappers.Register()
    /// </remarks>
    public interface IMapper
    {
        /// <summary>
        ///     Get information about the table associated with a POCO class
        /// </summary>
        /// <param name="pocoType">The poco type.</param>
        /// <returns>A TableInfo instance</returns>
        /// <remarks>
        ///     This method must return a valid TableInfo.
        ///     To create a TableInfo from a POCO's attributes, use TableInfo.FromPoco
        /// </remarks>
        TableInfo GetTableInfo(Type pocoType);

        /// <summary>
        ///     Get information about the column associated with a property of a POCO
        /// </summary>
        /// <param name="pocoProperty">The PropertyInfo of the property being queried</param>
        /// <returns>A reference to a ColumnInfo instance, or null to ignore this property</returns>
        /// <remarks>
        ///     To create a ColumnInfo from a property's attributes, use PropertyInfo.FromProperty
        /// </remarks>
        ColumnInfo GetColumnInfo(PropertyInfo pocoProperty);

        /// <summary>
        ///     Supply a function to convert a database value to the correct property value
        /// </summary>
        /// <param name="targetProperty">The target property</param>
        /// <param name="sourceType">The type of data returned by the DB</param>
        /// <returns>A Func that can do the conversion, or null for no conversion</returns>
        Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType);

        /// <summary>
        ///     Supply a function to convert a property value into a database value
        /// </summary>
        /// <param name="sourceProperty">The property to be converted</param>
        /// <returns>A Func that can do the conversion</returns>
        /// <remarks>
        ///     This conversion is only used for converting values from POCO's that are
        ///     being Inserted or Updated.
        ///     Conversion is not available for parameter values passed directly to queries.
        /// </remarks>
        Func<object, object> GetToDbConverter(PropertyInfo sourceProperty);
    }
}