using System;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: Xunit.TestFramework("PetaPoco.Tests.Integration.BeforeAnyTestRuns", "PetaPoco.Tests.Integration")]

namespace PetaPoco.Tests.Integration
{
   public class BeforeAnyTestRuns : XunitTestFramework
   {
      public BeforeAnyTestRuns(IMessageSink messageSink)
        :base(messageSink)
      {
        Shouldly.ShouldlyConfiguration.DefaultTaskTimeout = TimeSpan.FromSeconds(30);
      }
   }
}
