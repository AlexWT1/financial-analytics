namespace FinancialAnalytics.Core.Entities;

public class Portfolio : IEntity
{
    public Id Id { get; set; }
    public string Name { get; set; }
    
    public virtual User User { get; set; }
    public virtual ICollection<InstrumentInPortfolio> Instruments { get; set; }
}
