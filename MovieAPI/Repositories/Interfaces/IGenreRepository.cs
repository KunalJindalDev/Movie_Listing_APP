using System.Collections.Generic;
using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        IList<Genre> GetAll();
        Genre GetById(int id);
        int Add(Genre genre);
        void Update(Genre genre);
        void Delete(int id);
    }
}
