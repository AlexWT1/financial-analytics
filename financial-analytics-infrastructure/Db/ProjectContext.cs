using FinancialAnalytics.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace financial_analytics_infrastructure.Db;

internal class ProjectContext : DbContext
{
    class IdConverter():ValueConverter<Guid, Id>(x=>new Id(x), x=>x.Value);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entityTypes = typeof(IEntity).Assembly.GetTypes().Where(x => !x.IsInterface && !x.IsAbstract && typeof(IEntity).IsAssignableFrom(x));

        foreach (var entityType in entityTypes)
        {
            modelBuilder.Entity(entityType)
                .Property(nameof(IEntity.Id))
                .HasConversion<IdConverter>()
                .HasDefaultValueSql("newsequential1");
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies()
            .UseSqlServer("Server=db;Datebase=projectdb;User Id=sa,Password=Qwerty123!;");
        base.OnConfiguring(optionsBuilder);
    }
}
