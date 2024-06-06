namespace FinancialAnalytics.Application.Portfolios;

public class PortfolioService(IRepository<Portfolio> repository) : IServise
{
    private IRepository<Portfolio> Repository { get; init; } = repository;

    public async Task<IEnumerable<PortfolioDTO>> GetPortfolios(CancellationToken cancellationToken = default) =>
       (await Repository.Get(cancellationToken)).Cast<PortfolioDTO>();

    public async Task CreatePortfolio(string newName, CancellationToken cancellationToken = default)
    {
        var potentialCopies = await Repository.GetWithoutTracking(x => x.Name.ToLower().Equals(newName.ToLower(), StringComparison.CurrentCultureIgnoreCase), cancellationToken);
        if (potentialCopies.Any())
            throw new SimilarPortfolioNameException(newName);
        await Repository.Add(new() { Name = newName.Trim() }, cancellationToken);
    }

    public async Task BuyInstruments(Guid portfolioId, InstrumentInPortfolio instrument, CancellationToken cancellationToken = default)
    {
        var portfolio = (await Repository.Get(x => x.Id.Value == portfolioId, cancellationToken)).FirstOrDefault() ??
            throw new PortfolioNotFoundException(portfolioId);

        var oldInstrument = portfolio.Instruments.FirstOrDefault(x => x.Instrument == instrument.Instrument);
        if (oldInstrument is null)
            portfolio.Instruments.Add(new()
            {
                Instrument = instrument.Instrument,
                Amount = instrument.Amount
            });
        else
            oldInstrument.Amount += instrument.Amount;

        await Repository.Update(portfolio, cancellationToken);
    }

    public async Task SellInstruments(Guid portfolioId, Guid instrumentId, InstrumentInPortfolio instrument, CancellationToken cancellationToken = default)
    {
        var portfolio = (await Repository.Get(x => x.Id.Value == portfolioId, cancellationToken)).FirstOrDefault() ??
            throw new PortfolioNotFoundException(portfolioId);

        var oldInstrument = portfolio.Instruments.FirstOrDefault(x => x.Instrument == instrument.Instrument) ??
            throw new InstrumentNotFoundException(instrumentId);

        if ((oldInstrument.Amount - instrument.Amount) >= 0)
            oldInstrument.Amount -= instrument.Amount;
        else
            throw new InstrumentAmountException(instrumentId, instrument.Amount);

        if (oldInstrument.Amount == 0)
            portfolio.Instruments.Remove(oldInstrument);

        await Repository.Update(portfolio, cancellationToken);
    }

}