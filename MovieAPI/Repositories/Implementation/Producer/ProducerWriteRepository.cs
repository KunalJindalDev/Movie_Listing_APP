using System;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MovieApp.Repositories
{
    public class ProducerWriteRepository : BaseRepository, IProducerWriteRepository
    {
        private readonly IDatabase _cache;
        private const string AllKey = "Producers:All";
        private const string ItemKeyPrefix = "Producer:";

        public ProducerWriteRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
        }

        public int Add(Producer producer)
        {
            string sql = @"
INSERT INTO Producers (Name, DOB, Bio, Gender)
VALUES (@Name, @DOB, @Bio, @Gender); 
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            int id = Add(sql, new
            {
                producer.Name,
                producer.DOB,
                producer.Bio,
                producer.Gender
            });

            _cache.KeyDelete(AllKey);
            return id;
        }

        public void Update(Producer producer)
        {
            string sql = @"
UPDATE Producers 
SET Name = @Name, DOB = @DOB, Bio = @Bio, Gender = @Gender
WHERE Id = @Id";
            
            Update(sql, new
            {
                producer.Name,
                producer.DOB,
                producer.Bio,
                producer.Gender,
                producer.Id
            });

            _cache.KeyDelete(AllKey);
            _cache.KeyDelete(ItemKeyPrefix + producer.Id);
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Producers WHERE Id = @Id";
            Delete(sql, new { Id = id });

            _cache.KeyDelete(AllKey);
            _cache.KeyDelete(ItemKeyPrefix + id);
        }
    }
}
