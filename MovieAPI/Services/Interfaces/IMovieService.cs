using System.Collections.Generic;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;

namespace MovieApp.Services.Interfaces
{
    public interface IMovieService
    {
        IList<MovieResponse> GetAll(int? year);
        MovieResponse GetById(int id);
        int Add(MovieRequest request);
        bool Update(int id, MovieRequest request);
        bool Delete(int id);
    }
}
