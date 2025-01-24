namespace Ergasia2.Tests;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AccountControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Signup_ValidInput_ShouldReturnSuccess()
    {
        var signupModel = new { Email = "testuser@example.com", Password = "Test@123", ConfirmPassword = "Test@123" };
        var content = new StringContent(JsonSerializer.Serialize(signupModel), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/signup", content);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Signup_InvalidInput_ShouldReturnBadRequest()
    {
        var signupModel = new { Email = "", Password = "", ConfirmPassword = "" };
        var content = new StringContent(JsonSerializer.Serialize(signupModel), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/signup", content);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnToken()
    {
        var signupModel = new { Email = "loginuser@example.com", Password = "Login@123", ConfirmPassword = "Login@123" };
        var signupContent = new StringContent(JsonSerializer.Serialize(signupModel), Encoding.UTF8, "application/json");
        await _client.PostAsync("/signup", signupContent);

        var loginModel = new { Email = "loginuser@example.com", Password = "Login@123" };
        var loginContent = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/auth/login", loginContent);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var responseData = await response.Content.ReadAsStringAsync();
        Assert.Contains("token", responseData);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ShouldReturnUnauthorized()
    {
        var loginModel = new { Email = "nonexistentuser@example.com", Password = "WrongPassword" };
        var loginContent = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/auth/login", loginContent);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_MissingCredentials_ShouldReturnBadRequest()
    {
        var loginModel = new { Email = "", Password = "" };
        var loginContent = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/auth/login", loginContent);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}
