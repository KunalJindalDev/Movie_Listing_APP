using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using MovieApp.Repositories;
using MovieApp.Repositories.Interfaces;

namespace MovieApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMovieApiInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Redis connection. Prefer configuration: "Redis:Configuration" or ConnectionStrings:Redis
            var redisConfig = configuration.GetValue<string>("Redis:Configuration") 
                              ?? configuration.GetConnectionString("Redis")
                              ?? "localhost:6379";
            var multiplexer = ConnectionMultiplexer.Connect(redisConfig);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            // CQRS repository registrations - Actor
            services.AddScoped<IActorReadRepository, ActorReadRepository>();
            services.AddScoped<IActorWriteRepository, ActorWriteRepository>();

            // CQRS repository registrations - Genre
            services.AddScoped<IGenreReadRepository, GenreReadRepository>();
            services.AddScoped<IGenreWriteRepository, GenreWriteRepository>();

            // CQRS repository registrations - Producer
            services.AddScoped<IProducerReadRepository, ProducerReadRepository>();
            services.AddScoped<IProducerWriteRepository, ProducerWriteRepository>();

            // CQRS repository registrations - Movie
            services.AddScoped<IMovieReadRepository, MovieReadRepository>();
            services.AddScoped<IMovieWriteRepository, MovieWriteRepository>();

            // CQRS repository registrations - Review
            services.AddScoped<IReviewReadRepository, ReviewReadRepository>();
            services.AddScoped<IReviewWriteRepository, ReviewWriteRepository>();

            // CQRS repository registrations - User
            services.AddScoped<IUserReadRepository, UserReadRepository>();
            services.AddScoped<IUserWriteRepository, UserWriteRepository>();

            return services;
        }
    }
}
