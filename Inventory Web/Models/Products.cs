using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Web.Models
{
    public class Products
    {
        public int Id { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public int price { get; set; }
        [ValidateNever]
        public string url { get; set; }
        [Required]
        public int stock { get; set; }
        [ValidateNever]
        public string status { get; set; }
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        public int SellerId { get; set; }
        [ValidateNever]
        public Sellers Sellers { get; set; }
        [ValidateNever]
        public ICollection<Orders> Orders { get; set; }
    }
}
