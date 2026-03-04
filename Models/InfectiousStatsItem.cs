namespace IllnessesRecordingSystem.Models;

public class InfectiousStatsItem
{
    public int InfectiousCount { get; set; }
    public int TotalCount { get; set; }
    public double Percentage => TotalCount > 0 ? (InfectiousCount * 100.0 / TotalCount) : 0;
    public string DisplayText => $"Инфекционные {Percentage:F0}% ({InfectiousCount} из {TotalCount})";
}