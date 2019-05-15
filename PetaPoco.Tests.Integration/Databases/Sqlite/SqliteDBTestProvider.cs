using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    public class SqliteDBTestProvider : DBTestProvider
    {
        private string _connectionName = "sqlite";
        protected override IDatabase Database => LoadFromConnectionName(_connectionName);

        public override string ProviderName => GetProviderName(_connectionName);

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.SqliteBuildDatabase.sql";

        public IDatabase GetDatabase()
        {
            return Database;
        }

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
                                return o => new DateTime((long) o, DateTimeKind.Utc);
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
                            return o => ((DateTime?) o)?.Ticks;
                    }

                    return null;
                };
            });
        }
    }
}