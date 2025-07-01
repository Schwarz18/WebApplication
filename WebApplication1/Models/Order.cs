using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string DeliveryAddress { get; set; }
        public string PaymentMethod { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
