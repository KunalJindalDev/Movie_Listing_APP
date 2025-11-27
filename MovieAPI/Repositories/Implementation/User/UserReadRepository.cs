using System;
using System.Text.Json;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Dapper;

namespace MovieApp.Repositories
{
    public class UserReadRepository : BaseRepository, IUserReadRepository
    {
        private readonly IDatabase _cache;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string ItemKeyPrefix = "User:";

        public UserReadRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public User GetByEmail(string email)
        {
            string key = ItemKeyPrefix + email;
            var cached = _cache.StringGet(key);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<User>(cached!, _jsonOptions);
            }

            using var connection = CreateConnection();
            string sql = "SELECT * FROM Users WHERE Email = @Email";
            var user = connection.QueryFirstOrDefault<User>(sql, new { Email = email });

            if (user != null)
            {
                _cache.StringSet(key, JsonSerializer.Serialize(user, _jsonOptions), TimeSpan.FromMinutes(10));
            }

            return user;
        }
    }
}
