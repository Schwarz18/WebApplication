namespace WebApplication1.DTOs
{
    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateOrderDto
    {
        public Guid UserId { get; set; }
        public string DeliveryAddress { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemReadDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class OrderReadDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public List<OrderItemReadDto> Items { get; set; }
    }
}
