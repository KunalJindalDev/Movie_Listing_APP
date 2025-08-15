using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace MovieApp.Repositories
{
    public abstract class BaseRepository
    {
        private readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // CRUD - Get All
        public IEnumerable<T> GetAll<T>(string sql)
        {
            using (var connection = CreateConnection())
            {
                return connection.Query<T>(sql);
            }
        }

        // CRUD - Get By Id
        public T GetById<T>(string sql, object param)
        {
            using (var connection = CreateConnection())
            {
                return connection.QuerySingleOrDefault<T>(sql, param);
            }
        }

        // CRUD - Add
        public int Add<T>(string sql, T entity)
        {
            using (var connection = CreateConnection())
            {
                return connection.ExecuteScalar<int>(sql, entity);
            }
        }

        // CRUD - Update
        public void Update<T>(string sql, T entity)
        {
            using (var connection = CreateConnection())
            {
                connection.Execute(sql, entity);
            }
        }

        // CRUD - Delete
        public void Delete(string sql, object param)
        {
            using (var connection = CreateConnection())
            {
                connection.Execute(sql, param);
            }
        }
    }
}
