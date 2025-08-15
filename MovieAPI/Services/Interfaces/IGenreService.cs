using System.Collections.Generic;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;

namespace MovieApp.Services.Interfaces
{
    public interface IGenreService
    {
        IList<GenreResponse> GetAll();
        GenreResponse GetById(int id);
        int Add(GenreRequest request);
        bool Update(int id, GenreRequest request);
        bool Delete(int id);
    }
}
