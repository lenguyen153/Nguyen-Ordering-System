using OrderManagementSystem.Models;
using Xunit;
using System;
using System.Collections.Generic;

namespace OrderManagementSystem.UnitTests;

public class BasicUnitTests
{
    [Fact]
    public void Product_Model_Stores_And_Retrieves_Values()
    {
        var product = new Product { Name = "Test", Price = 9.99m, Stock = 5 };
        Assert.Equal("Test", product.Name);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal(5, product.Stock);
    }

    [Fact]
    public void Order_TotalPrice_Is_Sum_Of_Items()
    {
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { Price = 10m, Quantity = 2 },
                new OrderItem { Price = 5m, Quantity = 3 }
            }
        };
        order.TotalPrice = 0;
        foreach (var item in order.Items)
            order.TotalPrice += item.Price * item.Quantity;
        Assert.Equal(35m, order.TotalPrice);
    }

    [Fact]
    public void ApplicationUser_Defaults_To_User_Role()
    {
        var user = new ApplicationUser();
        Assert.Equal(UserRole.User, user.Role);
    }
}