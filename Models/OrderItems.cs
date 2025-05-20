
using System.Text.Json.Serialization;

namespace OrderDeliverySystem.Models
{
    public class OrderItems
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quatity { get; set; }
    }
}
