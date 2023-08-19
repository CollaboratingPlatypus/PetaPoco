using System;

namespace PetaPoco
{
    /// <summary>
    /// The IgnoreAttribute class defines an attribute for POCO properties that should be explicitly ignored by the mapper.
    /// </summary>
    /// <remarks>
    /// Properties decorated with this attribute are completely ignored by PetaPoco, and do not participate in any database-related
    /// operations.
    /// <para>If you find yourself using this attribute excessively, consider instead decorating your POCO class with the <see
    /// cref="ExplicitColumnsAttribute"/>, and then marking the properties you want mapped with their appropriate column attribute.</para>
    /// </remarks>
    /// <seealso cref="ExplicitColumnsAttribute"/>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}
