using PetaPoco.Tests.Integration.Models;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases
{
    public abstract class BaseMiscellaneousTests : BaseDbContext
    {
        protected BaseMiscellaneousTests(BaseDbProviderFactory provider)
            : base(provider)
        {
        }

        [Fact]
        [Trait("Issue", "#261")]
        public void Insert_GivenPocoWithColumnsOfDifferentAccessSpecifiers_ShouldInsert()
        {
            var p = new AccessSpecifiersPoco();
            p.SetValues(1, 2, 3, 4);

            DB.Insert(p);
            var otherP = DB.Single<AccessSpecifiersPoco>(p.Id);

            otherP.ShouldBeValid(1, 2, 3, 4);
        }

        [Fact]
        [Trait("Issue", "#261")]
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
    }
}
