using FinancialAnalytics.Application.Portfolios;
using FinancialAnalytics.Core;
using FinancialAnalytics.Core.Entities;
using FinancialAnalytics.Core.Exceptions;
using project_management_applicationtests;

namespace financial_analytics_applicatio_tests.Portfolios;

public class PortfolioServiceTests
{
    [TestCase("name1", "Name1")]
    [TestCase("naMe1", "Name1")]
    [TestCase("NamE1", "Name1")]
    public void CreatePortfolio_WhenSameName_ShouldThrowException(string name1, string name2)
    {
        // Arrange
        var portfolioRepo = new FakeRepository<Portfolio>();
        portfolioRepo.AddRange([new Portfolio() { Id = new Id(Guid.NewGuid()), Name = name1 }]);
        var portfolioService = new PortfolioService(portfolioRepo);

        // Act
        AsyncTestDelegate act = async delegate { Console.WriteLine("TEST !!!!! Delegate START");  await portfolioService.CreatePortfolio(name2); Console.WriteLine("TEST !!!!! Delegate FINISH"); };

        // Assert
        Assert.ThrowsAsync<SimilarPortfolioNameException>(act);
    }
}
