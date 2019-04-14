using System;
using Shouldly;

namespace PetaPoco.Tests.Integration.Documentation.Pocos
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

        [Column("OrderStatus")]
        public OrderStatus Status { get; set; }

        public void ShouldBe(Order other)
        {
            Id.ShouldBe(other.Id);
            PersonId.ShouldBe(other.PersonId);
            PoNumber.ShouldBe(other.PoNumber);
            Status.ShouldBe(other.Status);
            CreatedOn.ShouldBe(other.CreatedOn);
            CreatedBy.ShouldBe(other.CreatedBy);
        }
    }
}