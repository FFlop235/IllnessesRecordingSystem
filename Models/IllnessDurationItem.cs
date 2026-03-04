namespace IllnessesRecordingSystem.Models;

public class IllnessDurationItem
{
    public string IllnessName { get; set; }
    public double AverageDays { get; set; }
    public string DisplayText => $"{IllnessName}: {AverageDays:F1} дня";
}