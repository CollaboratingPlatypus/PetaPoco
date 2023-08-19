using System;

namespace PetaPoco
{
    /// <summary>
    /// The ExplicitColumnsAttribute class defines an attribute for POCO classes specifying that only explicitly-marked properties should be
    /// mapped to columns in the database table.
    /// </summary>
    /// <remarks>
    /// When using this attribute, any properties <i>not</i> decorated with one of the following attributes are ignored by the mapper:
    /// <list type="bullet">
    /// <item><see cref="ColumnAttribute"/></item>
    /// <item><see cref="ResultColumnAttribute"/></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IgnoreAttribute"/>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExplicitColumnsAttribute : Attribute
    {
    }
}
