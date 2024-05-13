namespace FinancialAnalytics.Core.Exceptions;

public class PortfolioNotFoundException(Guid id) : Exception($"Portfolio {id} not found.")
{
}
