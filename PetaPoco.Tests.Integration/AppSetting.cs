#if NETCOREAPP
using System.Collections.Generic;

namespace PetaPoco.Tests.Integration
{
    public class AppSetting
    {
        public List<ConnectionStringSetting> ConnectionStrings { get; set;  } = new List<ConnectionStringSetting>();
    }
}
#endif