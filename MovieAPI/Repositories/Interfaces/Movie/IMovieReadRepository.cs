using System.Collections.Generic;
using MovieApp.Models.DBModels;
using MovieApp.Models.ResponseModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IMovieReadRepository
    {
        IList<MovieResponse> GetAll(int? year);
        MovieResponse GetById(int id);
    }
}
