using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Web.Models
{
    public class Address
    {
        public int Id { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Street { get; set; }
        public int? SellerId { get; set; }
        [ValidateNever]
        public Sellers Sellers { get; set; }
    }
}
