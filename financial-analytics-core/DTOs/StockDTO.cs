using FinancialAnalytics.Core.Entities;
using FinancialAnalytics.Core.Extensions;

namespace FinancialAnalytics.Core.DTOs;

public class StockDTO
{
    public string Figi { get; set; }

    public string Name { get; set; }
    public Money LastPrice { get; set; }

    public string Ticker { get; set; }

    public Prognose Prognose { get; set; }

    public static implicit operator StockDTO(Stock other) =>
       new()
       {
           Figi = other.Figi,
           Name = other.Name,
           LastPrice = other.LastPrice,
           Ticker = other.Ticker,
           Prognose = other.Prognose
       };
}
