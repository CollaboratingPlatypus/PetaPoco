// <copyright company="PetaPoco - CollaboratingPlatypus">
//      Apache License, Version 2.0 https://github.com/CollaboratingPlatypus/PetaPoco/blob/master/LICENSE.txt
// </copyright>
// <author>PetaPoco - CollaboratingPlatypus</author>
// <date>2015/12/07</date>

using System;
using Shouldly;

namespace PetaPoco.Tests.Integration.Models
{
    [ExplicitColumns]
    [TableName("Orders")]
    [PrimaryKey("Id")]
    public class Order
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public Guid PersonId { get; set; }

        [Column]
        public string PoNumber { get; set; }

        [Column]
        public DateTime CreatedOn { get; set; }

        [Column]
        public string CreatedBy { get; set; }

        public void ShouldBe(Order other)
        {
            Id.ShouldBe(other.Id);
            PersonId.ShouldBe(other.PersonId);
            PoNumber.ShouldBe(other.PoNumber);
            CreatedOn.ShouldBe(other.CreatedOn);
            CreatedBy.ShouldBe(other.CreatedBy);
        }
    }
}