using Microsoft.Extensions.Localization;

namespace Dashboard._Web.ViewModels;

public sealed class MarketHistoryFiltersContentViewModel
{
    public required IStringLocalizer SharedLocalizer { get; init; }
    public required IEnumerable<string> Tickers { get; init; }
    public string? CurrentTicker { get; init; }
    public required string SystemUrlMarketHistory { get; init; }
}