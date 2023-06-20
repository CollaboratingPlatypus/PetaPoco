using System;
using System.Linq;
using System.Reflection;

namespace PetaPoco
{
    /// <summary>
    /// A class used by <see cref="IMapper"/> to override table bindings for a POCO object.
    /// </summary>
    public class TableInfo
    {
        /// <summary>
        /// Gets or sets the database table name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the name of the primary key column of the table.
        /// </summary>
        public string PrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets whether the primary key column is an auto-incrementing.
        /// </summary>
        public bool AutoIncrement { get; set; }

        /// <summary>
        /// Gets or sets the name of the sequence used for auto-incrementing Oracle primary key fields.
        /// </summary>
        public string SequenceName { get; set; }

        /// <summary>
        /// Constructs and initializes a TableInfo instance from the attributes of a POCO.
        /// </summary>
        /// <param name="pocoType">The POCO type.</param>
        /// <returns>The TableInfo instance.</returns>
        public static TableInfo FromPoco(Type pocoType)
        {
            var ti = new TableInfo();
            PopulateTableNameFromPoco(pocoType, ref ti, out _);
            PopulatePrimaryKeyFromPoco(pocoType, ref ti, out _, out _);
            return ti;
        }


        internal static void PopulateTableNameFromPoco(Type t, ref TableInfo ti, out TableNameAttribute tblAttr)
        {
            ti = ti ?? new TableInfo();
            tblAttr = t.GetCustomAttributes(typeof(TableNameAttribute), true).FirstOrDefault() as TableNameAttribute;
            ti.TableName = tblAttr?.Value ?? t.Name;
        }

        internal static void PopulatePrimaryKeyFromPoco(Type t, ref TableInfo ti, out PrimaryKeyAttribute pkAttr, out PropertyInfo idProp)
        {
            ti = ti ?? new TableInfo();
            pkAttr = t.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).FirstOrDefault() as PrimaryKeyAttribute;
            idProp = null;

            ti.PrimaryKey = pkAttr?.Value;
            ti.SequenceName = pkAttr?.SequenceName;
            ti.AutoIncrement = pkAttr?.AutoIncrement ?? false;

            if (String.IsNullOrWhiteSpace(ti.PrimaryKey))
            {
                bool isIdProp(PropertyInfo p)
                {
                    bool hasName(string name) => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
                    return hasName("id")
                        || hasName(t.Name + "id")
                        || hasName(t.Name + "_id");
                }

                idProp = t.GetProperties().FirstOrDefault(isIdProp) as PropertyInfo;
                if (idProp != null)
                {
                    ti.PrimaryKey = idProp.Name;
                    ti.AutoIncrement = idProp.PropertyType.IsValueType;
                }
            }
        }
    }
}
