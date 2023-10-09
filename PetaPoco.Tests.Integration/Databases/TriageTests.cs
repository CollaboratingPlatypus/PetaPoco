namespace PetaPoco.Tests.Integration
{
    public abstract class TriageTests : BaseDbContext
    {
        protected TriageTests(TestProvider provider)
            : base(provider)
        {
        }
    }
}
