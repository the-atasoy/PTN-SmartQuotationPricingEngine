using Application.Interfaces;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditableEntityInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var userId = _currentUserService.UserId;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (userId.HasValue)
                {
                    entry.Entity.SetCreator(userId.Value);
                    entry.Entity.SetUpdater(userId.Value);
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (userId.HasValue)
                {
                    entry.Entity.SetUpdater(userId.Value);
                }
                else
                {
                    entry.Property(e => e.UpdatedAt).CurrentValue = DateTime.UtcNow;
                }
            }
            else if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.Delete();

                if (userId.HasValue)
                {
                    entry.Entity.SetUpdater(userId.Value);
                }
                else
                {
                    entry.Property(e => e.UpdatedAt).CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}
