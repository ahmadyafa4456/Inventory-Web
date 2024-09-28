using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Web.Models
{
    public class AppUser : IdentityUser<int>
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string? Name { get; set; }
        [ValidateNever]
        public ICollection<Sellers> Sellers { get; set; }
    }
}
