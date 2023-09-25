using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

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
