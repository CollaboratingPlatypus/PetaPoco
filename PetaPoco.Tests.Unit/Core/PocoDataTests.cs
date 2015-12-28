// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/13</date>

using System;
using PetaPoco.Core;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    public class PocoDataTests
    {
        [Fact]
        public void GetFactory_GivenTypeWithNotPublicConstructor_ShouldBeValid()
        {
            var pd = PocoData.ForObject(TestEntity.Instance, "Id", new ConventionMapper());

            Should.Throw<InvalidOperationException>(() => pd.GetFactory("", "", 1, 1, null, null));
        }

        public class TestEntity
        {
            private TestEntity(string arg1)
            {
                
            }

            public static TestEntity Instance => new TestEntity("");
        }
    }
}