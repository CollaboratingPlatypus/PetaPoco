using Shouldly;

namespace PetaPoco.Tests.Integration.Models
{
    [TableName("OrderLines")]
    [PrimaryKey("Id")]
    public class OrderLine
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int OrderId { get; set; }

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

        public void ShouldBe(OrderLine other)
        {
            Id.ShouldBe(other.Id);
            OrderId.ShouldBe(other.OrderId);
            Quantity.ShouldBe(other.Quantity);
            Status.ShouldBe(other.Status);
            SellPrice.ShouldBe(other.SellPrice);
        }

        public void ShouldNotBe(OrderLine other, bool sameIds)
        {
            if (sameIds)
            {
                Id.ShouldBe(other.Id);
                OrderId.ShouldBe(other.OrderId);
            }
            else
            {
                Id.ShouldNotBe(other.Id);
                OrderId.ShouldNotBe(other.OrderId);
            }

            Quantity.ShouldNotBe(other.Quantity);
            Status.ShouldNotBe(other.Status);
            SellPrice.ShouldNotBe(other.SellPrice);
        }
    }
}