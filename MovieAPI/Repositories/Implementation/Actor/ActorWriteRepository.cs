using Dapper;
using System;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MovieApp.Repositories
{
    public class ActorWriteRepository : BaseRepository, IActorWriteRepository
    {
        private readonly IDatabase _cache;
        private const string AllKey = "Actors:All";
        private const string ItemKeyPrefix = "Actor:";

        public ActorWriteRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
        }

        public int Add(Actor actor)
        {
            string sql = @"
INSERT INTO Actors (Name, DOB, Bio, Gender)
VALUES (@Name, @DOB, @Bio, @Gender);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            int id = Add(sql, new
            {
                actor.Name,
                actor.DOB,
                actor.Bio,
                actor.Gender
            });

            // Invalidate list cache. Individual item cache will be created on next read.
            _cache.KeyDelete(AllKey);
            return id;
        }

        public void Update(Actor actor)
        {
            string sql = @"
UPDATE Actors 
SET Name = @Name, DOB = @DOB, Bio = @Bio, Gender = @Gender
WHERE Id = @Id";

            Update(sql, new
            {
                actor.Name,
                actor.DOB,
                actor.Bio,
                actor.Gender,
                actor.Id
            });

            // Invalidate caches for this actor and the list
            _cache.KeyDelete(AllKey);
            _cache.KeyDelete(ItemKeyPrefix + actor.Id);
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Actors WHERE Id = @Id";
            Delete(sql, new { Id = id });

            _cache.KeyDelete(AllKey);
            _cache.KeyDelete(ItemKeyPrefix + id);
        }
    }
}
