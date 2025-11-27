using System;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Dapper;

namespace MovieApp.Repositories
{
    public class UserWriteRepository : BaseRepository, IUserWriteRepository
    {
        private readonly IDatabase _cache;
        private const string ItemKeyPrefix = "User:";

        public UserWriteRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
        }

        public void Create(User user)
        {
            using var connection = CreateConnection();
            string sql = @"
INSERT INTO Users (Name, Email, PasswordHash)
VALUES (@Name, @Email, @PasswordHash)";

            connection.Execute(sql, new
            {
                user.Name,
                user.Email,
                user.PasswordHash
            });

            // Invalidate cache for this user email
            _cache.KeyDelete(ItemKeyPrefix + user.Email);
        }
    }
}
