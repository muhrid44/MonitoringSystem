using Dapper;
using Microsoft.Data.Sqlite;
using Npgsql;
using System.Net.NetworkInformation;

namespace MonitoringSystem.Repository;

public static class DatabaseInitializer
{
    public static void Initialize(string connectionString)
    {
        var connection = new NpgsqlConnection(connectionString); 
        connection.Open();

        connection.Execute("""
        CREATE TABLE IF NOT EXISTS orders (
        id UUID PRIMARY KEY,
        created_at TIMESTAMP NOT NULL,
        status INTEGER NOT NULL,
        retry_count INTEGER NOT NULL DEFAULT 0
        );
        """);
    }
}
