using System;
using DotNetEnv;
using MySqlConnector;

namespace IllnessesRecordingSystem.DB;

public abstract class StatisticBaseRepository<T>: IDisposable, SRepository<T> where T: class
{
    public abstract T Get();
    
    protected MySqlConnection connection;

    public StatisticBaseRepository()
    {
        Env.Load();
        MySqlConnectionStringBuilder builder = new();
        builder.CharacterSet = "utf8";
        builder.Server = Environment.GetEnvironmentVariable("DB_HOST");
        builder.Port = Convert.ToUInt32(Environment.GetEnvironmentVariable("DB_PORT"));
        builder.Database = Environment.GetEnvironmentVariable("DB_NAME");
        builder.UserID = Environment.GetEnvironmentVariable("DB_USER");
        builder.Password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        
        connection = new MySqlConnection(builder.ConnectionString);
    }
    
    public bool OpenConnection()
    {
        try
        {
            connection.Open();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public bool CloseConnection()
    {
        try
        {
            connection.Close();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public bool ExecuteNonQuery(string query)
    {
        using var cmd = new MySqlCommand(query, connection);
        return cmd.ExecuteNonQuery() > 0;
    }

    public void Dispose()
    {
        connection.Dispose();
    }
}