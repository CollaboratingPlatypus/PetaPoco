// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/07</date>

using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    [RequiresCleanUpAttribute]
    public class MapperTests
    {
        [Fact]
        public void NoColumnMapper()
        {
            Mappers.Register(Assembly.GetExecutingAssembly(), new MyColumnMapper());
            var pd = Internal.PocoData.ForType(typeof(Poco2));

            pd.Columns.Count.ShouldBe(3);
            pd.Columns["prop1"].PropertyInfo.Name.ShouldBe("prop1");
            pd.Columns["remapped2"].ColumnName.ShouldBe("remapped2");
            pd.Columns["prop3"].ColumnName.ShouldBe("prop3");
            string.Join(", ", pd.QueryColumns).ShouldBe("prop1, remapped2");
            pd.TableInfo.PrimaryKey.ShouldBe("id");
            pd.TableInfo.TableName.ShouldBe("petapoco");

            PetaPoco.Mappers.Revoke(Assembly.GetExecutingAssembly());
        }

        public class Poco2
        {
            public string prop1 { get; set; }

            public string prop2 { get; set; }

            public string prop3 { get; set; }

            public string prop4 { get; set; }
        }

        public class MyColumnMapper : IMapper
        {
            public TableInfo GetTableInfo(Type t)
            {
                var ti = TableInfo.FromPoco(t);

                if (t == typeof(Poco2))
                {
                    ti.TableName = "petapoco";
                    ti.PrimaryKey = "id";
                }

                return ti;
            }

            public ColumnInfo GetColumnInfo(PropertyInfo pi)
            {
                var ci = ColumnInfo.FromProperty(pi);
                if (ci == null)
                    return null;

                if (pi.DeclaringType == typeof(Poco2))
                {
                    switch (pi.Name)
                    {
                        case "prop1":
                            // Leave this property as is
                            break;
                        case "prop2":
                            // Rename this column
                            ci.ColumnName = "remapped2";
                            break;
                        case "prop3":
                            // Mark this as a result column
                            ci.ResultColumn = true;
                            break;
                        case "prop4":
                            // Ignore this property
                            return null;
                    }
                }
                // Do default property mapping
                return ci;
            }

            public Func<object, object> GetFromDbConverter(PropertyInfo targetProperty, Type sourceType)
            {
                return null;
            }

            public Func<object, object> GetToDbConverter(PropertyInfo sourceProperty)
            {
                return null;
            }
        }
    }
}