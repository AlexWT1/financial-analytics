using FinancialAnalytics.Core.Extensions;

namespace FinancialAnalytics.Core;

public record Id(Guid Value);
public interface IEntity
{
    Id Id { get; }
}

public interface IInstrument : IEntity
{
    string Figi {  get; }
    string Name {  get; }
    Money LasPrice { get; }
    DateTime LastUpdate { get; }
    (Money Price, DateTime Date) History { get; }
}