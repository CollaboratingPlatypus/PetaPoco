using Shouldly;

namespace PetaPoco.Tests.Integration.Documentation.Pocos
{
    public class UnconventionalPoco
    {
        public int PrimaryKey { get; set; }

        public string Text { get; set; }

        public void PropertiesShouldBe(UnconventionalPoco other)
        {
            PrimaryKey.ShouldBe(other.PrimaryKey);
            Text.ShouldBe(other.Text);
        }
    }
}
