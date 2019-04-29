namespace PetaPoco.Tests.Integration.Documentation.Pocos
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
    }
}