using System;
using System.Linq;
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
        ///     True if this is a result column but should be included in auto select queries.
        /// </summary>
        public bool AutoSelectedResultColumn { get; set; }

        /// <summary>
        ///     True if time and date values returned through this column should be forced to UTC DateTimeKind. (no conversion is
        ///     applied - the Kind of the DateTime property
        ///     is simply set to DateTimeKind.Utc instead of DateTimeKind.Unknown.
        /// </summary>
        public bool ForceToUtc { get; set; }
        /// <summary>
        ///     True if Database Column is DbType.AnsiString (like VARCHAR)
        /// </summary>
        public bool ForceToAnsiString { get; set; }
        /// <summary>
        ///     True if Database Colume is DbType.DateTime2
        /// </summary>
        public bool ForceToDateTime2 { get; set; }

        /// <summary>
        ///     The insert template. If not null, this template is used for generating the insert section instead of the deafult
        ///     string.Format("{0}{1}", paramPrefix, index"). Setting this allows DB related interactions, such as "CAST({0}{1} AS
        ///     json)"
        /// </summary>
        public string InsertTemplate { get; set; }

        /// <summary>
        ///     The update template. If not null, this template is used for generating the update section instead of the deafult
        ///     string.Format("{0} = {1}{2}", colName, paramPrefix, index"). Setting this allows DB related interactions, such as
        ///     "{0} = CAST({1}{2} AS
        ///     json)"
        /// </summary>
        public string UpdateTemplate { get; set; }

        internal static void PopulateFromProperty(PropertyInfo pi, ref ColumnInfo ci, out ColumnAttribute columnAttr)
        {
            // Check if declaring poco has [Explicit] attribute
            var isExplicit = pi.DeclaringType.GetCustomAttributes(typeof(ExplicitColumnsAttribute), true).Any();

            // Check for [Column]/[Ignore] Attributes
            columnAttr = Attribute.GetCustomAttributes(pi, typeof(ColumnAttribute)).FirstOrDefault() as ColumnAttribute;
            var isIgnore = Attribute.GetCustomAttributes(pi, typeof(IgnoreAttribute)).Any();

            if (isIgnore || (isExplicit && columnAttr == null))
            {
                ci = null;
            }
            else
            {
                ci = ci ?? new ColumnInfo();

                ci.ColumnName = columnAttr?.Name ?? pi.Name;
                ci.ForceToUtc = columnAttr?.ForceToUtc == true;
                ci.ForceToAnsiString = columnAttr?.ForceToAnsiString == true;
                ci.ForceToDateTime2 = columnAttr?.ForceToDateTime2 == true;
                ci.InsertTemplate = columnAttr?.InsertTemplate;
                ci.UpdateTemplate = columnAttr?.UpdateTemplate;

                if (columnAttr is ResultColumnAttribute resAttr)
                {
                    ci.ResultColumn = true;
                    ci.AutoSelectedResultColumn = resAttr.IncludeInAutoSelect == IncludeInAutoSelect.Yes;
                }                
            }
        }

        /// <summary>
        ///     Creates and populates a ColumnInfo from the attributes of a POCO property.
        /// </summary>
        /// <param name="propertyInfo">The property whose column info is required</param>
        /// <returns>A ColumnInfo instance</returns>
        public static ColumnInfo FromProperty(PropertyInfo propertyInfo)
        {
            var ci = new ColumnInfo();
            PopulateFromProperty(propertyInfo, ref ci, out _);
            return ci;
        }
    }
}