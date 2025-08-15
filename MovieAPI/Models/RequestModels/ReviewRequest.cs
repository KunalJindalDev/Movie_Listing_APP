using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models.RequestModels
{
    public class ReviewRequest
    {
        public string Message { get; set; }
        public int MovieId { get; set; }
    }
}
