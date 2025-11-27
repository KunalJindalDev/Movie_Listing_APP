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
    public class ProducerReadRepository : BaseRepository, IProducerReadRepository
    {
        private readonly IDatabase _cache;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string AllKey = "Producers:All";
        private const string ItemKeyPrefix = "Producer:";

        public ProducerReadRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public IList<Producer> GetAll()
        {
            var cached = _cache.StringGet(AllKey);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<List<Producer>>(cached!, _jsonOptions);
            }

            string sql = "SELECT * FROM Producers";
            var items = GetAll<Producer>(sql).ToList();

            _cache.StringSet(AllKey, JsonSerializer.Serialize(items, _jsonOptions), TimeSpan.FromMinutes(10));
            return items;
        }

        public Producer GetById(int id)
        {
            string key = ItemKeyPrefix + id;
            var cached = _cache.StringGet(key);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<Producer>(cached!, _jsonOptions);
            }

            string sql = "SELECT * FROM Producers WHERE Id = @Id";
            var producer = GetById<Producer>(sql, new { Id = id });

            if (producer != null)
            {
                _cache.StringSet(key, JsonSerializer.Serialize(producer, _jsonOptions), TimeSpan.FromMinutes(10));
            }

            return producer;
        }
    }
}
