// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/28</date>

using System;
using System.Linq;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    public class SqliteDBTestProvider : DBTestProvider
    {
        public IDatabase GetDatabase()
        {
            return Database;
        }

        protected override IDatabase Database => DatabaseConfiguration.Build().UsingConnectionStringName("sqlite").UsingDefaultMapper<ConventionMapper>(m =>
        {
            m.FromDbConverter = (targetProperty, sourceType) =>
            {
                if (targetProperty != null && sourceType == typeof(long))
                {
                    var type = !targetProperty.PropertyType.IsNullableType()
                        ? targetProperty.PropertyType
                        : targetProperty.PropertyType.GetGenericArguments().First();

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
                var type = !sourceProperty.PropertyType.IsNullableType()
                    ? sourceProperty.PropertyType
                    : sourceProperty.PropertyType.GetGenericArguments().First();

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.DateTime:
                        return o => ((DateTime?) o)?.Ticks;
                }

                return null;
            };
        }).Create();

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.SqliteBuildDatabase.sql";
    }
}