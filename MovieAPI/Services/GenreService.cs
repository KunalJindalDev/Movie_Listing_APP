using System.Collections.Generic;
using System.Linq;
using System;
using MovieApp.Models.DBModels;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;
using MovieApp.Repositories.Interfaces;
using MovieApp.Services.Interfaces;
using MovieApp.Exceptions; 

namespace MovieApp.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public IList<GenreResponse> GetAll()
        {
            var genres = _genreRepository.GetAll();
            return genres.Select(g => new GenreResponse
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();
        }

        public GenreResponse GetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid genre ID.");
            }

            var genre = _genreRepository.GetById(id);
            if (genre == null)
            {
                throw new NotFoundException($"Genre with ID {id} not found.");
            }

            return new GenreResponse
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }

        public int Add(GenreRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Genre name is required.");

            if (request.Name.Length > 100)
                throw new ArgumentException("Genre name cannot be longer than 100 characters.");

            var genres = _genreRepository.GetAll();
            if (genres.Any(g => g.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("A genre with this name already exists.");

            var genre = new Genre
            {
                Name = request.Name
            };
            return _genreRepository.Add(genre);
        }

        public bool Update(int id, GenreRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Genre name is required.");

            if (request.Name.Length > 100)
                throw new ArgumentException("Genre name cannot be longer than 100 characters.");

            var genres = _genreRepository.GetAll();
            if (genres.Any(g => g.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("A genre with this name already exists.");

            var genre = _genreRepository.GetById(id);
            if (genre == null)
            {
                throw new NotFoundException($"Genre with ID {id} not found.");
            }

            genre.Name = request.Name;
            _genreRepository.Update(genre);
            return true;
        }

        public bool Delete(int id)
        {
            var genre = _genreRepository.GetById(id);
            if (genre == null)
            {
                throw new NotFoundException($"Genre with ID {id} not found.");
            }

            _genreRepository.Delete(id);
            return true;
        }
    }
}
