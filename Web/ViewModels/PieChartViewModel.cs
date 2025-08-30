namespace Web.ViewModels;

public class PieChartViewModel
{
    public string Title { get; set; } = string.Empty;
    public List<PieChartDataPoint> Data { get; set; } = new();
    public string Format { get; set; } = "currency"; // "currency" | "percentage" | "number"
}

public class PieChartDataPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; } = 0;
}