using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await SeedUsersAsync(context);
        await SeedCustomersAsync(context);
        await SeedProductsAndHistoriesAsync(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var admin = User.Create(
            "admin@piton.com.tr",
            BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            UserRole.Admin);

        var user = User.Create(
            "user@piton.com.tr",
            BCrypt.Net.BCrypt.HashPassword("User123!"),
            UserRole.User);

        context.Users.AddRange(admin, user);
    }

    private static async Task SeedCustomersAsync(AppDbContext context)
    {
        if (await context.Customers.AnyAsync())
            return;

        var customers = new[]
        {
            Customer.Create("Aselsan Elektronik", "satin.alma@aselsan.com.tr", "+90 312 592 10 00"),
            Customer.Create("Türk Telekom", "tedarik@turktelekom.com.tr", "+90 312 555 00 00"),
            Customer.Create("Arçelik A.Ş.", "procurement@arcelik.com", "+90 212 314 34 34")
        };

        context.Customers.AddRange(customers);
    }

    private static async Task SeedProductsAndHistoriesAsync(AppDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        // Seed 3 products: HMI, Led Panel, LCD
        var hmi = Product.Create("HMI", 12500.00m);
        var ledPanel = Product.Create("Led Panel", 45000.00m);
        var lcd = Product.Create("LCD", 22000.00m);

        // Pre-populate last_request_price and last_request_date for demo data
        var seedDate = new DateTime(2025, 5, 15, 10, 0, 0, DateTimeKind.Utc);

        hmi.UpdateLastRequestPrice(11800.00m, seedDate);
        ledPanel.UpdateLastRequestPrice(43500.00m, seedDate.AddDays(-10));

        context.Products.AddRange(hmi, ledPanel, lcd);

        // Create a seed request to link price histories to
        if (!await context.Requests.AnyAsync())
        {
            var customers = context.Customers.Local.ToList();
            if (customers.Count == 0)
                customers = await context.Customers.ToListAsync();

            var seedRequest = Request.Create("RQ-20250515-001", customers[0].Id);

            seedRequest.AddItem(hmi.Id, 2, 11800.00m);
            seedRequest.AddItem(ledPanel.Id, 3, 43500.00m);
            seedRequest.AddItem(lcd.Id, 5, 21000.00m);
            seedRequest.MarkAsSent();

            context.Requests.Add(seedRequest);

            // Create 6 product_price_history rows
            var histories = new[]
            {
                ProductPriceHistory.Create(hmi.Id, seedRequest.Id, 11800.00m),
                ProductPriceHistory.Create(hmi.Id, seedRequest.Id, 12200.00m),
                ProductPriceHistory.Create(hmi.Id, seedRequest.Id, 12500.00m),
                ProductPriceHistory.Create(ledPanel.Id, seedRequest.Id, 43500.00m),
                ProductPriceHistory.Create(ledPanel.Id, seedRequest.Id, 44000.00m),
                ProductPriceHistory.Create(lcd.Id, seedRequest.Id, 21000.00m)
            };

            context.ProductPriceHistories.AddRange(histories);
        }
    }
}
