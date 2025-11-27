using System.Collections.Generic;
using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IReviewReadRepository
    {
        IList<Review> GetAll();
        Review GetById(int id);
    }
}
