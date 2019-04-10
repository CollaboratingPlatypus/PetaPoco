using Shouldly;

namespace PetaPoco.Tests.Unit.Models
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

        [ResultColumn]
        public decimal Total { get; set; }

        public void ShouldBe(OrderLine other)
        {
            Id.ShouldBe(other.Id);
            OrderId.ShouldBe(other.OrderId);
            Quantity.ShouldBe(other.Quantity);
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
            SellPrice.ShouldNotBe(other.SellPrice);
        }
    }
}