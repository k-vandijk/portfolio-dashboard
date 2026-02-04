namespace Dashboard._Web.ViewModels;

public class MarketHistoryMetricsCardViewModel
{
    public MetricViewModel CurrentPrice { get; set; } = null!;
    public MetricViewModel InterestEur { get; set; } = null!;
    public MetricViewModel InterestPct { get; set; } = null!;
}