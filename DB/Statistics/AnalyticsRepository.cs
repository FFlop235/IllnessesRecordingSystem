using System;
using System.Collections.Generic;
using IllnessesRecordingSystem.Models;
using MySqlConnector;

namespace IllnessesRecordingSystem.DB;

public class AnalyticsRepository: StatisticBaseRepository<object>, IDisposable
{
    public AnalyticsRepository()
    {
        OpenConnection();
    }

    public override object Get()
    {
        throw new NotImplementedException();
    }

    public List<TopIllnessItem> GetTopIllnesses(int count = 3)
    {
        var result = new List<TopIllnessItem>();

        var cmd = new MySqlCommand(@"
            SELECT
                it.Name AS IllnessName,
                COUNT(*) AS CaseCount
            FROM IllnessRecords ir
            JOIN IllnessTypes it ON ir.IllnessTypeId = it.Id
            GROUP BY it.Id, it.Name
            ORDER BY COUNT(*) DESC
            LIMIT @count", connection);
        
        cmd.Parameters.AddWithValue("@count", count);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new TopIllnessItem
            {
                IllnessName = reader.GetString("IllnessName"),
                CaseCount = reader.GetInt32("CaseCount")
            });
        }
        
        return result;
    }
    
        public List<IllnessDurationItem> GetAverageDurations(List<string> illnessNames)
    {
        var result = new List<IllnessDurationItem>();
        
        var placeholders = new List<string>();
        for (int i = 0; i < illnessNames.Count; i++)
            placeholders.Add($"@name{i}");
        
        var cmd = new MySqlCommand($@"
            SELECT 
                it.Name AS IllnessName,
                ROUND(AVG(DATEDIFF(ir.EndDate, ir.StartDate)), 1) AS AverageDays
            FROM IllnessRecords ir
            JOIN IllnessTypes it ON ir.IllnessTypeId = it.Id
            WHERE it.Name IN ({string.Join(", ", placeholders)})
            GROUP BY it.Id, it.Name", connection);
        
        for (int i = 0; i < illnessNames.Count; i++)
            cmd.Parameters.AddWithValue($"@name{i}", illnessNames[i]);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new IllnessDurationItem
            {
                IllnessName = reader.GetString("IllnessName"),
                AverageDays = reader.GetDouble("AverageDays")
            });
        }
        
        return result;
    }

    public List<DepartmentSickDaysItem> GetTopSickDepartments(int count = 1)
    {
        var result = new List<DepartmentSickDaysItem>();
        
        var cmd = new MySqlCommand(@"
            SELECT 
                d.Name AS DepartmentName,
                SUM(DATEDIFF(ir.EndDate, ir.StartDate)) AS TotalSickDays
            FROM IllnessRecords ir
            JOIN Employees e ON ir.EmployeeId = e.Id
            JOIN Departments d ON e.DepartmentId = d.Id
            GROUP BY d.Id, d.Name
            ORDER BY TotalSickDays DESC
            LIMIT @count", connection);
        
        cmd.Parameters.AddWithValue("@count", count);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new DepartmentSickDaysItem
            {
                DepartmentName = reader.GetString("DepartmentName"),
                TotalSickDays = reader.GetInt32("TotalSickDays")
            });
        }
        
        return result;
    }

    public InfectiousStatsItem GetInfectiousStats()
    {
        var cmd = new MySqlCommand(@"
            SELECT 
                SUM(CASE WHEN it.IsInfectious = 1 THEN 1 ELSE 0 END) AS InfectiousCount,
                COUNT(*) AS TotalCount
            FROM IllnessRecords ir
            JOIN IllnessTypes it ON ir.IllnessTypeId = it.Id", connection);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new InfectiousStatsItem
            {
                InfectiousCount = reader.GetInt32("InfectiousCount"),
                TotalCount = reader.GetInt32("TotalCount")
            };
        }
        
        return new InfectiousStatsItem { InfectiousCount = 0, TotalCount = 0 };
    }

    public TopEmployeeItem GetMostResilientEmployee()
    {
        var cmd = new MySqlCommand(@"
            SELECT 
                e.FullName,
                COUNT(ir.Id) AS IllnessCount
            FROM Employees e
            LEFT JOIN IllnessRecords ir ON e.Id = ir.EmployeeId
            GROUP BY e.Id, e.FullName
            HAVING COUNT(ir.Id) = 1
            ORDER BY COUNT(ir.Id) ASC
            LIMIT 1", connection);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new TopEmployeeItem
            {
                FullName = reader.GetString("FullName"),
                IllnessCount = reader.GetInt32("IllnessCount")
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