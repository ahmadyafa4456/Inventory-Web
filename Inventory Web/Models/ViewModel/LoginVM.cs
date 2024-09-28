using System.ComponentModel.DataAnnotations;

namespace Inventory_Web.Models.ViewModel
{
    public class LoginVM
    {
        [Required]
        public string? Name { get; set; }
        //[Required]
        //[DataType(DataType.EmailAddress)]
        //public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
