using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IGenreWriteRepository
    {
        int Add(Genre genre);
        void Update(Genre genre);
        void Delete(int id);
    }
}
