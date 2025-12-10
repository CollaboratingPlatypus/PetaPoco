using System;
using PetaPoco.Tests.Integration.Models;
using Shouldly;
using Xunit;
using PetaPoco.Tests.Integration.Providers;


namespace PetaPoco.Tests.Integration.Databases.MSAccess
{
    [Collection("MSAccess")]
    public class MSAccessInsertTests : InsertTests
    {
        public MSAccessInsertTests()
            : base(new MSAccessTestProvider())
        {
        }

        /// <summary>
        /// Regression test for MS Access DateTime millisecond handling.
        /// MS Access OLE DB provider cannot handle DateTime parameters with sub-second precision.
        /// Without proper handling in MapParameterValue, DateTime values with milliseconds cause
        /// "Data type mismatch in criteria expression" errors. The fix truncates milliseconds before
        /// sending DateTime values to the OLE DB provider.
        /// See: https://github.com/CollaboratingPlatypus/PetaPoco/issues/736
        /// </summary>
        [Fact]
        public void Insert_GivenPocoWithMillisecondPrecisionDateTime_ShouldInsertSuccessfully()
        {
            // Test with DateTime that has NO milliseconds (baseline)
            var personWithoutMillis = new Person
            {
                Id = Guid.NewGuid(),
                Name = "No Milliseconds Person",
                Age = 35,
                Height = 180,
                Dob = new DateTime(2000, 6, 15, 14, 30, 45, 0, DateTimeKind.Utc)
            };

            _ = DB.Insert(personWithoutMillis);
            var retrieved1 = DB.Single<Person>(personWithoutMillis.Id);
            retrieved1.ShouldNotBeNull();
            retrieved1.Dob.ShouldNotBeNull();

            // Test with DateTime that HAS milliseconds (the actual regression test)
            // Before fix: throws OleDbException "Data type mismatch in criteria expression"
            // After fix: succeeds but milliseconds are truncated
            var personWithMillis = new Person
            {
                Id = Guid.NewGuid(),
                Name = "With Milliseconds Person",
                Age = 40,
                Height = 185,
                Dob = new DateTime(2000, 6, 15, 14, 30, 45, 123, DateTimeKind.Utc)
            };

            _ = DB.Insert(personWithMillis);

            var retrieved2 = DB.Single<Person>(personWithMillis.Id);
            retrieved2.ShouldNotBeNull();
            retrieved2.Dob.ShouldNotBeNull();
            // Allow 1 second tolerance because milliseconds are truncated by the fix
            retrieved2.Dob.Value.ShouldBe(personWithMillis.Dob.Value, TimeSpan.FromSeconds(1));
        }
    }
}
