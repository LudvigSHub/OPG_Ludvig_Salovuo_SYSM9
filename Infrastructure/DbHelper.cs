using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Infrastructure
{
    public static class DbHelper
    {
        // Sökväg till databasen
        // lägger till databasens filplats i applikationens basmapp
        private static readonly string _dbPath = 
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookmaster.db");

        // Connection string för SQLite-databasen
        // anger datakällan och aktiverar foreign key-stöd
        // Denna säger typ "var ligger databasen och använd foreign keys"
        private static readonly string _connectionString =
            $"Data Source= {_dbPath};Foreign Keys=True;";

        // Metod för att skapa och öppna en SQLite-anslutning
        // Detta är "dörröppnaren" till databasen
        // Denna metod används varje gång databasen ska nås
        public static SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public static void Initialize()
        {
            using var connection = GetConnection();
            using var command = connection.CreateCommand();

            command.CommandText = @"
            PRAGMA foreign_keys = ON;
            CREATE TABLE IF NOT EXISTS Users (
                UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Country TEXT NOT NULL,
                SecurityQuestion TEXT,
                SecurityAnswer TEXT,
                IsAdmin INTEGER NOT NULL DEFAULT 0,
                CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
            );


             ";            
            command.ExecuteNonQuery();
        }
    }
}
