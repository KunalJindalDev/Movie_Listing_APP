using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MovieApp.Repositories
{
    public class ActorRepository : BaseRepository, IActorRepository
    {
        public ActorRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public IList<Actor> GetAll()
        {
            string sql = "SELECT * FROM Actors"; 
            return GetAll<Actor>(sql).ToList();  
        }

        public Actor GetById(int id)
        {
            string sql = "SELECT * FROM Actors WHERE Id = @Id"; 
            return GetById<Actor>(sql, new { Id = id }); 
        }

        public int Add(Actor actor)
        {
            string sql = @"
INSERT INTO Actors (Name, DOB, Bio, Gender)
VALUES (@Name, @DOB, @Bio, @Gender);
SELECT CAST(SCOPE_IDENTITY() AS INT);"; 

            return Add(sql, new
            {
                actor.Name,
                actor.DOB,
                actor.Bio,
                actor.Gender
            }); 
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
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Actors WHERE Id = @Id"; 
            Delete(sql, new { Id = id }); 
        }
    }
}
