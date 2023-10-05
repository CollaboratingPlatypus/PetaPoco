using Shouldly;

namespace PetaPoco.Tests.Integration.Models.MSAccess
{
    [TableName("JoinableOrderLines")]
    [PrimaryKey("Id")]
    public class JoinableOrderLine
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int JoinableOrderId { get; set; }

        [Column(Name = "Qty")]
        public short Quantity { get; set; }

        [Column]
        public decimal SellPrice { get; set; }

        [Column]
        public OrderLineStatus Status { get; set; }

        [ResultColumn]
        public decimal Total
        {
            get { return SellPrice * Quantity; }
        }

        public void ShouldBe(JoinableOrderLine other)
        {
            Id.ShouldBe(other.Id);
            JoinableOrderId.ShouldBe(other.JoinableOrderId);
            Quantity.ShouldBe(other.Quantity);
            Status.ShouldBe(other.Status);
            SellPrice.ShouldBe(other.SellPrice);
        }

        public void ShouldNotBe(JoinableOrderLine other, bool sameIds)
        {
            if (sameIds)
            {
                Id.ShouldBe(other.Id);
                JoinableOrderId.ShouldBe(other.JoinableOrderId);
            }
            else
            {
                Id.ShouldNotBe(other.Id);
                JoinableOrderId.ShouldNotBe(other.JoinableOrderId);
            }

            Quantity.ShouldNotBe(other.Quantity);
            Status.ShouldNotBe(other.Status);
            SellPrice.ShouldNotBe(other.SellPrice);
        }
    }
}
