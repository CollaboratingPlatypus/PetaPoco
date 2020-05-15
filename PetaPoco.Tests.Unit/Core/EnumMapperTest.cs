using PetaPoco.Internal;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    public class EnumMapperTest
    {
        [Fact]
        public void Mapping_ExistingEnum_Ok()
        {
            var expected = FakeEnum.ExistingValue;
            var enumType = expected.GetType();
            var name = Enum.GetName(enumType, expected);

            var actual = EnumMapper.EnumFromString(enumType, name);

            actual.ShouldBe(expected);
        }

        [Fact]
        public void Mapping_NonExistingEnum_ThrowsMeaningfulException()
        {
            var enumType = typeof(FakeEnum);
            var nonExistentName = "ThisValueDoesNotExistOnAnyEnum";

            try
            {
                EnumMapper.EnumFromString(enumType, nonExistentName);
            }
            catch (InvalidOperationException ex)
            {
                var message = ex.Message;
                message.ShouldContain(nonExistentName, "missing value to convert");
                message.ShouldContain(enumType.Name, "missing Enum to convert into");
                return;
            }

            Assert.False(true, "Expedted InvalidOperationException");
        }

        private enum FakeEnum
        {
            ExistingValue
        }
    }
}