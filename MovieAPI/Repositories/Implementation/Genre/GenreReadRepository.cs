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
    public class GenreReadRepository : BaseRepository, IGenreReadRepository
    {
        private readonly IDatabase _cache;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string AllKey = "Genres:All";
        private const string ItemKeyPrefix = "Genre:";

        public GenreReadRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public IList<Genre> GetAll()
        {
            var cached = _cache.StringGet(AllKey);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<List<Genre>>(cached!, _jsonOptions);
            }

            string sql = "SELECT * FROM Genres";
            var items = GetAll<Genre>(sql).ToList();

            _cache.StringSet(AllKey, JsonSerializer.Serialize(items, _jsonOptions), TimeSpan.FromMinutes(10));
            return items;
        }

        public Genre GetById(int id)
        {
            string key = ItemKeyPrefix + id;
            var cached = _cache.StringGet(key);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<Genre>(cached!, _jsonOptions);
            }

            string sql = "SELECT * FROM Genres WHERE Id = @Id";
            var genre = GetById<Genre>(sql, new { Id = id });

            if (genre != null)
            {
                _cache.StringSet(key, JsonSerializer.Serialize(genre, _jsonOptions), TimeSpan.FromMinutes(10));
            }

            return genre;
        }
    }
}
