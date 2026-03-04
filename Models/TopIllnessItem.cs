namespace IllnessesRecordingSystem.Models;

public class TopIllnessItem
{
    public string IllnessName { get; set; }
    public int CaseCount { get; set; }
    public string DisplayText => $"{CaseCount} случаев - {IllnessName}";
}