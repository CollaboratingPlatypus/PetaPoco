// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2020/04/03</date>

using System;
using PetaPoco.Core;
using PetaPoco.Tests.Unit.Models;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    public class PrimaryKeyAttributeTests
    {
        [Fact]
        public void CompositeKey_AutoIncrement_ShouldBeFalse()
        {
            var attr = (PrimaryKeyAttribute)typeof(Item).GetCustomAttributes(typeof(PrimaryKeyAttribute), false)[0];

            attr.AutoIncrement.ShouldBe(false);
        }

        [Fact]
        public void CompositeKey_SetAutoIncrementToTrue_ShouldThrow()
        {
            var attr = (PrimaryKeyAttribute)typeof(Item).GetCustomAttributes(typeof(PrimaryKeyAttribute), false)[0];

            Should.Throw<InvalidOperationException>(() => attr.AutoIncrement = true);
        }
    }
}