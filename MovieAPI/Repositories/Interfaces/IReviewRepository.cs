using System.Collections.Generic;
using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        IList<Review> GetAll();
        Review GetById(int id);
        int Add(Review review);
        void Update(Review review);
        void Delete(int id);
    }
}