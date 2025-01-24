using Ergasia2.Tests.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Ergasia2.Tests;
public class TodoControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTodos_ShouldReturnOk()
    {
        await AuthenticateClient();
        var response = await _client.GetAsync("/todos");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAllTodos_Unauthorized_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/todos");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_ShouldReturnOk()
    {
        await AuthenticateClient();

        var todoModel = new { Title = "New Todo" };
        var content = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/todos", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task CreateTodo_Unauthorized_ShouldReturnUnauthorized()
    {
        var todoModel = new { Title = "New Todo" };
        var content = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/todos", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodoById_ShouldReturnOk()
    {
        await AuthenticateClient();

        var todoModel = new { Title = "Todo for GetTodoById Test" };
        var content = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/todos", content);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var responseBody = await createResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(createdTodo?.Id);

        var response = await _client.GetAsync($"/todos/{createdTodo.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task GetTodoById_Unauthorized_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/todos/1");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_ShouldReturnOk()
    {
        await AuthenticateClient();

        var todoModel = new { Title = "Todo for UpdateTodo Test" };
        var content = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/todos", content);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var responseBody = await createResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(createdTodo?.Id);

        var updatedTodoModel = new { Title = "Updated Todo Name" };
        var updateContent = new StringContent(JsonSerializer.Serialize(updatedTodoModel), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/todos/{createdTodo.Id}", updateContent);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_Unauthorized_ShouldReturnUnauthorized()
    {
        var todoModel = new { Title = "Updated Todo" };
        var content = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("/todos/1", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_ShouldReturnOk()
    {
        await AuthenticateClient();

        var todoModel = new { Title = "Todo for DeleteTodo Test" };
        var content = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/todos", content);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        var responseBody = await createResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(createdTodo?.Id);

        var response = await _client.DeleteAsync($"/todos/{createdTodo.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    [Fact]
    public async Task DeleteTodo_Unauthorized_ShouldReturnUnauthorized()
    {
        var response = await _client.DeleteAsync("/todos/1");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetTodoItem_ShouldReturnOk()
    {
        await AuthenticateClient();

        var todoModel = new { Title = "Todo for GetTodoItem Test" };
        var todoContent = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");
        var createTodoResponse = await _client.PostAsync("/todos", todoContent);

        Assert.Equal(HttpStatusCode.OK, createTodoResponse.StatusCode);

        var todoResponseBody = await createTodoResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoResponse>(todoResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var itemModel = new { Name = "Todo Item for GetTodoItem Test" };
        var itemContent = new StringContent(JsonSerializer.Serialize(itemModel), Encoding.UTF8, "application/json");
        var createItemResponse = await _client.PostAsync($"/todos/{createdTodo!.Id}/items", itemContent);

        Assert.Equal(HttpStatusCode.Created, createItemResponse.StatusCode);

        var itemResponseBody = await createItemResponse.Content.ReadAsStringAsync();
        var createdItem = JsonSerializer.Deserialize<TodoItemResponse>(itemResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var response = await _client.GetAsync($"/todos/{createdTodo.Id}/items/{createdItem!.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }



    [Fact]
    public async Task GetTodoItem_Unauthorized_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/todos/1/items/1");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodoItem_ShouldReturnCreated()
    {
        await AuthenticateClient();

        var todoModel = new { Title = "Todo for CreateTodoItem Test" };
        var todoContent = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");
        var createTodoResponse = await _client.PostAsync("/todos", todoContent);

        Assert.Equal(HttpStatusCode.OK, createTodoResponse.StatusCode);

        var todoResponseBody = await createTodoResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoResponse>(todoResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var itemModel = new { Name = "New Todo Item", IsComplete = false };
        var itemContent = new StringContent(JsonSerializer.Serialize(itemModel), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/todos/{createdTodo!.Id}/items", itemContent);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodoItem_Unauthorized_ShouldReturnUnauthorized()
    {
        var itemModel = new { Name = "New Todo Item" };
        var content = new StringContent(JsonSerializer.Serialize(itemModel), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/todos/1/items", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodoItem_ShouldReturnOK()
    {
        await AuthenticateClient();

        var todoModel = new { Title = "Todo for UpdateTodoItem Test" };
        var todoContent = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");
        var createTodoResponse = await _client.PostAsync("/todos", todoContent);

        Assert.Equal(HttpStatusCode.OK, createTodoResponse.StatusCode);

        var todoResponseBody = await createTodoResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoResponse>(todoResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var itemModel = new { Name = "Todo Item for UpdateTodoItem Test", IsComplete = false };
        var itemContent = new StringContent(JsonSerializer.Serialize(itemModel), Encoding.UTF8, "application/json");
        var createItemResponse = await _client.PostAsync($"/todos/{createdTodo!.Id}/items", itemContent);

        Assert.Equal(HttpStatusCode.Created, createItemResponse.StatusCode);

        var itemResponseBody = await createItemResponse.Content.ReadAsStringAsync();
        var createdItem = JsonSerializer.Deserialize<TodoItemResponse>(itemResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var updatedItemModel = new { Name = "Updated Todo Item", IsComplete = true };
        var updatedContent = new StringContent(JsonSerializer.Serialize(updatedItemModel), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/todos/{createdTodo.Id}/items/{createdItem!.Id}", updatedContent);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodoItem_Unauthorized_ShouldReturnUnauthorized()
    {
        var itemModel = new { Name = "Updated Todo Item" };
        var content = new StringContent(JsonSerializer.Serialize(itemModel), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("/todos/1/items/1", content);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodoItem_ShouldReturnOk()
    {
        await AuthenticateClient();

        var todoModel = new { Title = "Todo for DeleteTodoItem Test" };
        var todoContent = new StringContent(JsonSerializer.Serialize(todoModel), Encoding.UTF8, "application/json");
        var createTodoResponse = await _client.PostAsync("/todos", todoContent);

        Assert.Equal(HttpStatusCode.OK, createTodoResponse.StatusCode);

        var todoResponseBody = await createTodoResponse.Content.ReadAsStringAsync();
        var createdTodo = JsonSerializer.Deserialize<TodoResponse>(todoResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var itemModel = new { Name = "Todo Item for DeleteTodoItem Test" };
        var itemContent = new StringContent(JsonSerializer.Serialize(itemModel), Encoding.UTF8, "application/json");
        var createItemResponse = await _client.PostAsync($"/todos/{createdTodo!.Id}/items", itemContent);

        Assert.Equal(HttpStatusCode.Created, createItemResponse.StatusCode);

        var itemResponseBody = await createItemResponse.Content.ReadAsStringAsync();
        var createdItem = JsonSerializer.Deserialize<TodoItemResponse>(itemResponseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var response = await _client.DeleteAsync($"/todos/{createdTodo.Id}/items/{createdItem!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodoItem_Unauthorized_ShouldReturnUnauthorized()
    {
        var response = await _client.DeleteAsync("/todos/1/items/1");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #region Helpers

    private async Task<string> GetAuthToken()
    {
        var signupModel = new { Email = "test@user", Password = "Test@123", ConfirmPassword = "Test@123" };
        var signupContent = new StringContent(JsonSerializer.Serialize(signupModel), Encoding.UTF8, "application/json");
        await _client.PostAsync("/signup", signupContent);

        var loginModel = new { Email = "test@user", Password = "Test@123" };
        var loginContent = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");
        var loginResponse = await _client.PostAsync("/auth/login", loginContent);

        var responseBody = await loginResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var tokenResponse = JsonSerializer.Deserialize<CustomAccessTokenResponse>(responseBody, options);
        return tokenResponse?.AccessToken ?? throw new Exception("Failed to retrieve access token.");
    }

    private async Task AuthenticateClient()
    {
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    #endregion
}
