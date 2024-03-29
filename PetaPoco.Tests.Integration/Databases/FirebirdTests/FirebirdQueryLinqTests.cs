﻿using PetaPoco.Tests.Integration.Providers;
using Xunit;

namespace PetaPoco.Tests.Integration.Databases.Firebird
{
    [Collection("Firebird")]
    public class FirebirdQueryLinqTests : QueryLinqTests
    {
        public FirebirdQueryLinqTests()
            : base(new FirebirdTestProvider())
        {
        }
    }
}
