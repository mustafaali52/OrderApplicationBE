﻿namespace OrderDeliverySystem.Core.Dtos
{
    public class OrderItemsDto
    {
        public int Id { get; set; }
        public int OrderId {  get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
