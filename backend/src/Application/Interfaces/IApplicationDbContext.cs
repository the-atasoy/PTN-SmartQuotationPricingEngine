using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Product> Products { get; }
    DbSet<Request> Requests { get; }
    DbSet<RequestItem> RequestItems { get; }
    DbSet<ProductPriceHistory> ProductPriceHistories { get; }
        Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
