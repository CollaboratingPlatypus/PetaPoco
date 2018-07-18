// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using System;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    public class StandardMapperTests
    {
        private readonly IMapper _mapper;

        public StandardMapperTests()
            : this(null)
        {
        }

        protected StandardMapperTests(IMapper mapper)
        {
            _mapper = mapper ?? new StandardMapper();
        }

        [Fact]
        public void GetTableInfo_GivenEntityWithoutTableAttribute_ShouldBeValid()
        {
            var exaEntity = _mapper.GetTableInfo(typeof(ExaEntity));
            var zettaEntity = _mapper.GetTableInfo(typeof(ZettaEntity));

            exaEntity.TableName.ShouldBe("ExaEntity");
            exaEntity.PrimaryKey.ShouldBe("Id");
            exaEntity.AutoIncrement.ShouldBeTrue();
            exaEntity.SequenceName.ShouldBeNull();

            zettaEntity.TableName.ShouldBe("ZettaEntity");
            zettaEntity.PrimaryKey.ShouldBeNull();
            zettaEntity.AutoIncrement.ShouldBeFalse();
            zettaEntity.SequenceName.ShouldBeNull();
        }

        [Fact]
        public void GetTableInfo_GivenEntityWithTableAttribute_ShouldBeValid()
        {
            var teraEntity = _mapper.GetTableInfo(typeof(TeraEntity));
            var yottaEntity = _mapper.GetTableInfo(typeof(YottaEntity));

            teraEntity.TableName.ShouldBe("TblTeraEntity");
            teraEntity.PrimaryKey.ShouldBe("TheId");
            teraEntity.AutoIncrement.ShouldBeFalse();
            teraEntity.SequenceName.ShouldBeNull();

            yottaEntity.TableName.ShouldBe("yotta_entities");
            yottaEntity.PrimaryKey.ShouldBe("Id");
            yottaEntity.AutoIncrement.ShouldBeTrue();
            yottaEntity.SequenceName.ShouldBe("SEQ_1");
        }

        [Fact]
        public void GetColumnInfo_GivenPropertyWithoutColumnAttribute_ShouldBeValid()
        {
            var columnInfo1 = _mapper.GetColumnInfo(typeof(ExaEntity).GetProperty(nameof(ExaEntity.Id)));
            var columnInfo2 = _mapper.GetColumnInfo(typeof(ExaEntity).GetProperty(nameof(ExaEntity.AnotherColumn)));

            columnInfo1.ColumnName.ShouldBe("Id");
            columnInfo1.ForceToUtc.ShouldBeFalse();
            columnInfo1.ResultColumn.ShouldBeFalse();

            columnInfo2.ColumnName.ShouldBe("AnotherColumn");
            columnInfo2.ForceToUtc.ShouldBeFalse();
            columnInfo2.ResultColumn.ShouldBeFalse();
        }

        [Fact]
        public void GetColumnInfo_GivenPropertyWithoutColumnAttributeAndEntityWithExplicitAttribute_ShouldBeNull()
        {
            var columnInfo = _mapper.GetColumnInfo(typeof(TeraEntity).GetProperty(nameof(TeraEntity.NotAColumn)));
            columnInfo.ShouldBeNull();
        }

        [Fact]
        public void GetColumnInfo_GivenPropertyWithIgnoreColumnAttributeAndEntityWithoutExplicitAttribute_ShouldBeNull()
        {
            var columnInfo = _mapper.GetColumnInfo(typeof(YottaEntity).GetProperty(nameof(YottaEntity.NotAColumn)));
            columnInfo.ShouldBeNull();
        }

        [Fact]
        public void GetColumnInfo_GivenPropertyWithColumnAttribute_ShouldBeValid()
        {
            var columnInfo1 = _mapper.GetColumnInfo(typeof(TeraEntity).GetProperty(nameof(TeraEntity.TheId)));
            var columnInfo2 = _mapper.GetColumnInfo(typeof(TeraEntity).GetProperty(nameof(TeraEntity.AnotherColumn)));
            var columnInfo3 = _mapper.GetColumnInfo(typeof(TeraEntity).GetProperty(nameof(TeraEntity.ColumnWithInsertTemplate)));

            columnInfo1.ColumnName.ShouldBe("Id");
            columnInfo1.ForceToUtc.ShouldBeFalse();
            columnInfo1.ResultColumn.ShouldBeFalse();

            columnInfo2.ColumnName.ShouldBe("another_column");
            columnInfo2.ForceToUtc.ShouldBeFalse();
            columnInfo2.ResultColumn.ShouldBeFalse();

            columnInfo3.ColumnName.ShouldBe("ColumnWithInsertTemplate");
            columnInfo3.InsertTemplate.ShouldBe("test");
            columnInfo3.UpdateTemplate.ShouldBe("test1");
        }

        [Fact]
        public void GetColumnInfo_GivenPropertyWithResultAttribute_ShouldBeValdid()
        {
            var columnInfo1 = _mapper.GetColumnInfo(typeof(TeraEntity).GetProperty(nameof(TeraEntity.ResultColumn)));
            var columnInfo2 = _mapper.GetColumnInfo(typeof(YottaEntity).GetProperty(nameof(YottaEntity.ResultColumn)));

            columnInfo1.ColumnName.ShouldBe("result_column");
            columnInfo1.ForceToUtc.ShouldBeFalse();
            columnInfo1.ResultColumn.ShouldBeTrue();

            columnInfo2.ColumnName.ShouldBe("ResultColumn");
            columnInfo2.ForceToUtc.ShouldBeFalse();
            columnInfo2.ResultColumn.ShouldBeTrue();
        }

        [Fact]
        public void GetFromDbConverter_GivenPropertyAndType_ShouldBeNull()
        {
            var func = _mapper.GetFromDbConverter(typeof(TeraEntity).GetProperty(nameof(TeraEntity.TheId)), typeof(int));
            func.ShouldBeNull();
        }

        [Fact]
        public void GetToDbConverter_GivenProperty_ShouldBeNull()
        {
            var func = _mapper.GetToDbConverter(typeof(TeraEntity).GetProperty(nameof(TeraEntity.TheId)));
            func.ShouldBeNull();
        }

        [TableName("TblTeraEntity")]
        [PrimaryKey("TheId", AutoIncrement = false)]
        [ExplicitColumns]
        public class TeraEntity
        {
            [Column("Id")]
            public int TheId { get; set; }

            [Column("another_column")]
            public string AnotherColumn { get; set; }

            [ResultColumn("result_column")]
            public string ResultColumn { get; set; }

            public string NotAColumn { get; set; }

            [Column(ForceToUtc = true)]
            public DateTime Created { get; set; }

            [Column(InsertTemplate = "test", UpdateTemplate = "test1")]
            public string ColumnWithInsertTemplate { get; set; }
        }

        [TableName("yotta_entities")]
        [PrimaryKey("Id", AutoIncrement = true, SequenceName = "SEQ_1")]
        public class YottaEntity
        {
            [Column]
            public int Id { get; set; }

            [Column]
            public string AnotherColumn { get; set; }

            [ResultColumn]
            public string ResultColumn { get; set; }

            [Ignore]
            public string NotAColumn { get; set; }

            [Column(ForceToUtc = true)]
            public DateTime Created { get; set; }
        }

        public class ExaEntity
        {
            public int Id { get; set; }

            public string AnotherColumn { get; set; }
        }

        public class ZettaEntity
        {
            public int TheId { get; set; }

            public string AnotherColumn { get; set; }
        }
    }
}