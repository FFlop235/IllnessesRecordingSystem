namespace IllnessesRecordingSystem.Models;

public class DepartmentsSickView
{
    public string DepartmentName { get; set; }
    public int TotalSickDays { get; set; }
    public int IllnessCount { get; set; }
    public double AvgDuration { get; set; }
}