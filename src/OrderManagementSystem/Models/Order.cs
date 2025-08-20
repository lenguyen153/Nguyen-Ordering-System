using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<OrderItem> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}
