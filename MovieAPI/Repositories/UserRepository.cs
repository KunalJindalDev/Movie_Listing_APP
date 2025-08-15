using Dapper;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MovieApp.Models.DBModels;
using MovieApp.Repositories.Interfaces;

namespace MovieApp.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public void Create(User user)
        {
            using var connection = CreateConnection();
            string sql = @"
INSERT INTO Users (Name, Email, PasswordHash)
VALUES (@Name, @Email, @PasswordHash)";

            connection.Execute(sql, new
            {
                user.Name,
                user.Email,
                user.PasswordHash
            });
        }

        public User GetByEmail(string email)
        {
            using var connection = CreateConnection();
            string sql = "SELECT * FROM Users WHERE Email = @Email";
            return connection.QueryFirstOrDefault<User>(sql, new { Email = email });
        }
    }
}
