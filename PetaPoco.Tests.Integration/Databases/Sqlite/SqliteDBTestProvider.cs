using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    public class SqliteDBTestProvider : DBTestProvider
    {
        protected override string ConnectionName => "sqlite";

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.SqliteBuildDatabase.sql";

        public IDatabase GetDatabase() => Database;

        protected override IDatabaseBuildConfiguration BuildFromConnectionName(string name)
        {
            return base.BuildFromConnectionName(name).UsingDefaultMapper<ConventionMapper>(m =>
            {
                m.FromDbConverter = (targetProperty, sourceType) =>
                {
                    if (targetProperty != null && sourceType == typeof(long))
                    {
                        var type = !targetProperty.PropertyType.IsNullableType() ? targetProperty.PropertyType : targetProperty.PropertyType.GetGenericArguments().First();

                        switch (Type.GetTypeCode(type))
                        {
                            case TypeCode.DateTime:
                                return o => new DateTime((long)o, DateTimeKind.Utc);
                            default:
                                return o => Convert.ChangeType(o, Type.GetTypeCode(type));
                        }
                    }

                    return null;
                };
                m.ToDbConverter = sourceProperty =>
                {
                    var type = !sourceProperty.PropertyType.IsNullableType() ? sourceProperty.PropertyType : sourceProperty.PropertyType.GetGenericArguments().First();

                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.DateTime:
                            return o => ((DateTime?)o)?.Ticks;
                    }

                    return null;
                };
            });
        }
    }
}
