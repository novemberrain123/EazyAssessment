using EazyAssessment.Server.Controllers;
using EazyAssessment.Server.Data;
using EazyAssessment.Server.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class ControllerUnitTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "fakekeyfakekeyfakekeyfakekeyfakekeyfakekey",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            })
            .Build();
    }

    [Fact]
    public async Task Register_ShouldCreateUser()
    {
        var context = CreateDbContext();
        var config = CreateConfiguration();
        var controller = new AuthController(context, config);

        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Lim",
            Email = "john@example.com",
            Password = "Password123!"
        };

        var result = await controller.Register(request);

        Assert.IsType<OkResult>(result);
        Assert.Single(context.Users);
        Assert.Equal("john@example.com", context.Users.First().Email);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturnBadRequest()
    {
        var context = CreateDbContext();
        var config = CreateConfiguration();
        var controller = new AuthController(context, config);

        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Lim",
            Email = "john@example.com",
            Password = "Password123!"
        };

        await controller.Register(request);
        var result = await controller.Register(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_WrongPassword_ShouldReturnUnauthorized()
    {
        var context = CreateDbContext();
        var config = CreateConfiguration();
        var controller = new AuthController(context, config);

        await controller.Register(new RegisterRequest
        {
            FirstName = "John",
            LastName = "Lim",
            Email = "john@example.com",
            Password = "CorrectPassword123!"
        });

        var result = await controller.Login(new LoginRequest
        {
            Email = "john@example.com",
            Password = "WrongPassword123!"
        });

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}