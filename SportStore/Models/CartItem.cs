
using Newtonsoft.Json;

namespace SportStore.Models
{
    public class CartItem
    {
        public Product? product { get; set; }
        public int producDetailId { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }
}
