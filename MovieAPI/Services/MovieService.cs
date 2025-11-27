using System.Collections.Generic;
using System.Linq;
using System;
using MovieApp.Models.DBModels;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;
using MovieApp.Repositories.Interfaces;
using MovieApp.Services.Interfaces;
using System.Data.SqlClient;

namespace MovieApp.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieReadRepository _movieReadRepository;
        private readonly IMovieWriteRepository _movieWriteRepository;

        public MovieService(IMovieReadRepository movieReadRepository, IMovieWriteRepository movieWriteRepository)
        {
            _movieReadRepository = movieReadRepository;
            _movieWriteRepository = movieWriteRepository;
        }

        public IList<MovieResponse> GetAll(int? year)
        {
            var movies = _movieReadRepository.GetAll(year);

            return movies.Select(m => new MovieResponse
            {
                Id = m.Id,
                Name = m.Name,
                YearOfRelease = m.YearOfRelease,
                Plot = m.Plot,
                Poster = m.Poster,
                Producer = new ProducerResponse
                {
                    Id = m.Producer.Id,
                    Name = m.Producer.Name,
                    Bio = m.Producer.Bio,
                    DOB = m.Producer.DOB
                },
                Actors = m.Actors.Select(a => new ActorResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    Bio = a.Bio,
                    DOB = a.DOB,
                    Gender = a.Gender
                }).ToList(),
                Genres = m.Genres
            }).ToList();
        }

        public MovieResponse GetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Movie ID.");
            }

            var movie = _movieReadRepository.GetById(id);
            if (movie == null) return null;

            return new MovieResponse
            {
                Id = movie.Id,
                Name = movie.Name,
                YearOfRelease = movie.YearOfRelease,
                Plot = movie.Plot,
                Poster = movie.Poster,
                Producer = new ProducerResponse
                {
                    Id = movie.Producer.Id,
                    Name = movie.Producer.Name,
                    Bio = movie.Producer.Bio,
                    DOB = movie.Producer.DOB
                },
                Actors = movie.Actors.Select(a => new ActorResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    Bio = a.Bio,
                    DOB = a.DOB,
                    Gender = a.Gender
                }).ToList(),
                Genres = movie.Genres
            };
        }


        public int Add(MovieRequest request)
        {
            ValidateRequest(request);

            if (request.ActorIds == null || request.ActorIds.Count == 0)
                throw new ArgumentException("At least one actor must be specified.");

            if (request.GenreIds == null || request.GenreIds.Count == 0)
                throw new ArgumentException("At least one genre must be specified.");

            var movie = new Movie
            {
                Name = request.Name,
                YearOfRelease = request.YearOfRelease,
                Plot = request.Plot,
                Poster = request.Poster,
                ProducerId = request.ProducerId,
                ActorIds = request.ActorIds,
                GenreIds = request.GenreIds,
            };

            try
            {
                return _movieWriteRepository.Add(movie);
            }
            catch (SqlException ex) when (ex.Number == 547) // FK violation
            {
                throw new ArgumentException("Invalid producer, actor, or genre ID.");
            }
        }

        public bool Update(int id, MovieRequest request)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid Movie ID.");

            ValidateRequest(request);

            var movie = new Movie
            {
                Id = id,
                Name = request.Name,
                YearOfRelease = request.YearOfRelease,
                Plot = request.Plot,
                Poster = request.Poster,
                ProducerId = request.ProducerId,
                ActorIds = request.ActorIds,
                GenreIds = request.GenreIds
            };

            _movieWriteRepository.Update(movie);
            return true;
        }


        public bool Delete(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Movie ID.");
            }

            var movie = _movieReadRepository.GetById(id);
            if (movie == null) return false;

            _movieWriteRepository.Delete(id);
            return true;
        }

        private void ValidateRequest(MovieRequest request)
        {
            if (request == null)
                throw new ArgumentException("Request cannot be null.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Movie name is required.");

            if (string.IsNullOrWhiteSpace(request.Plot))
                throw new ArgumentException("Plot is required.");

            if (string.IsNullOrWhiteSpace(request.Poster))
                throw new ArgumentException("Poster is required.");

            if (request.YearOfRelease < 1888 || request.YearOfRelease > DateTime.Now.Year)
                throw new ArgumentException("Invalid release year.");

            if (request.ActorIds == null || !request.ActorIds.Any())
                throw new ArgumentException("At least one actor must be associated with the movie.");

            if (request.GenreIds == null || !request.GenreIds.Any())
                throw new ArgumentException("At least one genre must be associated with the movie.");
        }
    }
}
