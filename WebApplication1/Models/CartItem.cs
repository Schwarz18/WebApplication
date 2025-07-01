namespace WebApplication1.Models
{
    public class CartItem
    {
        public Guid id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid UserId { get; set; }
        public int Quantity { get; set; }
    }
}
