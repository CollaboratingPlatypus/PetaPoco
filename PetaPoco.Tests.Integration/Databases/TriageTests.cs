namespace PetaPoco.Tests.Integration
{
    public abstract class TriageTests : BaseDbContext
    {
        protected TriageTests(BaseDbProviderFactory provider)
            : base(provider)
        {
        }
    }
}
