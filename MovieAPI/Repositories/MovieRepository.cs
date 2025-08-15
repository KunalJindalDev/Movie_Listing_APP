using System.Collections.Generic;
using MovieApp.Models.DBModels;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;
using MovieApp.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Dapper;

namespace MovieApp.Repositories
{
    public class MovieRepository : BaseRepository, IMovieRepository
    {
        public MovieRepository(IConfiguration configuration) : base(configuration) { }

        public IList<MovieResponse> GetAll(int? year)
        {
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

            connection.Query<Movie, ProducerResponse, ActorResponse, string, MovieResponse>(
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

            return movieDictionary.Values.ToList();
        }



        public MovieResponse GetById(int id)
        {
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

            return movie;
        }



        public int Add(Movie movie)
        {
            string procedure = "usp_AddMovie"; // Using stored procedure

            return Add(procedure, new
            {
                movie.Name,
                movie.YearOfRelease,
                movie.Plot,
                movie.Poster,
                movie.ProducerId,
                ActorIds = string.Join(",", movie.ActorIds),
                GenreIds = string.Join(",", movie.GenreIds)
            });
        }

        public void Update(Movie movie)
        {
            string procedure = "usp_UpdateMovie"; // Using stored procedure

            Update(procedure, new
            {
                MovieId = movie.Id,
                movie.Name,
                movie.YearOfRelease,
                movie.Plot,
                movie.Poster,
                movie.ProducerId,
                ActorIds = string.Join(",", movie.ActorIds),
                GenreIds = string.Join(",", movie.GenreIds)
            });
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM Movies WHERE Id = @Id";
            Delete(sql, new { Id = id });
        }
    }
}
