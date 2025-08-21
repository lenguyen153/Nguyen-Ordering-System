using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace OrderManagementSystem.IntegrationTests;

public class BasicApiIntegrationTest : IClassFixture<WebApplicationFactory<OrderManagementSystem.Program>>
{
    private readonly WebApplicationFactory<OrderManagementSystem.Program> _factory;

    public BasicApiIntegrationTest(WebApplicationFactory<OrderManagementSystem.Program> factory)
    {
        _factory = factory;
    }
    
    // Test the Products API
    [Fact]
    public async Task Get_Products_Returns_Unauthorized_Or_EmptyList()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/products");
        // Should be 401 Unauthorized (if auth enforced) or 200 OK with empty list (if not)
        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized || response.IsSuccessStatusCode);
    }
}
