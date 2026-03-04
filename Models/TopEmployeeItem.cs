namespace IllnessesRecordingSystem.Models;

public class TopEmployeeItem
{
    public string FullName { get; set; }
    public int IllnessCount { get; set; }
    public string DisplayText => $"{FullName}. Случаев болезни - {IllnessCount}";
}