namespace FinancialAnalytics.Core.Entities;

public class InstrumentInPortfolio : IEntity
{
    public Id Id { get; set; }
    public virtual IInstrument Instrument { get; set; }
    public int Amount { get; set; }
    public virtual Portfolio Portfolio { get; set; }
}
