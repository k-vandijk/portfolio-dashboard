using Dashboard.Application.Dtos;

namespace Dashboard.Application.Helpers;

public static class LineChartHelper
{
    /// <summary>
    /// Calculate the delta (change) in values over a period
    /// </summary>
    /// <param name="points">Data points representing the time series</param>
    /// <returns>The difference between the last and first data point, or null if no points exist</returns>
    public static decimal? CalculatePeriodDelta(List<DataPointDto> points)
    {
        if (points.Count == 0) return null;

        // For all modes, show the delta (change) during the filtered period
        // This represents the profit gained or value change during the period
        return points[^1].Value - points[0].Value;
    }

    /// <summary>
    /// Normalize profit series to start at zero
    /// This is useful for filtered time ranges to show profit growth relative to the start of the period
    /// </summary>
    /// <param name="points">Data points to normalize</param>
    /// <param name="mode">Chart mode (profit, profit-percentage, value)</param>
    /// <returns>Normalized data points</returns>
    public static List<DataPointDto> NormalizeSeries(IReadOnlyList<DataPointDto> points, string? mode)
    {
        // For profit modes, normalize to start at zero
        // This shows profit growth relative to the start of the filtered period
        if (mode == "profit" || mode == "profit-percentage")
        {
            var first = points.FirstOrDefault()?.Value ?? 0m;

            return points.Select(p => new DataPointDto
            {
                Label = p.Label,
                Value = p.Value - first
            }).ToList();
        }

        return points.ToList();
    }
}
