using FinancialAnalytics.Core;
using Microsoft.EntityFrameworkCore;
using System;

namespace financial_analytics_infrastructure.Db;

public class BasicRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    private static readonly ProjectContext _context = new();

    public async Task AddRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync();
    }
       
    public Task<IEnumerable<TEntity>> Get(CancellationToken cancellationToken) =>
        Task.FromResult(_context.Set<TEntity>().ToArray().AsEnumerable());
    public Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> predicate, CancellationToken cancellationToken) =>
        Task.FromResult(_context.Set<TEntity>().Where(predicate).ToArray().AsEnumerable());

    public Task<IEnumerable<TEntity>> GetWithoutTracking(CancellationToken cancellationToken) =>
        Task.FromResult(_context.Set<TEntity>().AsNoTracking().ToArray().AsEnumerable());

    public Task<IEnumerable<TEntity>> GetWithoutTracking(Func<TEntity, bool> predicate, CancellationToken cancellationToken) =>
        Task.FromResult(_context.Set<TEntity>().AsNoTracking().Where(predicate).ToArray().AsEnumerable());

    public Task RemoveRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        Task.Run(() =>
        {
            _context.RemoveRange(entities);
            _context.SaveChanges();

        }, cancellationToken);


    public Task UpdateRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
         Task.Run(() =>
         {
             _context.UpdateRange(entities);
             _context.SaveChanges();
         }, cancellationToken);
}
