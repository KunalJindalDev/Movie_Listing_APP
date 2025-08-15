using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MovieApp.Repositories
{
    public class GenreRepository : BaseRepository, IGenreRepository
    {
        public GenreRepository(IConfiguration configuration) : base(configuration) { }

        public IList<Genre> GetAll()
        {
            string sql = "SELECT * FROM Genres";
            return GetAll<Genre>(sql).ToList();
        }

        public Genre GetById(int id)
        {
            string sql = "SELECT * FROM Genres WHERE Id = @Id";
            return GetById<Genre>(sql, new { Id = id });
        }

        public int Add(Genre genre)
        {
            string sql = "INSERT INTO Genres (Name) VALUES (@Name); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return Add(sql, new { genre.Name });
        }

        public void Update(Genre genre)
        {
            string sql = "UPDATE Genres SET Name = @Name WHERE Id = @Id";
            Update(sql, new { genre.Name, genre.Id });
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Genres WHERE Id = @Id";
            Delete(sql, new { Id = id });
        }
    }
}
