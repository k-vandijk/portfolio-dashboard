namespace Dashboard._Web.ViewModels;

public class DashboardViewModel
{
    public List<DashboardTableRowViewModel> TableRows { get; set; } = new();
    public LineChartViewModel LineChart { get; set; } = new();
    public int[] Years { get; set; } = [];
}
