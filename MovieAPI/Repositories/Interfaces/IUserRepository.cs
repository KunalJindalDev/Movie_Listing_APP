using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void Create(User user);
        User GetByEmail(string email);
    }
}
