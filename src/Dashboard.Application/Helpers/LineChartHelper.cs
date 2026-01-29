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
}
