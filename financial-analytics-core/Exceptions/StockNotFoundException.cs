namespace FinancialAnalytics.Core.Exceptions;

public class StockNotFoundException(Guid id) : Exception($"There is no such stock - {id} my friend")
{
}
