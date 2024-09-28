using System.ComponentModel.DataAnnotations;

namespace Inventory_Web.Models.ViewModel
{
    public class SellerVM
    {
        [Required]
        [MinLength(3)]
        public string? Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Street { get; set; }
        [Required]
        public string? City { get; set; }
        public int? SellerId { get; set; }
    }
}
