using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IMovieWriteRepository
    {
        int Add(Movie movie);
        void Update(Movie movie);
        void Delete(int id);
    }
}
