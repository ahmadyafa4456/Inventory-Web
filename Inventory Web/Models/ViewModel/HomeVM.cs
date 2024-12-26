using Inventory_Web.Models;

namespace Inventory_Web.Models.ViewModel
{
    public class HomeVM
    {
        public List<Products> product = new List<Products>();
        public List<Orders> orders = new List<Orders>();
    }
}