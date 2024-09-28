using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Web.Models
{
    public class Sellers
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public Address Address { get; set; }
        [ValidateNever]
        public ICollection<Products> Products { get; set; }
        [ValidateNever]
        public ICollection<Orders> Orders { get; set; }
        [ValidateNever]
        public ICollection<Order_history> Order_history { get; set; }
    }
}
