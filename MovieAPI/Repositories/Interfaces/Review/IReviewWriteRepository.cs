using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IReviewWriteRepository
    {
        int Add(Review review);
        void Update(Review review);
        void Delete(int id);
    }
}
