using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MovieApp.Repositories
{
    public class ActorReadRepository : BaseRepository, IActorReadRepository
    {
        private readonly IDatabase _cache;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string AllKey = "Actors:All";
        private const string ItemKeyPrefix = "Actor:";

        public ActorReadRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public IList<Actor> GetAll()
        {
            // Try cache
            var cached = _cache.StringGet(AllKey);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<List<Actor>>(cached!, _jsonOptions);
            }

            // Cache miss -> DB
            string sql = "SELECT * FROM Actors";
            var items = GetAll<Actor>(sql).ToList();

            // Cache result (short TTL)
            _cache.StringSet(AllKey, JsonSerializer.Serialize(items, _jsonOptions), TimeSpan.FromMinutes(10));
            return items;
        }

        public Actor GetById(int id)
        {
            string key = ItemKeyPrefix + id;
            var cached = _cache.StringGet(key);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<Actor>(cached!, _jsonOptions);
            }

            string sql = "SELECT * FROM Actors WHERE Id = @Id";
            var actor = GetById<Actor>(sql, new { Id = id });

            if (actor != null)
            {
                _cache.StringSet(key, JsonSerializer.Serialize(actor, _jsonOptions), TimeSpan.FromMinutes(10));
            }

            return actor;
        }
    }
}
