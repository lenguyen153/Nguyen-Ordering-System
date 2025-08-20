using System.Text.Json.Serialization;

namespace OrderManagementSystem.DTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SalesRange { Day, Week, Month }

public class SalesStatPoint
{
    public DateTime PeriodStart { get; set; }
    public decimal TotalSales { get; set; }
}
