using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseTriageTests : BaseDbContext
    {
        protected BaseTriageTests(BaseDbProviderFactory provider)
            : base(provider)
        {
        }
    }
}
