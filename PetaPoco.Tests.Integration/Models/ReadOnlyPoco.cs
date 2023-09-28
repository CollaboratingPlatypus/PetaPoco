namespace PetaPoco.Tests.Integration.Models
{
    public class ReadOnlyPoco
    {
        public string FullName { get; }
    }

    public class ReadOnlyMultiPoco
    {
        public int OrderId { get; set; }
        public Person Person { get; }
    }
}
