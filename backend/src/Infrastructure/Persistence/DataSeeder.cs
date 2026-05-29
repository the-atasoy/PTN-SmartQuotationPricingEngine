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
            Customer.Create("Arçelik A.Ş.", "procurement@arcelik.com", "+90 212 314 34 34"),
            Customer.Create("Piton Technology User", "user@piton.com.tr", "+90 222 222 22 22")
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

        hmi.UpdateLastRequestPrice(11800.00m, Currency.TRY, seedDate);
        ledPanel.UpdateLastRequestPrice(43500.00m, Currency.TRY, seedDate.AddDays(-10));

        context.Products.AddRange(hmi, ledPanel, lcd);

        // Create a seed request to link price histories to
        if (!await context.Requests.AnyAsync())
        {
            var customers = context.Customers.Local.ToList();
            if (customers.Count == 0)
                customers = await context.Customers.ToListAsync();

            var seedRequest = Request.Create("RQ-20250515-001", customers[0].Id, Currency.TRY);

            seedRequest.AddItem(hmi.Id, 2, 11800.00m);
            seedRequest.AddItem(ledPanel.Id, 3, 43500.00m);
            seedRequest.AddItem(lcd.Id, 5, 21000.00m);
            seedRequest.MarkAsSent();

            context.Requests.Add(seedRequest);

            // Create 6 product_price_history rows
            var histories = new[]
            {
                ProductPriceHistory.Create(hmi.Id, seedRequest.Id, 11800.00m, Currency.TRY),
                ProductPriceHistory.Create(hmi.Id, seedRequest.Id, 12200.00m, Currency.TRY),
                ProductPriceHistory.Create(hmi.Id, seedRequest.Id, 12500.00m, Currency.TRY),
                ProductPriceHistory.Create(ledPanel.Id, seedRequest.Id, 43500.00m, Currency.TRY),
                ProductPriceHistory.Create(ledPanel.Id, seedRequest.Id, 44000.00m, Currency.TRY),
                ProductPriceHistory.Create(lcd.Id, seedRequest.Id, 21000.00m, Currency.TRY)
            };

            context.ProductPriceHistories.AddRange(histories);
            
            // Create 20 more dummy requests to simulate pagination
            var random = new Random(42);
            for (int i = 2; i <= 21; i++)
            {
                var reqDate = seedDate.AddDays(i);
                var reqDateStr = reqDate.ToString("yyyyMMdd");
                var c = customers[random.Next(customers.Count)];
                
                var dummyReq = Request.Create($"RQ-{reqDateStr}-{i:D3}", c.Id, Currency.TRY);
                
                // Add some items
                dummyReq.AddItem(hmi.Id, random.Next(1, 10), 0);
                dummyReq.AddItem(lcd.Id, random.Next(1, 5), 0);
                
                // Set custom CreatedAt
                typeof(Domain.Common.BaseEntity).GetProperty("CreatedAt")?.SetValue(dummyReq, reqDate);
                
                // Set status for some requests
                if (i % 3 == 0)
                {
                    dummyReq.UpdateItem(dummyReq.Items.First().Id, dummyReq.Items.First().Quantity, random.Next(10000, 20000));
                    dummyReq.MarkAsSent();
                }
                else if (i % 5 == 0)
                {
                    dummyReq.Cancel();
                }
                
                context.Requests.Add(dummyReq);
            }
        }
    }
}
