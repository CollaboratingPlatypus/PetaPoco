using System;

namespace PetaPoco.Tests.Unit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; set; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}