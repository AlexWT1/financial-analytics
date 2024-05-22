using FinancialAnalytics.Core;

namespace project_management_applicationtests;

internal class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
{
    private readonly List<TEntity> _entities = [];

    //public Task AddRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
    //    Task.Run(() => _entities.AddRange(entities), cancellationToken);
    public Task AddRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _entities.AddRange(entities);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<TEntity>> Get(CancellationToken cancellationToken = default) =>
        Task.FromResult(_entities.AsEnumerable());

    public Task<IEnumerable<TEntity>> Get(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default) =>
        Task.FromResult(_entities.Where(predicate).ToArray().AsEnumerable());

    public Task<IEnumerable<TEntity>> GetWithoutTracking(CancellationToken cancellationToken = default) =>
        Get(cancellationToken);
    public Task<IEnumerable<TEntity>> GetWithoutTracking(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default) =>
        Get(predicate, cancellationToken);

    public Task RemoveRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
        Task.Run(() => _entities.RemoveAll(x => entities.Contains(x)));

    public Task UpdateRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
       Task.Run(() =>
       {
           foreach (var entity in entities)
           {
               var index = _entities.FindIndex(e => e.Id.Value == entity.Id.Value);
               if (index != -1)
               {
                   _entities[index] = entity;
               }
           }
       }, cancellationToken);
}