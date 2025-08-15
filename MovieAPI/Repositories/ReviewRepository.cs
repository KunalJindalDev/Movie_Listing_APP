using System.Collections.Generic;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace MovieApp.Repositories
{
    public class ReviewRepository : BaseRepository, IReviewRepository
    {
        public ReviewRepository(IConfiguration configuration) : base(configuration) { }

        public IList<Review> GetAll()
        {
            string sql = "SELECT * FROM Reviews";
            return GetAll<Review>(sql).ToList();
        }

        public Review GetById(int id)
        {
            string sql = "SELECT * FROM Reviews WHERE Id = @Id";
            return GetById<Review>(sql, new { Id = id });
        }

        public int Add(Review review)
        {
            string sql = @"
INSERT INTO Reviews (Message, MovieId)
VALUES (@Message, @MovieId); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return Add(sql, review);
        }

        public void Update(Review review)
        {
            string sql = @"
UPDATE Reviews 
SET Message = @Message, MovieId = @MovieId
WHERE Id = @Id";
            Update(sql, review);
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Reviews WHERE Id = @Id";
            Delete(sql, new { Id = id });
        }
    }
}
