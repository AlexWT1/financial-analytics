namespace FinancialAnalytics.Core.Exceptions;

public class SimilarPortfolioNameException(string newName) : Exception
{
    public override string Message => $"There is already portfolio name with {newName}";
}
