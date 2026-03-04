using System;
using IllnessesRecordingSystem.Models;
using MySqlConnector;

namespace IllnessesRecordingSystem.DB;

public class SickDepartmentsReposiroty: StatisticBaseRepository<DepartmentsSickView>, IDisposable
{
    public SickDepartmentsReposiroty()
    {
        OpenConnection();
    }

    public override DepartmentsSickView Get()
    {
        var cmd = new MySqlCommand(@"
        SELECT 
            d.Name AS DepartmentName,
            SUM(DATEDIFF(ir.EndDate, ir.StartDate)) AS TotalSickDays,
            COUNT(ir.Id) AS IllnessCount,
            ROUND(AVG(DATEDIFF(ir.EndDate, ir.StartDate)), 1) AS AvgDuration
        FROM IllnessRecords ir
        JOIN Employees e ON ir.EmployeeId = e.Id
        JOIN Departments d ON e.DepartmentId = d.Id
        GROUP BY d.Id, d.Name
        ORDER BY TotalSickDays DESC
        LIMIT 5;
        ", connection);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new DepartmentsSickView
            {
                DepartmentName = reader.GetString("DepartmentName"),
                IllnessCount = reader.GetInt32("IllnessCount"),
                AvgDuration = reader.GetDouble("AvgDuration"),
                TotalSickDays = reader.GetInt32("TotalSickDays")
            };
        }

        return null;
    }

    public void Dispose()
    {
        CloseConnection();
        base.Dispose();
    }
}