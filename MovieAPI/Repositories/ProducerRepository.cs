using System.Collections.Generic;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MovieApp.Repositories
{
    public class ProducerRepository : BaseRepository, IProducerRepository
    {
        public ProducerRepository(IConfiguration configuration) : base(configuration) { }

        public IList<Producer> GetAll()
        {
            string sql = "SELECT * FROM Producers";
            return GetAll<Producer>(sql).ToList();
        }

        public Producer GetById(int id)
        {
            string sql = "SELECT * FROM Producers WHERE Id = @Id";
            return GetById<Producer>(sql, new { Id = id });
        }

        public int Add(Producer producer)
        {
            string sql = @"
INSERT INTO Producers (Name, DOB, Bio, Gender)
VALUES (@Name, @DOB, @Bio, @Gender); 
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return Add(sql, new
            {
                producer.Name,
                producer.DOB,
                producer.Bio,
                producer.Gender
            });
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
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Producers WHERE Id = @Id";
            Delete(sql, new { Id = id });
        }
    }
}
