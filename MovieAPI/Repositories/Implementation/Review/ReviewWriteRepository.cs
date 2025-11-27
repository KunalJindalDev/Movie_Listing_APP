using System;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MovieApp.Repositories
{
    public class ReviewWriteRepository : BaseRepository, IReviewWriteRepository
    {
        private readonly IDatabase _cache;
        private const string AllKey = "Reviews:All";
        private const string ItemKeyPrefix = "Review:";

        public ReviewWriteRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
        }

        public int Add(Review review)
        {
            string sql = @"
INSERT INTO Reviews (Message, MovieId)
VALUES (@Message, @MovieId); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            int id = Add(sql, review);

            _cache.KeyDelete(AllKey);
            return id;
        }

        public void Update(Review review)
        {
            string sql = @"
UPDATE Reviews 
SET Message = @Message, MovieId = @MovieId
WHERE Id = @Id";
            
            Update(sql, review);

            _cache.KeyDelete(AllKey);
            _cache.KeyDelete(ItemKeyPrefix + review.Id);
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Reviews WHERE Id = @Id";
            Delete(sql, new { Id = id });

            _cache.KeyDelete(AllKey);
            _cache.KeyDelete(ItemKeyPrefix + id);
        }
    }
}
