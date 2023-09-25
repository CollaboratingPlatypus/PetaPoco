namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseTriageTests : BaseDatabase
    {
        protected BaseTriageTests(DBTestProvider provider)
            : base(provider)
        {
        }
    }
}
