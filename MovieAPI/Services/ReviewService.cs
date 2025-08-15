using System;
using System.Collections.Generic;
using System.Linq;
using MovieApp.Exceptions;
using MovieApp.Models.DBModels;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;
using MovieApp.Repositories.Interfaces;
using MovieApp.Services.Interfaces;

namespace MovieApp.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMovieRepository _movieRepository;

        public ReviewService(IReviewRepository reviewRepository, IMovieRepository movieRepository)
        {
            _reviewRepository = reviewRepository;
            _movieRepository = movieRepository;
        }

        public IList<ReviewResponse> GetAll()
        {
            var reviews = _reviewRepository.GetAll();
            return reviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                Message = r.Message,
                MovieId = r.MovieId
            }).ToList();
        }

        public ReviewResponse GetById(int id)
        {
            var review = _reviewRepository.GetById(id);
            if (review == null)
                throw new NotFoundException($"Review with id {id} not found.");

            return new ReviewResponse
            {
                Id = review.Id,
                Message = review.Message,
                MovieId = review.MovieId
            };
        }

        public int Add(ReviewRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                throw new ArgumentException("Review message cannot be empty.");

            if (_movieRepository.GetById(request.MovieId) == null)
                throw new NotFoundException($"Movie with id {request.MovieId} not found.");

            var review = new Review
            {
                Message = request.Message,
                MovieId = request.MovieId
            };
            return _reviewRepository.Add(review);
        }

        public bool Update(int id, ReviewRequest request)
        {
            var review = _reviewRepository.GetById(id);
            if (review == null)
                throw new NotFoundException($"Review with id {id} not found.");

            if (string.IsNullOrWhiteSpace(request.Message))
                throw new ArgumentException("Review message cannot be empty.");

            review.Message = request.Message;

            _reviewRepository.Update(review);
            return true;
        }

        public bool Delete(int id)
        {
            var review = _reviewRepository.GetById(id);
            if (review == null)
                throw new NotFoundException($"Review with id {id} not found.");

            _reviewRepository.Delete(id);
            return true;
        }
    }
}
