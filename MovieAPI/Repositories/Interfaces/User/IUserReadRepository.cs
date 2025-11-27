using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IUserReadRepository
    {
        User GetByEmail(string email);
    }
}
