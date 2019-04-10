using System;

namespace PetaPoco.Tests.Unit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiresCleanUpAttribute : Attribute
    {
    }
}