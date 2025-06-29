using EventLogger.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace EventLogger.Tests;

public class HealthCheckControllerTests
{
    [Fact]
    public void Get_ReturnsHealthyStatus()
    {
        // Arrange
        var controller = new HealthCheckController();

        // Act
        var result = controller.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        
        var response = result.Value;
        Assert.NotNull(response);
        
        // Verify the response has the expected structure
        var responseProperties = response.GetType().GetProperties();
        Assert.Contains(responseProperties, p => p.Name == "status");
        Assert.Contains(responseProperties, p => p.Name == "timestamp");
        Assert.Contains(responseProperties, p => p.Name == "service");
    }
}