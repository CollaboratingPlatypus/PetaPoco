using System;
using System.Reflection;

namespace PetaPoco
{
    /// <summary>
    /// Provides a way to hook into PetaPoco's DB-to-POCO mapping mechanism to either customise or completely replace it.
    /// </summary>
    /// <remarks>
    /// To use this functionality, instantiate a class that implements IMapper and then register it using one of the <see cref="Mappers" /> static register methods.
    /// </remarks>
    /// <seealso cref="Mappers.Register(Assembly, IMapper)"/>
    /// <seealso cref="Mappers.Register(Type, IMapper)"/>
    public interface IMapper
    {
        /// <summary>
        /// Returns information about the table associated with a POCO class.
        /// </summary>
        /// <remarks>
        /// This method must return a valid <see cref="TableInfo"/>. To create a TableInfo from a POCO's attributes, use <see cref="TableInfo.FromPoco"/>.
        /// </remarks>
        /// <param name="pocoType">The POCO type.</param>
        /// <returns>A TableInfo instance.</returns>
        TableInfo GetTableInfo(Type pocoType);

        /// <summary>
        /// Returns a <see cref="ColumnInfo"/> object containing information about the column associated with a property of a POCO.
        /// </summary>
        /// <remarks>
        /// To create a ColumnInfo from a property's attributes, use <see cref="ColumnInfo.FromProperty"/>
        /// </remarks>
        /// <param name="pocoProperty">The PropertyInfo of the property being queried.</param>
        /// <returns>A ColumnInfo instance, or <see langword="null"/> if the property should be ignored.</returns>
        ColumnInfo GetColumnInfo(PropertyInfo pocoProperty);

        /// <summary>
        /// Supplies a function to convert a database value to the correct property value.
        /// </summary>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="sourceType">The type of data returned by the DB.</param>
        /// <returns>A Func that can do the conversion, or <see langword="null"/> for no conversion.</returns>
        Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType);

        /// <summary>
        /// Supplies a function to convert a property value to the correct database value.
        /// </summary>
        /// <remarks>
        /// This conversion is only used for converting values from POCOs that are being Inserted or Updated. Conversion is not available for parameter values passed directly to queries.
        /// </remarks>
        /// <param name="sourceProperty">The property to be converted.</param>
        /// <returns>A Func that can do the conversion.</returns>
        Func<object, object> GetToDbConverter(PropertyInfo sourceProperty);
    }
}
