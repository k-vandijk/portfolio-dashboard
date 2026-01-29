using Dashboard.Application.Dtos;
using Dashboard.Application.Helpers;

namespace Dashboard.Tests.Application.Helpers;

public class LineChartHelperTests
{
    // -------- CalculatePeriodDelta --------
    
    [Fact]
    public void CalculatePeriodDelta_ReturnsNull_WhenNoPoints()
    {
        // Arrange
        var points = new List<DataPointDto>();

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CalculatePeriodDelta_ReturnsZero_WhenSinglePointWithZeroValue()
    {
        // Arrange
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 0m }
        };

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.Value);
    }

    [Fact]
    public void CalculatePeriodDelta_ReturnsValueForSinglePoint()
    {
        // Arrange
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 100m }
        };

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.NotNull(result);
        // For a single point, delta is 0 (no change over time)
        // This is semantically correct: last - first = 100 - 100 = 0
        Assert.Equal(0m, result.Value);
    }

    [Fact]
    public void CalculatePeriodDelta_ReturnsPositiveDelta_WhenValuesIncrease()
    {
        // Arrange
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 100m },
            new DataPointDto { Label = "2025-01-02", Value = 150m },
            new DataPointDto { Label = "2025-01-03", Value = 200m }
        };

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100m, result.Value); // 200 - 100 = 100
    }

    [Fact]
    public void CalculatePeriodDelta_ReturnsNegativeDelta_WhenValuesDecrease()
    {
        // Arrange
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 200m },
            new DataPointDto { Label = "2025-01-02", Value = 150m },
            new DataPointDto { Label = "2025-01-03", Value = 100m }
        };

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(-100m, result.Value); // 100 - 200 = -100
    }

    [Fact]
    public void CalculatePeriodDelta_IgnoresMiddleValues()
    {
        // Arrange - middle values should not affect the delta
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 100m },
            new DataPointDto { Label = "2025-01-02", Value = 1000m }, // spike in the middle
            new DataPointDto { Label = "2025-01-03", Value = 200m }
        };

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100m, result.Value); // 200 - 100 = 100, ignoring the 1000 spike
    }

    [Fact]
    public void CalculatePeriodDelta_WorksWithNegativeValues()
    {
        // Arrange - profit can be negative (loss)
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = -50m },
            new DataPointDto { Label = "2025-01-02", Value = -30m },
            new DataPointDto { Label = "2025-01-03", Value = 20m }
        };

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(70m, result.Value); // 20 - (-50) = 70
    }

    [Fact]
    public void CalculatePeriodDelta_WorksWithDecimalPrecision()
    {
        // Arrange
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 123.45m },
            new DataPointDto { Label = "2025-01-02", Value = 234.56m }
        };

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(111.11m, result.Value); // 234.56 - 123.45 = 111.11
    }

    [Fact]
    public void CalculatePeriodDelta_WorksWithPercentageValues()
    {
        // Arrange - for profit-percentage mode
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 5.5m }, // 5.5%
            new DataPointDto { Label = "2025-01-02", Value = 10.25m } // 10.25%
        };

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4.75m, result.Value); // 10.25 - 5.5 = 4.75
    }

    [Fact]
    public void CalculatePeriodDelta_WorksWithLargeValues()
    {
        // Arrange - realistic portfolio values
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 50000.00m },
            new DataPointDto { Label = "2025-12-31", Value = 65000.00m }
        };

        // Act
        var result = LineChartHelper.CalculatePeriodDelta(points);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15000.00m, result.Value); // 65000 - 50000 = 15000
    }

    // -------- NormalizeSeries --------

    [Fact]
    public void NormalizeSeries_ReturnsEmptyList_WhenNoPoints()
    {
        // Arrange
        var points = new List<DataPointDto>();

        // Act
        var result = LineChartHelper.NormalizeSeries(points, "profit");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void NormalizeSeries_NormalizesToZero_ForProfitMode()
    {
        // Arrange
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 5000m },
            new DataPointDto { Label = "2025-01-15", Value = 5500m },
            new DataPointDto { Label = "2025-02-01", Value = 6000m }
        };

        // Act
        var result = LineChartHelper.NormalizeSeries(points, "profit");

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(0m, result[0].Value); // First point normalized to 0
        Assert.Equal(500m, result[1].Value); // 5500 - 5000
        Assert.Equal(1000m, result[2].Value); // 6000 - 5000
    }

    [Fact]
    public void NormalizeSeries_NormalizesToZero_ForProfitPercentageMode()
    {
        // Arrange
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 10.5m },
            new DataPointDto { Label = "2025-06-01", Value = 15.25m },
            new DataPointDto { Label = "2025-12-31", Value = 20.0m }
        };

        // Act
        var result = LineChartHelper.NormalizeSeries(points, "profit-percentage");

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(0m, result[0].Value); // First point normalized to 0
        Assert.Equal(4.75m, result[1].Value); // 15.25 - 10.5
        Assert.Equal(9.5m, result[2].Value); // 20.0 - 10.5
    }

    [Fact]
    public void NormalizeSeries_DoesNotNormalize_ForValueMode()
    {
        // Arrange
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 10000m },
            new DataPointDto { Label = "2025-06-01", Value = 12000m },
            new DataPointDto { Label = "2025-12-31", Value = 15000m }
        };

        // Act
        var result = LineChartHelper.NormalizeSeries(points, "value");

        // Assert - values should remain unchanged for value mode
        Assert.Equal(3, result.Count);
        Assert.Equal(10000m, result[0].Value);
        Assert.Equal(12000m, result[1].Value);
        Assert.Equal(15000m, result[2].Value);
    }

    [Fact]
    public void NormalizeSeries_HandlesNegativeStartingValue()
    {
        // Arrange - profit can start negative
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = -500m },
            new DataPointDto { Label = "2025-06-01", Value = 0m },
            new DataPointDto { Label = "2025-12-31", Value = 1000m }
        };

        // Act
        var result = LineChartHelper.NormalizeSeries(points, "profit");

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(0m, result[0].Value); // -500 - (-500) = 0
        Assert.Equal(500m, result[1].Value); // 0 - (-500) = 500
        Assert.Equal(1500m, result[2].Value); // 1000 - (-500) = 1500
    }

    [Fact]
    public void NormalizeSeries_PreservesLabels()
    {
        // Arrange
        var points = new List<DataPointDto>
        {
            new DataPointDto { Label = "2025-01-01", Value = 100m },
            new DataPointDto { Label = "2025-02-01", Value = 200m }
        };

        // Act
        var result = LineChartHelper.NormalizeSeries(points, "profit");

        // Assert
        Assert.Equal("2025-01-01", result[0].Label);
        Assert.Equal("2025-02-01", result[1].Label);
    }
}
