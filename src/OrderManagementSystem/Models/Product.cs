using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Models;

public class Product
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}
