using FinancialAnalytics.Core.Extensions;

namespace FinancialAnalytics.Core.Entities;

public class Stock : IInstrument
{
    public Id Id { get; set; }
    public string Figi { get; set; }

    public string Name { get; set; }

    public Money LastPrice { get; set; }

    public DateTime LastUpdate { get; set; }

    public (Money Price, DateTime Date)[] History { get; set; }

    public double Coeff
    {
        get
        {
            if (History.Length <= 31)
                return 0;

            double[] fn = [.. Enumerable.Range(-30, Math.Min(30, History.Length - 30))
                                        .Select(i => History[..^i]
                                        .Take(30)
                                        .Select(x => (double)x.Price)
                                        .Average())];

            double[] diff = [.. Enumerable.Range(1, fn.Length - 1)
                                           .Select(i => fn[i] - fn[i - 1])];

            return diff.Average();
        }
    }
   


    public string Ticker { get; set; }

    public Prognose Prognose { get; set; }
    

}