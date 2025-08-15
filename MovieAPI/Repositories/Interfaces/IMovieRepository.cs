using System.Collections.Generic;
using MovieApp.Models.DBModels;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        IList<MovieResponse> GetAll(int? year);
        MovieResponse GetById(int id);
        int Add(Movie movie);
        void Update(Movie movie);
        void Delete(int id);
    }
}
