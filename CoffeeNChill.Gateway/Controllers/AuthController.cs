using Azure.Data.Tables;
using CoffeeNChill.Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoffeeNChill.Gateway.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly TableClient _tableClient;
    private readonly PasswordHasher<UserAccount> _passwordHasher;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;

        string connectionString =
            configuration.GetConnectionString("AzureWebJobsStorage")!;

        _tableClient = new TableClient(connectionString, "Users");
        _tableClient.CreateIfNotExists();

        _passwordHasher = new PasswordHasher<UserAccount>();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("All fields are required.");
        }

        string email = request.Email.Trim().ToLowerInvariant();

        var existingUser =
            await _tableClient.GetEntityIfExistsAsync<UserAccount>(
                "Customer", email);

        if (existingUser.HasValue)
            return BadRequest("Email already registered.");

        var user = new UserAccount
        {
            PartitionKey = "Customer",
            RowKey = email,
            Email = email,
            FullName = request.FullName,
            Role = "Customer",
            CreatedDate = DateTimeOffset.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, request.Password);

        await _tableClient.AddEntityAsync(user);

        return Ok("Registration successful.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        string email = request.Email.Trim().ToLowerInvariant();

        var result =
            await _tableClient.GetEntityIfExistsAsync<UserAccount>(
                "Customer", email);

        if (!result.HasValue)
            return Unauthorized("Invalid email or password.");

        var user = result.Value;

        var verification = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verification == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid email or password.");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            fullName = user.FullName,
            role = user.Role
        });
    }
}
