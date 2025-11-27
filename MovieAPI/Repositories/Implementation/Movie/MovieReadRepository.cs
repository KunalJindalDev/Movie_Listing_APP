using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MovieApp.Models.DBModels;
using MovieApp.Models.ResponseModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Dapper;

namespace MovieApp.Repositories
{
    public class MovieReadRepository : BaseRepository, IMovieReadRepository
    {
        private readonly IDatabase _cache;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string AllKey = "Movies:All";
        private const string AllWithYearKeyPrefix = "Movies:Year:";
        private const string ItemKeyPrefix = "Movie:";

        public MovieReadRepository(IConfiguration configuration, IConnectionMultiplexer multiplexer) : base(configuration)
        {
            _cache = multiplexer.GetDatabase();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public IList<MovieResponse> GetAll(int? year)
        {
            string cacheKey = year.HasValue ? AllWithYearKeyPrefix + year.Value : AllKey;
            var cached = _cache.StringGet(cacheKey);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<List<MovieResponse>>(cached!, _jsonOptions);
            }

            using var connection = CreateConnection();

            var sql = @"
SELECT 
    M.Id, M.Name, M.YearOfRelease, M.Plot, M.Poster,
    P.Id AS Id, P.Name AS Name, P.Bio AS Bio, P.DOB AS DOB, P.Gender As Gender,
    A.Id AS Id, A.Name AS Name, A.Bio AS Bio, A.DOB AS DOB, A.Gender AS Gender,
    G.Name AS Name
FROM Movies M
LEFT JOIN Producers P ON M.ProducerId = P.Id
LEFT JOIN MovieActors MA ON M.Id = MA.MovieId
LEFT JOIN Actors A ON MA.ActorId = A.Id
LEFT JOIN MovieGenres MG ON M.Id = MG.MovieId
LEFT JOIN Genres G ON MG.GenreId = G.Id";

            if (year.HasValue)
                sql += " WHERE M.YearOfRelease = @Year";

            var movieDictionary = new Dictionary<int, MovieResponse>();

            connection.Query<MovieResponse, ProducerResponse, ActorResponse, string, MovieResponse>(
                sql,
                (movie, producer, actor, genre) =>
                {
                    if (!movieDictionary.TryGetValue(movie.Id, out var movieEntry))
                    {
                        movieEntry = new MovieResponse
                        {
                            Id = movie.Id,
                            Name = movie.Name,
                            YearOfRelease = movie.YearOfRelease,
                            Plot = movie.Plot,
                            Poster = movie.Poster,
                            Producer = producer,
                            Actors = new List<ActorResponse>(),
                            Genres = new List<string>()
                        };

                        movieDictionary.Add(movie.Id, movieEntry);
                    }

                    if (actor != null && !movieEntry.Actors.Any(a => a.Id == actor.Id))
                        movieEntry.Actors.Add(actor);

                    if (!string.IsNullOrWhiteSpace(genre) && !movieEntry.Genres.Contains(genre))
                        movieEntry.Genres.Add(genre);

                    return movieEntry;
                },
                new { Year = year },
                splitOn: "Id,Id,Name"
            );

            var result = movieDictionary.Values.ToList();
            _cache.StringSet(cacheKey, JsonSerializer.Serialize(result, _jsonOptions), TimeSpan.FromMinutes(10));
            return result;
        }

        public MovieResponse GetById(int id)
        {
            string key = ItemKeyPrefix + id;
            var cached = _cache.StringGet(key);
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<MovieResponse>(cached!, _jsonOptions);
            }

            using var connection = CreateConnection();

            var sql = @"
SELECT 
    M.Id, M.Name, M.YearOfRelease, M.Plot, M.Poster,
    P.Id AS Id, P.Name AS Name, P.Bio AS Bio, P.DOB AS DOB, P.Gender AS Gender,
    A.Id AS Id, A.Name AS Name, A.Bio AS Bio, A.DOB AS DOB, A.Gender AS Gender,
    G.Name AS Name
FROM Movies M
LEFT JOIN Producers P ON M.ProducerId = P.Id
LEFT JOIN MovieActors MA ON M.Id = MA.MovieId
LEFT JOIN Actors A ON MA.ActorId = A.Id
LEFT JOIN MovieGenres MG ON M.Id = MG.MovieId
LEFT JOIN Genres G ON MG.GenreId = G.Id
WHERE M.Id = @Id";

            MovieResponse movie = null;

            connection.Query<MovieResponse, ProducerResponse, ActorResponse, string, MovieResponse>(
                sql,
                (m, producer, actor, genre) =>
                {
                    if (movie == null)
                    {
                        movie = m;
                        movie.Producer = producer;
                        movie.Actors = new List<ActorResponse>();
                        movie.Genres = new List<string>();
                    }

                    if (actor != null && !movie.Actors.Any(a => a.Id == actor.Id))
                        movie.Actors.Add(actor);

                    if (!string.IsNullOrWhiteSpace(genre) && !movie.Genres.Contains(genre))
                        movie.Genres.Add(genre);

                    return movie;
                },
                new { Id = id },
                splitOn: "Id,Id,Name"
            );

            if (movie != null)
            {
                _cache.StringSet(key, JsonSerializer.Serialize(movie, _jsonOptions), TimeSpan.FromMinutes(10));
            }

            return movie;
        }
    }
}
