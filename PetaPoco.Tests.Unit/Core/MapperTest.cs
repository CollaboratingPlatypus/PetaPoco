using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PetaPoco.Tests.Unit.Core
{
    public class MapperTest
    {
        private class StringMapperTest : ConventionMapper
        {
            public StringMapperTest()
            {
                Thread.Sleep(2000);
            }
        }

        [Fact]
        public void Register_Ok()
        {
            NoThrowExceptionMethod();
        }

        private void NoThrowExceptionMethod()
        {
            var t1 = Task.Run(() => RegisterMethod());
            var t2 = Task.Run(() => RegisterMethod());
            Task.WaitAll(t1, t2);
        }

        private void RegisterMethod()
        {
            if (Mappers.GetMapper(typeof(string), null) == null) Mappers.Register(typeof(string), new StringMapperTest());
        }
    }
}
