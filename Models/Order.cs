using System.Text.Json.Serialization;

namespace OrderDeliverySystem.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public ICollection<OrderItems> Items { get; set; }
    }
}
