using System;

namespace PetaPoco
{
    /// <summary>
    /// The ValueConverterAttribute class provides a base class all ValueConverters must derive from and implement.
    /// </summary>
    /// <remarks>
    /// ValueConverters are used to implement custom two-way conversions between your POCO property data type, and the mapped database column's data type. They are ideal for implementing a custom conversion without requiring any changes to the mapper.
    /// <para>To provide a custom ValueConverter for a property, inherit from this class, and supply definitions for both conversion methods for your data type. Decorate the appropriate properties that require your ValueConverter with your derived class.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ValueConverterAttribute : Attribute
    {
        /// <summary>
        /// Converts the given <paramref name="value"/> from the database type to your POCO's property type.
        /// </summary>
        /// <param name="value">The database value to be converted.</param>
        /// <returns>The converted property value.</returns>
        public abstract object ConvertFromDb(object value);

        /// <summary>
        /// Converts the given <paramref name="value"/> from your POCO's property type to the database type.
        /// </summary>
        /// <param name="value">The property value to be converted.</param>
        /// <returns>The converted database value.</returns>
        public abstract object ConvertToDb(object value);
    }
}
