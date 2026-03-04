namespace IllnessesRecordingSystem.Models;

public class DepartmentSickDaysItem
{
    public string DepartmentName { get; set; }
    public int TotalSickDays { get; set; }
    public string DisplayText => $"{DepartmentName} - {TotalSickDays} дней болезни";
}