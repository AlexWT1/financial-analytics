namespace FinancialAnalytics.Core.Extensions;

public record Money(double Value, string Curency)
{
    public static implicit operator double(Money money) => money.Value;
}
