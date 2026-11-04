using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace CoffeeNChill.Gateway.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize(Roles = "Admin,Staff")]
public class DocumentsController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public DocumentsController(IHttpClientFactory factory, IConfiguration config)
    {
        _httpClient = factory.CreateClient();
        _httpClient.BaseAddress = new Uri(config["FunctionsBaseUrl"]!);
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments()
    {
        var response = await _httpClient.GetAsync("api/documents");
        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json");
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please select a file.");

        using var form = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        using var fileContent = new StreamContent(stream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(file.ContentType);

        form.Add(fileContent, "file", file.FileName);

        var response = await _httpClient.PostAsync(
            "api/documents/upload", form);

        var content = await response.Content.ReadAsStringAsync();

        return Content(content, "application/json");
    }

    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> Download(string fileName)
    {
        var response = await _httpClient.GetAsync(
            $"api/documents/download/{Uri.EscapeDataString(fileName)}");

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var stream = await response.Content.ReadAsStreamAsync();

        var contentType = response.Content.Headers.ContentType?.ToString()
                          ?? "application/octet-stream";

        return File(stream, contentType, fileName);
    }
}