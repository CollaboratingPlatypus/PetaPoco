using System;

namespace PetaPoco
{
    /// <summary>
    ///     Represents an attribute which can decorate a Poco property to ensure PetaPoco does not map column, and therefore
    ///     ignores the column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}