namespace FinancialAnalytics.Core.Exceptions;

public class InstrumentAmountException(Guid id, int amount) : Exception($"Instrument {id} can't sell in a given quantity {amount}")
{
}
