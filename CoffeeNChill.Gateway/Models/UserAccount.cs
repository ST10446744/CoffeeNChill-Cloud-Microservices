using Azure;
using Azure.Data.Tables;

namespace CoffeeNChill.Gateway.Models;

public class UserAccount : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;

    public string RowKey { get; set; } = string.Empty;

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Customer";

    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
}