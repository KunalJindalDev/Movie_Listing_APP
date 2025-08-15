using System.Collections.Generic;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;

namespace MovieApp.Services.Interfaces
{
    public interface IReviewService
    {
        IList<ReviewResponse> GetAll();
        ReviewResponse GetById(int id);
        int Add(ReviewRequest request);
        bool Update(int id, ReviewRequest request);
        bool Delete(int id);
    }
}
