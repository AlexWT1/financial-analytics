namespace FinancialAnalytics.Application.Instruments;

public class StockService(IRepository<Stock> repository) : IServise
{
    private IRepository<Stock> Repository { get; init; } = repository;

    public async Task<IEnumerable<StockDTO>> GetStocks(CancellationToken cancellationToken = default) =>
       (await Repository.Get(cancellationToken)).Cast<StockDTO>();


    //public async Task<IEnumerable<StockDTO>> GetStocks(Guid id, CancellationToken cancellationToken = default)
    //{
    //    var potentialStock = (await Repository.GetWithoutTracking(x => x.Id.Value == id, cancellationToken)).FirstOrDefault() ??
    //       throw new StockNotFoundException(id);
    //    return {
    //        potentialStock =
    //    }
    //}

}
