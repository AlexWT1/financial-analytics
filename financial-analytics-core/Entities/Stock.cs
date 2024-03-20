using FinancialAnalytics.Core.Extensions;

namespace FinancialAnalytics.Core.Entities;

public class Stock : IInstrument
{
    public Id Id { get; set; }
    public string Figi { get; set; }

    public string Name { get; set; }

    public Money LasPrice { get; set; }

    public DateTime LastUpdate { get; set; }

    public (Money Price, DateTime Date) History { get; set; }

    public string Ticker { get; set; }

    public Prognose Prognose { get; set; }
    

}