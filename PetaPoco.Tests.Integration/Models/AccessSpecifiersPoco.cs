using Shouldly;

namespace PetaPoco.Tests.Integration.Models
{
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
