namespace Inventory_Web.Models
{
    public class Order_history
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public Sellers Sellers { get; set; }
        public int OrderId { get; set; }
        public Orders Orders { get; set; }
    }
}
