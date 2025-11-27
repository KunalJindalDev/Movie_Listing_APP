using System.Collections.Generic;
using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IGenreReadRepository
    {
        IList<Genre> GetAll();
        Genre GetById(int id);
    }
}
