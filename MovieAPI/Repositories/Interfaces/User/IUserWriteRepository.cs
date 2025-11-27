using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IUserWriteRepository
    {
        void Create(User user);
    }
}
