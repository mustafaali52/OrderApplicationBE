namespace OrderDeliverySystem.Core.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public List<OrderItemsDto> OrderListDto { get; set; }
    }
}
