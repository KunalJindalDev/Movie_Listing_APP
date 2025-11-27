using System;
using System.Linq;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MovieApp.Repositories
{
    public class MovieWriteRepository : BaseRepository, IMovieWriteRepository
    {
        private readonly IDatabase _cache;
        private const string AllKey = "Movies:All";
        private const string AllWithYearKeyPrefix = "Movies:Year:";
        private const string ItemKeyPrefix = "Movie:";

        public MovieWriteRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
        }

        public int Add(Movie movie)
        {
            string procedure = "usp_AddMovie";

            int id = Add(procedure, new
            {
                movie.Name,
                movie.YearOfRelease,
                movie.Plot,
                movie.Poster,
                movie.ProducerId,
                ActorIds = string.Join(",", movie.ActorIds),
                GenreIds = string.Join(",", movie.GenreIds)
            });

            InvalidateMovieCache();
            return id;
        }

        public void Update(Movie movie)
        {
            string procedure = "usp_UpdateMovie";

            Update(procedure, new
            {
                MovieId = movie.Id,
                movie.Name,
                movie.YearOfRelease,
                movie.Plot,
                movie.Poster,
                movie.ProducerId,
                ActorIds = string.Join(",", movie.ActorIds),
                GenreIds = string.Join(",", movie.GenreIds)
            });

            InvalidateMovieCache(movie.Id);
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Movies WHERE Id = @Id";
            Delete(sql, new { Id = id });

            InvalidateMovieCache(id);
        }

        private void InvalidateMovieCache(int? specificId = null)
        {
            _cache.KeyDelete(AllKey);
            
            // Delete all year-based cache keys
            var server = _cache.Multiplexer.GetServer(_cache.Multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: AllWithYearKeyPrefix + "*");
            foreach (var key in keys)
            {
                _cache.KeyDelete(key);
            }

            if (specificId.HasValue)
            {
                _cache.KeyDelete(ItemKeyPrefix + specificId.Value);
            }
        }
    }
}
