// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2018/07/02</date>

using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
    [Collection("MySqlTests")]
    public class MySqlMiscellaneousTests : BaseDatabase
    {
        public MySqlMiscellaneousTests()
            : base(new MySqlDBTestProvider())
        {
        }

        [Fact]
        public void Insert_GivenPocoWithColumnsOfDifferentAccessSpecifiers_ShouldInsert()
        {
            var p = new AccessSpecifiersPoco();
            p.SetValues(1, 2, 3, 4);

            DB.Insert(p);
            var otherP = DB.Single<AccessSpecifiersPoco>(p.Id);

            otherP.ShouldBeValid(1, 2, 3, 4);
        }

        [Fact]
        public void Update_GivenPocoWithColumnsOfDifferentAccessSpecifiers_ShouldUpdate()
        {
            var p = new AccessSpecifiersPoco();
            p.SetValues(1, 2, 3, 4);

            DB.Insert(p);
            p.SetValues(2, 3, 4, 5);
            DB.Update(p);
            var otherP = DB.Single<AccessSpecifiersPoco>(p.Id);

            otherP.ShouldBeValid(2, 3, 4, 5);
        }

        [ExplicitColumns]
        [TableName("BugInvestigation_3F489XV0")]
        [PrimaryKey("Id")]
        public class AccessSpecifiersPoco
        {
            [Column]
            public int Id { get; set; }

            [Column("TC1")]
            public int Column1 { get; set; }

            [Column("TC2")]
            internal int Column2 { get; set; }

            [Column("TC3")]
            private int Column3 { get; set; }

            [Column("TC4")]
            protected int Column4 { get; set; }

            public void SetValues(int c1, int c2, int c3, int c4)
            {
                Column1 = c1;
                Column2 = c2;
                Column3 = c3;
                Column4 = c4;
            }

            public void ShouldBeValid(int c1, int c2, int c3, int c4)
            {
                Column1.ShouldBe(c1);
                Column2.ShouldBe(c2);
                Column3.ShouldBe(c3);
                Column4.ShouldBe(c4);
            }
        }
    }
}