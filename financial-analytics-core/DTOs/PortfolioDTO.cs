using FinancialAnalytics.Core.Entities;

namespace FinancialAnalytics.Core.DTOs;

public class PortfolioDTO
{
    public Id Id { get; set; }
    public string Name { get; set; }

    public static implicit operator PortfolioDTO(Portfolio other) =>
       new()
       {
           Id = other.Id,
           Name = other.Name
       };
}