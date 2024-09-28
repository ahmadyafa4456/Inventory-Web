using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Inventory_Web.Models
{
    public class Orders
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ValidateNever]
        public string Status { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int ProductsId { get; set; }
        [ValidateNever]
        public Products Products{ get; set; }
        public int SellersId { get; set; }
        [ValidateNever]
        public Sellers Sellers { get; set; }
        [ValidateNever]
        public ICollection<Order_history> Order_history { get; set; }
    }
}
