// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/06</date>

using System;
using Shouldly;

namespace PetaPoco.Tests.Integration.Models
{
    [TableName("People")]
    [PrimaryKey("Id", AutoIncrement = false)]
    public class Person
    {
        [Column]
        public Guid Id { get; set; }

        [Column(Name = "FullName")]
        public string Name { get; set; }

        [Column]
        public long Age { get; set; }

        [Column]
        public int Height { get; set; }

        [Column]
        public DateTime? Dob { get; set; }

        [Ignore]
        public string NameAndAge => $"{Name} is of {Age}";

        public void ShouldBe(Person other)
        {
            Id.ShouldBe(other.Id);
            Name.ShouldBe(other.Name);
            Age.ShouldBe(other.Age);
            Height.ShouldBe(other.Height);
            Dob.ShouldBe(other.Dob);
        }
    }
}