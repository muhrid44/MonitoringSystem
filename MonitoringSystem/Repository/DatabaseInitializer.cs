using Dapper;
using Microsoft.Data.Sqlite;

namespace MonitoringSystem.Repository;

public static class DatabaseInitializer
{
    public static void Initialize(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        connection.Execute("""
        CREATE TABLE IF NOT EXISTS Orders (
            Id TEXT PRIMARY KEY,
            CreatedAt TEXT NOT NULL,
            Status INTEGER NOT NULL
        );
        """);
    }
}
