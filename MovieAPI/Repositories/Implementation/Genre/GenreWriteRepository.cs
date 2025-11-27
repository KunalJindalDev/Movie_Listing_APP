using System;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MovieApp.Repositories
{
    public class GenreWriteRepository : BaseRepository, IGenreWriteRepository
    {
        private readonly IDatabase _cache;
        private const string AllKey = "Genres:All";
        private const string ItemKeyPrefix = "Genre:";

        public GenreWriteRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
        }

        public int Add(Genre genre)
        {
            string sql = "INSERT INTO Genres (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            int id = Add(sql, new { genre.Name });

            _cache.KeyDelete(AllKey);
            return id;
        }

        public void Update(Genre genre)
        {
            string sql = "UPDATE Genres SET Name = @Name WHERE Id = @Id";
            Update(sql, new { genre.Name, genre.Id });

            _cache.KeyDelete(AllKey);
            _cache.KeyDelete(ItemKeyPrefix + genre.Id);
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Genres WHERE Id = @Id";
            Delete(sql, new { Id = id });

            _cache.KeyDelete(AllKey);
            _cache.KeyDelete(ItemKeyPrefix + id);
        }
    }
}
