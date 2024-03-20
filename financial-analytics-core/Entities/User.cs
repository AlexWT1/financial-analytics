namespace FinancialAnalytics.Core.Entities;

public class User : IEntity
{
    public Id Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public virtual ICollection<Portfolio> Portfolios { get; set; }
}
