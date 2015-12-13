// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;
using System.Linq;
using System.Reflection;

namespace PetaPoco.Tests.Integration.Databases.Sqlite
{
    public class SqliteDBTestProvider : DBTestProvider
    {
        protected override Database Database => new Database("sqlite");

        protected override string ScriptResourceName => "PetaPoco.Tests.Integration.Scripts.SqliteBuildDatabase.sql";

        public override Database Execute()
        {
            Mappers.Register(GetType().Assembly, new SqliteMapper());
            return base.Execute();
        }

        public override void Dispose()
        {
            Mappers.RevokeAll();
            base.Dispose();
        }

        public class SqliteMapper : StandardMapper
        {
            public override Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType)
            {
                if (sourceType == typeof(long))
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

                return base.GetFromDbConverter(targetProperty, sourceType);
            }

            public override Func<object, object> GetToDbConverter(PropertyInfo sourceProperty)
            {
                var type = !sourceProperty.PropertyType.IsNullableType()
                    ? sourceProperty.PropertyType
                    : sourceProperty.PropertyType.GetGenericArguments().First();

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.DateTime:
                        return o => ((DateTime) o).Ticks;
                }

                return base.GetToDbConverter(sourceProperty);
            }
        }
    }
}