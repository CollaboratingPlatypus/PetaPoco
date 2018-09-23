// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2016/07/30</date>

using System.Reflection;

namespace PetaPoco
{
    /// <summary>
    ///     Holds information about a column in the database.
    /// </summary>
    /// <remarks>
    ///     Typically ColumnInfo is automatically populated from the attributes on a POCO object and its properties. It can
    ///     however also be returned from the IMapper interface to provide your own bindings between the DB and your POCOs.
    /// </remarks>
    public class ColumnInfo
    {
        /// <summary>
        ///     The SQL name of the column
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        ///     True if this column returns a calculated value from the database and shouldn't be used in Insert and Update
        ///     operations.
        /// </summary>
        public bool ResultColumn { get; set; }

        /// <summary>
        /// True if this is a result column but should be included in auto select queries.
        /// </summary>
        public bool AutoSelectedResultColumn { get; set; }

        /// <summary>
        ///     True if time and date values returned through this column should be forced to UTC DateTimeKind. (no conversion is
        ///     applied - the Kind of the DateTime property
        ///     is simply set to DateTimeKind.Utc instead of DateTimeKind.Unknown.
        /// </summary>
        public bool ForceToUtc { get; set; }

        /// <summary>
        ///     The insert template. If not null, this template is used for generating the insert section instead of the deafult
        ///     string.Format("{0}{1}", paramPrefix, index"). Setting this allows DB related interactions, such as "CAST({0}{1} AS
        ///     json)"
        /// </summary>
        public string InsertTemplate { get; set; }

        /// <summary>
        ///     The update template. If not null, this template is used for generating the update section instead of the deafult
        ///     string.Format("{0} = {1}{2}", colName, paramPrefix, index"). Setting this allows DB related interactions, such as "{0} = CAST({1}{2} AS
        ///     json)"
        /// </summary>
        public string UpdateTemplate { get; set; }

        /// <summary>
        ///     Creates and populates a ColumnInfo from the attributes of a POCO property.
        /// </summary>
        /// <param name="propertyInfo">The property whose column info is required</param>
        /// <returns>A ColumnInfo instance</returns>
        public static ColumnInfo FromProperty(PropertyInfo propertyInfo)
        {
            // Check if declaring poco has [Explicit] attribute
            var explicitColumns =
                propertyInfo.DeclaringType.GetCustomAttributes(typeof(ExplicitColumnsAttribute), true).Length > 0;

            // Check for [Column]/[Ignore] Attributes
            var colAttrs = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
            if (explicitColumns)
            {
                if (colAttrs.Length == 0)
                    return null;
            }
            else
            {
                if (propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), true).Length != 0)
                    return null;
            }

            var ci = new ColumnInfo();

            // Read attribute
            if (colAttrs.Length > 0)
            {
                var colattr = (ColumnAttribute) colAttrs[0];
                ci.InsertTemplate = colattr.InsertTemplate;
                ci.UpdateTemplate = colattr.UpdateTemplate;
                ci.ColumnName = colattr.Name == null ? propertyInfo.Name : colattr.Name;
                ci.ForceToUtc = colattr.ForceToUtc;
                if ((colattr as ResultColumnAttribute) != null)
                    ci.ResultColumn = true;
            }
            else
            {
                ci.ColumnName = propertyInfo.Name;
                ci.ForceToUtc = false;
                ci.ResultColumn = false;
            }

            return ci;
        }
    }
}