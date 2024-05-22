using FinancialAnalytics.Application.Portfolios;
using FinancialAnalytics.Core;
using FinancialAnalytics.Core.Entities;
using FinancialAnalytics.Core.Exceptions;
using FinancialAnalytics.Core.Extensions;
using project_management_applicationtests;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

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
        AsyncTestDelegate act = async () => await portfolioService.CreatePortfolio(name2);

        // Assert
        Assert.ThrowsAsync<SimilarPortfolioNameException>(act);
    }

    [Test]
    public async Task BuyInstruments_WhenNewInstrument_ShouldAddInstrument()
    {
        // Arrange
        var portfolioRepo = new FakeRepository<Portfolio>();
        var portfolioId = Guid.NewGuid();
        portfolioRepo.AddRange([new Portfolio { Id = new Id(portfolioId), Instruments = [] }]);
        var portfolioService = new PortfolioService(portfolioRepo);

        var newStock = new Stock
        {
            Id = new Id(Guid.NewGuid()),
            Figi = "BBG000B9XRY4",
            Name = "Stock1",
            LastPrice = new Money (100, "USD"),
            LastUpdate = DateTime.UtcNow,
            History =
                [
                    (new Money(95, "USD"), DateTime.UtcNow.AddDays(-1)),
                    (new Money(90, "USD"), DateTime.UtcNow.AddDays(-2)),
                    (new Money(90, "USD"), DateTime.UtcNow.AddDays(-3)),
                 ],
            Ticker = "STK1",
            Prognose = new Prognose {}
        };

        var newInstrument = new InstrumentInPortfolio
        {
            Instrument = newStock,
            Amount = 10
        };

        // Act
        AsyncTestDelegate act = async () => await portfolioService.BuyInstruments(portfolioId, newInstrument);
        await act.Invoke();

        // Assert
        var updatedPortfolio = (await portfolioRepo.Get(x => x.Id.Value == portfolioId)).FirstOrDefault();
        Assert.IsNotNull(updatedPortfolio);
        Assert.That(updatedPortfolio.Instruments.Count, Is.EqualTo(1));
        var addedInstrument = updatedPortfolio.Instruments.First();
        Assert.That(addedInstrument.Instrument.Name, Is.EqualTo(newInstrument.Instrument.Name));
        Assert.That(addedInstrument.Amount, Is.EqualTo(newInstrument.Amount));
        Assert.That(((Stock)addedInstrument.Instrument).Figi, Is.EqualTo(newStock.Figi));
        Assert.That(((Stock)addedInstrument.Instrument).LastPrice.Value, Is.EqualTo(newStock.LastPrice.Value));
        Assert.That(((Stock)addedInstrument.Instrument).LastPrice.Curency, Is.EqualTo(newStock.LastPrice.Curency));
        Assert.That(((Stock)addedInstrument.Instrument).Ticker, Is.EqualTo(newStock.Ticker));
    }


    [Test]
    public async Task BuyInstruments_WhenExistingInstrument_ShouldIncreaseAmount()
    {
        // Arrange
        var portfolioRepo = new FakeRepository<Portfolio>();
        var portfolioId = Guid.NewGuid();
        var instrumentId = Guid.NewGuid();
        var existingStock = new Stock
        {
            Id = new Id(instrumentId),
            Figi = "BBG000B9XRY4",
            Name = "Stock1",
            LastPrice = new Money(100, "USD"),
            LastUpdate = DateTime.UtcNow,
            History =
                [
                    (new Money(95, "USD"), DateTime.UtcNow.AddDays(-1)),
                    (new Money(90, "USD"), DateTime.UtcNow.AddDays(-2)),
                    (new Money(89, "USD"), DateTime.UtcNow.AddDays(-3))
                ],
            Ticker = "STK1",
            Prognose = new Prognose {}
        };

        var existingInstrument = new InstrumentInPortfolio
        {
            Instrument = existingStock,
            Amount = 10
        };
        portfolioRepo.AddRange([new Portfolio { Id = new Id(portfolioId), Instruments = [existingInstrument] }]);
        var initialExistingAmount = existingInstrument.Amount;
        var portfolioService = new PortfolioService(portfolioRepo);

        var additionalInstrument = new InstrumentInPortfolio
        {
            Instrument = existingInstrument.Instrument,
            Amount = 5
        };

        // Act
        AsyncTestDelegate act = async () => await portfolioService.BuyInstruments(portfolioId, additionalInstrument);
        await act.Invoke();

        // Assert
        var updatedPortfolio = (await portfolioRepo.Get(x => x.Id.Value == portfolioId)).FirstOrDefault();
        Assert.IsNotNull(updatedPortfolio);
        Assert.That(updatedPortfolio.Instruments.Count, Is.EqualTo(1));
        var updatedInstrument = updatedPortfolio.Instruments.First();
        Assert.That(updatedInstrument.Amount, Is.EqualTo(initialExistingAmount + additionalInstrument.Amount));
        Assert.That(updatedInstrument.Instrument.Name, Is.EqualTo(existingStock.Name));
        Assert.That(((Stock)updatedInstrument.Instrument).Figi, Is.EqualTo(existingStock.Figi));
        Assert.That(((Stock)updatedInstrument.Instrument).LastPrice.Value, Is.EqualTo(existingStock.LastPrice.Value));
        Assert.That(((Stock)updatedInstrument.Instrument).LastPrice.Curency, Is.EqualTo(existingStock.LastPrice.Curency));
        Assert.That(((Stock)updatedInstrument.Instrument).Ticker, Is.EqualTo(existingStock.Ticker));
    }


    [Test]
    public void BuyInstruments_WhenPortfolioNotFound_ShouldThrowException()
    {
        // Arrange
        var portfolioRepo = new FakeRepository<Portfolio>();
        var portfolioService = new PortfolioService(portfolioRepo);
        var nonExistentPortfolioId = Guid.NewGuid();
        var newStock = new Stock
        {
            Id = new Id(Guid.NewGuid()),
            Figi = "BBG000B9XRY4",
            Name = "Stock1",
            LastPrice = new Money(100, "USD"),
            LastUpdate = DateTime.UtcNow,
            History = new[]
                {
                    (new Money(95, "USD"), DateTime.UtcNow.AddDays(-1)),
                    (new Money(90, "USD"), DateTime.UtcNow.AddDays(-2)),
                    (new Money(89, "USD"), DateTime.UtcNow.AddDays(-3))
                },
            Ticker = "STK1",
            Prognose = new Prognose { }
        };
        var newInstrument = new InstrumentInPortfolio { Instrument = newStock, Amount = 10 };

        // Act
        AsyncTestDelegate act = async () => await portfolioService.BuyInstruments(nonExistentPortfolioId, newInstrument);

        // Assert
        Assert.ThrowsAsync<PortfolioNotFoundException>(act);
    }


    [Test]
    public async Task SellInstruments_WhenSellingPartialAmount_ShouldDecreaseAmount()
    {
        // Arrange
        var portfolioRepo = new FakeRepository<Portfolio>();
        var portfolioId = Guid.NewGuid();
        var instrumentId = Guid.NewGuid();
        var existingInstrument = new InstrumentInPortfolio
        {
            Instrument = new Stock
            {
                Id = new Id(instrumentId),
                Name = "Stock1",
                Figi = "BBG000B9XRY4",
                LastPrice = new Money(100, "USD"),
                LastUpdate = DateTime.UtcNow,
                History = [(new Money(95, "USD"), DateTime.UtcNow.AddDays(-1)), (new Money(96, "USD"), DateTime.UtcNow.AddDays(-2))],
                Ticker = "STK1",
                Prognose = new Prognose {  }
            },
            Amount = 10
        };
        portfolioRepo.AddRange([new Portfolio { Id = new Id(portfolioId), Instruments = [existingInstrument] }]);
        var initialExistingAmount = existingInstrument.Amount;
        var portfolioService = new PortfolioService(portfolioRepo);

        var sellAmount = 5;

        // Act
        AsyncTestDelegate act = async () => await portfolioService.SellInstruments(portfolioId, instrumentId, new InstrumentInPortfolio { Instrument = existingInstrument.Instrument, Amount = sellAmount });
        await act.Invoke();

        // Assert
        var updatedPortfolio = (await portfolioRepo.Get(x => x.Id.Value == portfolioId)).FirstOrDefault();
        Assert.IsNotNull(updatedPortfolio);
        Assert.That(updatedPortfolio.Instruments.Count, Is.EqualTo(1));
        Assert.That(updatedPortfolio.Instruments.First().Amount, Is.EqualTo(initialExistingAmount - sellAmount));

        var updatedInstrument = updatedPortfolio.Instruments.First();
        Assert.That(((Stock)updatedInstrument.Instrument).Figi, Is.EqualTo("BBG000B9XRY4"));
        Assert.That(((Stock)updatedInstrument.Instrument).LastPrice.Value, Is.EqualTo(100));
        Assert.That(((Stock)updatedInstrument.Instrument).LastPrice.Curency, Is.EqualTo("USD"));
        Assert.That(((Stock)updatedInstrument.Instrument).Ticker, Is.EqualTo("STK1"));
    }


    [Test]
    public async Task SellInstruments_WhenSellingEntireAmount_ShouldRemoveInstrument()
    {
        // Arrange
        var portfolioRepo = new FakeRepository<Portfolio>();
        var portfolioId = Guid.NewGuid();
        var instrumentId = Guid.NewGuid();
        var existingInstrument = new InstrumentInPortfolio
        {
            Instrument = new Stock
            {
                Id = new Id(instrumentId),
                Name = "Stock1",
                Figi = "BBG000B9XRY4",
                LastPrice = new Money(100, "USD"),
                LastUpdate = DateTime.UtcNow,
                History = [(new Money(96, "USD"), DateTime.UtcNow.AddDays(-1)) , (new Money(95, "USD"), DateTime.UtcNow.AddDays(-2))],
                Ticker = "STK1",
                Prognose = new Prognose { }
            },
            Amount = 10
        };
        portfolioRepo.AddRange([new Portfolio { Id = new Id(portfolioId), Instruments = [existingInstrument] }]);
        var portfolioService = new PortfolioService(portfolioRepo);

        // Act
        AsyncTestDelegate act = async () => await portfolioService.SellInstruments(portfolioId, instrumentId, existingInstrument);
        await act.Invoke();

        // Assert
        var updatedPortfolio = (await portfolioRepo.Get(x => x.Id.Value == portfolioId)).FirstOrDefault();
        Assert.IsNotNull(updatedPortfolio);
        Assert.That(updatedPortfolio.Instruments, Is.Empty);
    }


    [Test]
    public void SellInstruments_WhenSellingExceedsAmount_ShouldThrowException()
    {
        // Arrange
        var portfolioRepo = new FakeRepository<Portfolio>();
        var portfolioId = Guid.NewGuid();
        var instrumentId = Guid.NewGuid();
        var existingInstrument = new InstrumentInPortfolio
        {
            Instrument = new Stock
            {
                Id = new Id(instrumentId),
                Name = "Stock1",
                Figi = "BBG000B9XRY4",
                LastPrice = new Money(100, "USD"),
                LastUpdate = DateTime.UtcNow,
                History = [(new Money(95, "USD"), DateTime.UtcNow.AddDays(-1)), (new Money(96, "USD"), DateTime.UtcNow.AddDays(-2))],
                Ticker = "STK1",
                Prognose = new Prognose {  }
            },
            Amount = 10
        };
        portfolioRepo.AddRange([new Portfolio { Id = new Id(portfolioId), Instruments = [existingInstrument] }]);
        var portfolioService = new PortfolioService(portfolioRepo);

        var sellAmount = 15;

        // Act
        AsyncTestDelegate act = async () => await portfolioService.SellInstruments(portfolioId, instrumentId, new InstrumentInPortfolio { Instrument = existingInstrument.Instrument, Amount = sellAmount });

        // Assert
        Assert.ThrowsAsync<InstrumentAmountException>(act);
    }

}
