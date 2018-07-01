#if NETCOREAPP
namespace PetaPoco.Tests.Integration
{
    public class ConnectionStringSetting
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
    }
}
#endif