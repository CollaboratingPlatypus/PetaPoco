using System;
using System.Reflection;

namespace PetaPoco
{
    /// <summary>
    /// This is the original default implementation of <see cref="IMapper"/> used by PetaPoco.
    /// </summary>
    /// <remarks>
    /// PetaPoco now ships with an improved <see cref="ConventionMapper"/> as the default mapper.
    /// </remarks>
    public class StandardMapper : IMapper
    {
        /// <inheritdoc/>
        public virtual TableInfo GetTableInfo(Type pocoType) => TableInfo.FromPoco(pocoType);

        /// <inheritdoc/>
        public virtual ColumnInfo GetColumnInfo(PropertyInfo pocoProperty) => ColumnInfo.FromProperty(pocoProperty);

        /// <inheritdoc/>
        public virtual Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType) => null;

        /// <inheritdoc/>
        public virtual Func<object, object> GetToDbConverter(PropertyInfo sourceProperty) => null;
    }
}
