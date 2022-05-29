using System;
using PetaPoco.Tests.Unit.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    public class ConventionMapperTests
    {
        private ConventionMapper _mapper;

        public ConventionMapperTests()
        {
            _mapper = new ConventionMapper();
        }

        [Fact]
        public void GetFromDbConverter_GivenPropertyAndType_ShouldBeNull()
        {
            var func = _mapper.GetFromDbConverter(typeof(Order).GetProperty(nameof(Order.OrderId)), typeof(int));
            func.ShouldBeNull();
        }

        [Fact]
        public void GetToDbConverter_GivenProperty_ShouldBeNull()
        {
            var func = _mapper.GetToDbConverter(typeof(Order).GetProperty(nameof(Order.OrderId)));
            func.ShouldBeNull();
        }

        [Fact]
        public void GetFromDbConverter_GivenPropertyWithValueConverterAttribute_ShouldNotBeNull()
        {
            var func = _mapper.GetFromDbConverter(typeof(Order).GetProperty(nameof(Order.PO)), typeof(string));
            func.ShouldNotBeNull();
        }

        [Fact]
        public void GetToDbConverter_GivenPropertyWithValueConverterAttribute_ShouldNotBeNull()
        {
            var func = _mapper.GetToDbConverter(typeof(Order).GetProperty(nameof(Order.PO)));
            func.ShouldNotBeNull();
        }

        [Fact]
        public void GetFromDbConverter_GivenPropertyTypeAndInterceptSet_ShouldCallback()
        {
            var wasCalled = false;
            _mapper.FromDbConverter = (info, type) =>
            {
                wasCalled = true;
                return null;
            };

            _mapper.GetFromDbConverter(typeof(Order).GetProperty(nameof(Order.OrderId)), typeof(int));

            wasCalled.ShouldBeTrue();
        }

        [Fact]
        public void GetToDbConverter_GivenPropertyAndInterceptSet_ShouldCallback()
        {
            var wasCalled = false;
            _mapper.ToDbConverter = (info) =>
            {
                wasCalled = true;
                return null;
            };

            _mapper.GetToDbConverter(typeof(Order).GetProperty(nameof(Order.OrderId)));

            wasCalled.ShouldBeTrue();
        }

        [Fact]
        public void GetTableInfo_UsingDefaults_ShouldBeValid()
        {
            var ti1 = _mapper.GetTableInfo(typeof(Order));
            var ti2 = _mapper.GetTableInfo(typeof(OrderLine));
            var ti3 = _mapper.GetTableInfo(typeof(Product));
            var ti4 = _mapper.GetTableInfo(typeof(Box));

            ti1.TableName.ShouldBe("Order");
            ti1.PrimaryKey.ShouldBe("OrderId");
            ti1.AutoIncrement.ShouldBeTrue();
            ti1.SequenceName.ShouldBeNull();

            ti2.TableName.ShouldBe("OrderLine");
            ti2.PrimaryKey.ShouldBe("OrderLine_Id");
            ti2.AutoIncrement.ShouldBeTrue();
            ti2.SequenceName.ShouldBeNull();

            ti3.TableName.ShouldBe("Product");
            ti3.PrimaryKey.ShouldBe("Id");
            ti3.AutoIncrement.ShouldBeTrue();
            ti3.SequenceName.ShouldBeNull();

            ti4.TableName.ShouldBe("Box");
            ti4.PrimaryKey.ShouldBe("Id");
            ti4.AutoIncrement.ShouldBeFalse();
            ti4.SequenceName.ShouldBeNull();
        }

        [Fact]
        public void GetColumnInfo_UsingDefaults_ShouldBeValid()
        {
            var ci1 = _mapper.GetColumnInfo(typeof(Order).GetProperty(nameof(Order.OrderId)));
            var ci2 = _mapper.GetColumnInfo(typeof(Order).GetProperty(nameof(Order.CreatedOn)));
            var ci3 = _mapper.GetColumnInfo(typeof(Order).GetProperty(nameof(Order.PO)));
            var ci4 = _mapper.GetColumnInfo(typeof(Order).GetProperty(nameof(Order.Discount)));

            ci1.ColumnName.ShouldBe("OrderId");
            ci1.ForceToUtc.ShouldBeFalse();
            ci1.ResultColumn.ShouldBeFalse();

            ci2.ColumnName.ShouldBe("CreatedOn");
            ci2.ForceToUtc.ShouldBeFalse();
            ci2.ResultColumn.ShouldBeFalse();

            ci3.ColumnName.ShouldBe("PO");
            ci3.ForceToUtc.ShouldBeFalse();
            ci3.ResultColumn.ShouldBeFalse();

            ci4.ColumnName.ShouldBe("Discount");
            ci4.ForceToUtc.ShouldBeFalse();
            ci4.ResultColumn.ShouldBeFalse();
        }

        [Fact]
        public void GetTableInfo_GivenEntityWithTableName_ShouldBeValid()
        {
            var ti = _mapper.GetTableInfo(typeof(EntityWithAttributes));
            ti.TableName.ShouldBe("Test1");
            ti.PrimaryKey.ShouldBe("ThatId");
            ti.AutoIncrement.ShouldBeTrue();
            ti.SequenceName.ShouldBeNull();
        }

        [Fact]
        public void GetColumnInfo_GivenColumnWithResultAttribute_ShouldBeValid()
        {
            var ci = _mapper.GetColumnInfo(typeof(EntityWithAttributes).GetProperty(nameof(EntityWithAttributes.Result)));
            ci.ColumnName.ShouldBe("Result");
            ci.ResultColumn.ShouldBeTrue();
            ci.ForceToUtc.ShouldBeFalse();
        }

        [Fact]
        public void GetColumnInfo_GivenColumnWithColumnAttribute_ShouldBeValid()
        {
            var ci = _mapper.GetColumnInfo(typeof(EntityWithAttributes).GetProperty(nameof(EntityWithAttributes.ColumnOne)));
            ci.ColumnName.ShouldBe("Col1");
            ci.ResultColumn.ShouldBeFalse();
            ci.ForceToUtc.ShouldBeFalse();

            ci = _mapper.GetColumnInfo(typeof(EntityWithAttributes).GetProperty(nameof(EntityWithAttributes.ColumnTwo)));
            ci.InsertTemplate.ShouldBe("test");
            ci.UpdateTemplate.ShouldBe("test1");
        }

        [Fact]
        public void GetColumnInfo_GivenColumnWithIgnoreAttribute_ShouldBeNull()
        {
            var ci = _mapper.GetColumnInfo(typeof(EntityWithAttributes).GetProperty(nameof(EntityWithAttributes.NonPersistedColumn)));
            ci.ShouldBeNull();
        }

        [Fact]
        public void GetColumnInfo_GivenColumnWithNoMapping_ShouldBeNull()
        {
            var ci = _mapper.GetColumnInfo(typeof(EntityWithAttributes).GetProperty(nameof(EntityWithAttributes.UnmappedColumn)));
            ci.ShouldBeNull();
        }

        [Fact]
        public void GetColumnInfo_GivenColumnAndInflectionInterceptSet_ShouldBeValid()
        {
            _mapper.InflectColumnName = (inflector, cn) => inflector.Underscore(cn).ToLowerInvariant();
            var ci = _mapper.GetColumnInfo(typeof(Order).GetProperty(nameof(Order.CreatedOn)));

            ci.ColumnName.ShouldBe("created_on");
            ci.ForceToUtc.ShouldBeFalse();
            ci.ResultColumn.ShouldBeFalse();
        }

        [Fact]
        public void GetTableInfo_GivenEntityAndInflectionInterceptSet_ShouldBeValid()
        {
            _mapper.InflectTableName = (inflector, tn) => inflector.Underscore(tn).ToLowerInvariant();
            var ti = _mapper.GetTableInfo(typeof(OrderLine));

            ti.TableName.ShouldBe("order_line");
            ti.PrimaryKey.ShouldBe("OrderLine_Id");
            ti.AutoIncrement.ShouldBeTrue();
            ti.SequenceName.ShouldBeNull();
        }

        [TableName("Test1")]
        [PrimaryKey("ThatId", AutoIncrement = true)]
        [ExplicitColumns]
        public class EntityWithAttributes
        {
            [Column]
            public int ThatId { get; set; }

            [ResultColumn]
            public string Result { get; set; }

            [Column("Col1")]
            public string ColumnOne { get; set; }

            [Ignore]
            public int NonPersistedColumn { get; set; }

            public DateTime UnmappedColumn { get; set; }

            [Column(InsertTemplate = "test", UpdateTemplate = "test1")]
            public string ColumnTwo { get; set; }
        }

        public class EmptyValueConverterAttribute : ValueConverterAttribute
        {
            public override object ConvertFromDb(object value)
            {
                return value;
            }

            public override object ConvertToDb(object value)
            {
                return value;
            }
        }

        public class Order
        {
            public long OrderId { get; set; }

            public DateTime CreatedOn { get; set; }

            [EmptyValueConverter]
            public string PO { get; set; }

            public decimal? Discount { get; set; }
        }

        public class OrderLine
        {
            public int OrderLine_Id { get; set; }

            public long OrderId { get; set; }

            public short Quantity { get; set; }

            public decimal Cost { get; set; }

            public string Description { get; set; }
        }

        public class Product
        {
            public ushort Id { get; set; }

            public int BoxId { get; set; }

            public decimal ListPrice { get; set; }
        }

        public class Box
        {
            public Guid Id { get; set; }

            public int Size { get; set; }
        }

        [Theory]
        [InlineData(typeof(Child1))]
        [InlineData(typeof(Child2))]
        [InlineData(typeof(Child3))]
        [InlineData(typeof(Child4))]
        public void Converters_Should_Inherit(Type type)
        {
            var func = _mapper.GetToDbConverter(type.GetProperty("ID"));
            func.ShouldNotBeNull();
        }
    }
}