using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers;

public class InvestmentController : Controller
{
    private readonly AzureTableService _service;

    public InvestmentController(AzureTableService service)
    {
        _service = service;
    }

    [HttpGet("/investment")]
    public IActionResult Investment([FromQuery] string? tickers, [FromQuery] string? dateRange)
    {
        var transactions = _service.GetTransactions();
        var filteredTransactions = FilterTransactions(transactions, tickers, dateRange);

        var pieChartViewModel = GetPieChartViewModel(filteredTransactions);
        var barChartViewModel = GetBarChartViewModel(filteredTransactions);
        var lineChartViewModel = GetLineChartViewModel(filteredTransactions);

        var viewModel = new InvestmentViewModel
        {
            PieChart = pieChartViewModel,
            BarChart = barChartViewModel,
            LineChart = lineChartViewModel
        };

        return View(viewModel);
    }

    private List<Transaction> FilterTransactions(List<Transaction> transactions, string? tickers, string? dateRange)
    {
        // Filter by multiple tickers (e.g. "sxrv.de,btceur")
        if (!string.IsNullOrWhiteSpace(tickers))
        {
            var tickerList = tickers
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.ToUpperInvariant())
                .ToList();

            transactions = transactions
                .Where(t => !string.IsNullOrWhiteSpace(t.Ticker) && tickerList.Contains(t.Ticker.ToUpperInvariant()))
                .ToList();
        }

        // Filter by date range (e.g. "2024-06-06,2025-06-06")
        if (!string.IsNullOrWhiteSpace(dateRange))
        {
            var dates = dateRange
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (dates.Length == 2 &&
                DateOnly.TryParse(dates[0], out var startDate) &&
                DateOnly.TryParse(dates[1], out var endDate))
            {
                transactions = transactions
                    .Where(t => t.Date >= startDate && t.Date <= endDate)
                    .ToList();
            }
        }

        return transactions;
    }

    private LineChartViewModel GetLineChartViewModel(List<Transaction> transactions)
    {
        var cumulativeSum = 0m;
        var groupedTransactions = transactions
            .GroupBy(t => t.Date)
            .Select(g =>
            {
                cumulativeSum += g.Sum(t => t.TotalCosts);
                return new LineChartDataPoint
                {
                    Label = g.Key.ToString("yyyy-MM-dd"),
                    Value = cumulativeSum
                };
            })
            .OrderBy(dp => dp.Label)
            .ToList();

        var lineChartViewModel = new LineChartViewModel
        {
            Title = "Investment (cumulative) per month",
            DataPoints = groupedTransactions,
            Format = "currency"
        };

        return lineChartViewModel;
    }

    private PieChartViewModel GetPieChartViewModel(List<Transaction> transactions)
    {
        var groupedTransactions = transactions
            .GroupBy(t => t.Ticker)
            .Select(g => new PieChartDataPoint
            {
                Label = g.Key,
                Value = g.Sum(t => t.TotalCosts)
            })
            .ToList();

        var pieChartViewModel = new PieChartViewModel
        {
            Title = "Investment per ticker",
            Data = groupedTransactions
        };

        return pieChartViewModel;
    }

    private BarChartViewModel GetBarChartViewModel(List<Transaction> transactions)
    {
        var groupedTransactions = transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => new BarChartDataPoint
            {
                Label = $"{g.Key.Year}-{g.Key.Month:D2}",
                Value = g.Sum(t => t.TotalCosts)
            })
            .OrderBy(dp => dp.Label)
            .ToList();

        var barChartViewModel = new BarChartViewModel
        {
            Title = "Investment per month",
            DataPoints = groupedTransactions
        };

        return barChartViewModel;
    }
}