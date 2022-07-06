using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace PetaPoco.Tests.Unit
{
    public class CreateParameterTests : IDisposable
    {
        public readonly DbConnection _conn = new SqlConnection();
        public readonly Database _db = new Database("foo", "System.Data.SqlClient");

        public void Dispose()
        {
            _conn.Dispose();
            _db.Dispose();
        }

        private void Validate(IDbDataParameter param, string name, object value, ParameterDirection direction)
        {
            param.ShouldSatisfyAllConditions(
                () => param.ShouldNotBeNull(),
                () => param.ParameterName.ShouldBe(name),
                () => param.Value.ShouldBe(value),
                () => param.Direction.ShouldBe(direction)
            );
        }

        [Fact]
        public void CreateWithNoParams_Should_ReturnDefault()
        {
            var output = _db.CreateParameter();
            Validate(output, String.Empty, null, ParameterDirection.Input);
        }

        [Fact]
        public void CreateWithNameAndValue_Should_ReturnInput()
        {
            var name = "request_id";
            var value = 42;
            var output = _db.CreateParameter(name, value);
            Validate(output, name, value, ParameterDirection.Input);
        }

        [Fact]
        public void CreateWithNameAndDirection_Should_HaveNullValue()
        {
            var name = "first_name";
            var direction = ParameterDirection.Output;
            var output = _db.CreateParameter(name, direction);
            Validate(output, name, null, direction);
        }

        [Fact]
        public void CreateWithAllParams_Should_ReturnThem()
        {
            var name = "ssn";
            var value = "123-45-6789";
            var direction = ParameterDirection.InputOutput;
            var output = _db.CreateParameter(name, value, direction);
            Validate(output, name, value, direction);
        }
    }
}
