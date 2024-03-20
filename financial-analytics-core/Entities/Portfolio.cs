namespace FinancialAnalytics.Core.Entities;

public class Portfolio : IEntity
{
    public Id Id { get; set; }
    public virtual ICollection<IInstrument> Instruments { get; set; }
}
