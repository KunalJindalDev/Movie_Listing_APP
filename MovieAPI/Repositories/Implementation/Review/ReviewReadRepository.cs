using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MovieApp.Repositories
{
    public class ReviewReadRepository : BaseRepository, IReviewReadRepository
    {
        private readonly IDatabase _cache;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string AllKey = "Reviews:All";
        private const string ItemKeyPrefix = "Review:";

        public ReviewReadRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public IList<Review> GetAll()
        {
            var cached = _cache.StringGet(AllKey);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<List<Review>>(cached!, _jsonOptions);
            }

            string sql = "SELECT * FROM Reviews";
            var items = GetAll<Review>(sql).ToList();

            _cache.StringSet(AllKey, JsonSerializer.Serialize(items, _jsonOptions), TimeSpan.FromMinutes(10));
            return items;
        }

        public Review GetById(int id)
        {
            string key = ItemKeyPrefix + id;
            var cached = _cache.StringGet(key);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<Review>(cached!, _jsonOptions);
            }

            string sql = "SELECT * FROM Reviews WHERE Id = @Id";
            var review = GetById<Review>(sql, new { Id = id });

            if (review != null)
            {
                _cache.StringSet(key, JsonSerializer.Serialize(review, _jsonOptions), TimeSpan.FromMinutes(10));
            }

            return review;
        }
    }
}
