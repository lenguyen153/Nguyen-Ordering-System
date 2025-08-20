using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagementSystem.Models;

public class OrderItem
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}
