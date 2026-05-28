using Domain.Common;

namespace Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? Phone { get; private set; }

    // Navigation properties
    private readonly List<Request> _requests = [];
    public IReadOnlyCollection<Request> Requests => _requests.AsReadOnly();

    private Customer() { } // EF Core

    public static Customer Create(string name, string email, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Customer email is required.", nameof(email));

        return new Customer
        {
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Phone = phone?.Trim()
        };
    }
}
