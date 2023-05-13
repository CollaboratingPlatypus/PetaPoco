using Xunit;

namespace PetaPoco.Tests.Integration.Databases.MySQL
{
	[Collection("MySql")]
	public class MySqlQueryTests : BaseQueryTests
	{
		public MySqlQueryTests()
			: base(new MySqlDBTestProvider())
		{
		}
	}
}
