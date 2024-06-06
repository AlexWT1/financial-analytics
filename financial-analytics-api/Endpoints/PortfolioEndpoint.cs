using FinancialAnalytics.Application.Portfolios;
using FinancialAnalytics.Core.DTOs;
using FinancialAnalytics.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace financial_analytics_api.Endpoints;

public static class PortfolioEndpoint
{
    public static void MapPortfolioEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("api/portfolio/create", CreatePortfolio).WithTags("Portfolio");
        routes.MapGet("api/portfolios", GetPortfolios).WithTags("Portfolio");

    }

    [HttpPost]
    public static async Task<IResult> CreatePortfolio(PortfolioDTO portfolio, PortfolioService service, HttpContext context, ILogger logger)
    {
        try
        {
            await service.CreatePortfolio(portfolio.Name);
            return Results.Created($"api/portfolio/{portfolio.Id}",portfolio);
        }
        catch (SimilarPortfolioNameException ex)
        {
            logger.LogError(ex, "Failed to create portfolio due to similar name.");
            return Results.BadRequest($"A portfolio with the name '{portfolio.Name}' already exists.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the portfolio.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    [HttpGet]
    public static async Task<IResult> GetPortfolios(PortfolioService service, ILogger logger)
    {
        try
        {
            var portfolios = await service.GetPortfolios();
            return Results.Ok(portfolios);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting the portfolios.");
            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
