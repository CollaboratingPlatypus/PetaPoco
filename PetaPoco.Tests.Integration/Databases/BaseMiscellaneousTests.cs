using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseMiscellaneousTests : BaseDatabase
    {
        protected BaseMiscellaneousTests(DBTestProvider provider)
            : base(provider)
        {
        }

    }
}
