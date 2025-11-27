using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading;

namespace MovieAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Retry logic
            int retries = 10;
            while (retries > 0)
            {
                try
                {
                    var builder = new SqlConnectionStringBuilder(connectionString);
                    var dbName = builder.InitialCatalog;
                    builder.InitialCatalog = "master"; // Connect to master to create DB

                    using (var connection = new SqlConnection(builder.ConnectionString))
                    {
                        connection.Open();
                        var checkDb = $"SELECT * FROM sys.databases WHERE name = '{dbName}'";
                        if (connection.QueryFirstOrDefault(checkDb) == null)
                        {
                            connection.Execute($"CREATE DATABASE [{dbName}]");
                            Console.WriteLine($"Database '{dbName}' created.");
                        }
                    }

                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        CreateSchema(connection);
                    }
                    return;
                }
                catch (SqlException)
                {
                    retries--;
                    Console.WriteLine($"Waiting for SQL Server... ({retries} left)");
                    Thread.Sleep(2000);
                }
            }
        }

        private static void CreateSchema(SqlConnection connection)
        {
            // 1. Independent Tables (No Foreign Keys)
            connection.Execute(@"
                IF OBJECT_ID('Users', 'U') IS NULL
                CREATE TABLE Users (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100),
                    Email NVARCHAR(255) UNIQUE,
                    PasswordHash NVARCHAR(MAX)
                )");

            connection.Execute(@"
                IF OBJECT_ID('Genres', 'U') IS NULL
                CREATE TABLE Genres (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100)
                )");

            connection.Execute(@"
                IF OBJECT_ID('Actors', 'U') IS NULL
                CREATE TABLE Actors (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100),
                    Bio NVARCHAR(MAX),
                    DOB DATETIME,
                    Gender INT
                )");

            connection.Execute(@"
                IF OBJECT_ID('Producers', 'U') IS NULL
                CREATE TABLE Producers (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100),
                    Bio NVARCHAR(MAX),
                    DOB DATETIME,
                    Gender INT
                )");

            // 2. Tables with Foreign Keys 
            connection.Execute(@"
                IF OBJECT_ID('Movies', 'U') IS NULL
                CREATE TABLE Movies (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(200),
                    YearOfRelease INT,
                    Plot NVARCHAR(MAX),
                    Poster NVARCHAR(MAX),
                    ProducerId INT,
                    FOREIGN KEY (ProducerId) REFERENCES Producers(Id)
                )");

            connection.Execute(@"
                IF OBJECT_ID('Reviews', 'U') IS NULL
                CREATE TABLE Reviews (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Message NVARCHAR(MAX),
                    MovieId INT,
                    FOREIGN KEY (MovieId) REFERENCES Movies(Id) ON DELETE CASCADE
                )");

            // 3. Many-to-Many Junction Tables
            connection.Execute(@"
                IF OBJECT_ID('MovieActors', 'U') IS NULL
                CREATE TABLE MovieActors (
                    MovieId INT,
                    ActorId INT,
                    PRIMARY KEY (MovieId, ActorId),
                    FOREIGN KEY (MovieId) REFERENCES Movies(Id) ON DELETE CASCADE,
                    FOREIGN KEY (ActorId) REFERENCES Actors(Id) ON DELETE CASCADE
                )");

            connection.Execute(@"
                IF OBJECT_ID('MovieGenres', 'U') IS NULL
                CREATE TABLE MovieGenres (
                    MovieId INT,
                    GenreId INT,
                    PRIMARY KEY (MovieId, GenreId),
                    FOREIGN KEY (MovieId) REFERENCES Movies(Id) ON DELETE CASCADE,
                    FOREIGN KEY (GenreId) REFERENCES Genres(Id) ON DELETE CASCADE
                )");

            Console.WriteLine("Cloud-Native Database Schema Initialized Successfully.");
        }
    }
}
