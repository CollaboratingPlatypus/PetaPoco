using System;
using System.Collections.Generic;
using Shouldly;

namespace PetaPoco.Tests.Integration.Models.MSAccess
{
    [ExplicitColumns]
    [TableName("JoinableOrders")]
    [PrimaryKey("Id")]
    public class JoinableOrder
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public Guid JoinablePersonId { get; set; }

        [Column]
        public string PoNumber { get; set; }

        [Column]
        public DateTime CreatedOn { get; set; }

        [Column]
        public string CreatedBy { get; set; }

        [Column("OrderStatus")]
        public OrderStatus Status { get; set; }

        public JoinablePerson JoinablePerson { get; set; }

        public List<OrderLine> OrderLines { get; set; }

        public void ShouldBe(JoinableOrder other)
        {
            Id.ShouldBe(other.Id);
            JoinablePersonId.ShouldBe(other.JoinablePersonId);
            PoNumber.ShouldBe(other.PoNumber);
            Status.ShouldBe(other.Status);
            CreatedOn.ShouldBe(other.CreatedOn);
            CreatedBy.ShouldBe(other.CreatedBy);
        }

        public void ShouldNotBe(JoinableOrder other, bool sameIds)
        {
            if (sameIds)
            {
                Id.ShouldBe(other.Id);
                JoinablePersonId.ShouldBe(other.JoinablePersonId);
            }
            else
            {
                Id.ShouldNotBe(other.Id);
                JoinablePersonId.ShouldNotBe(other.JoinablePersonId);
            }

            PoNumber.ShouldNotBe(other.PoNumber);
            Status.ShouldNotBe(other.Status);
            CreatedOn.ShouldNotBe(other.CreatedOn);
            CreatedBy.ShouldNotBe(other.CreatedBy);
        }
    }
}
