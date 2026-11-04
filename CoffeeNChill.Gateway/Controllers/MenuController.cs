using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Json;

namespace CoffeeNChill.Gateway.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public MenuController(IHttpClientFactory factory, IConfiguration config)
    {
        _httpClient = factory.CreateClient();
        _httpClient.BaseAddress = new Uri(config["FunctionsBaseUrl"]!);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _httpClient.GetAsync("api/menu");
        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var response = await _httpClient.GetAsync(
            $"api/menu/category/{Uri.EscapeDataString(category)}");

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] object menuItem)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/menu", menuItem);

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{category}/{id}")]
    public async Task<IActionResult> Update(
        string category, string id, [FromBody] object menuItem)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/menu/{Uri.EscapeDataString(category)}/{Uri.EscapeDataString(id)}",
            menuItem);

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpDelete("{category}/{id}")]
    public async Task<IActionResult> Delete(string category, string id)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/menu/{Uri.EscapeDataString(category)}/{Uri.EscapeDataString(id)}");

        var content = await response.Content.ReadAsStringAsync();

        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = content,
            ContentType = "application/json"
        };
    }
}
