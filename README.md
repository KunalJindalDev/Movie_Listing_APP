# Movie Listing App

A distributed microservice application for listing movies, actors, genres, producers, and reviews. This project implements **CQRS (Command Query Responsibility Segregation)** architecture and uses **Redis** for high-performance caching.

## Key Features

- **CQRS Architecture**: Separation of read and write operations for all entities (Actor, Genre, Producer, Movie, Review, User).
- **Redis Caching**: Cache-aside pattern with automatic invalidation on writes.
- **Docker Support**: Fully containerized with Docker Compose.
- **Dapper**: Lightweight ORM for efficient data access.
- **Swagger UI**: Interactive API documentation.

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) (for local development without Docker)

### Running with Docker Compose (Recommended)

This will start both the MovieAPI and Redis containers.

```bash
docker-compose up --build
```

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Redis**: localhost:6379

To stop the services:

```bash
docker-compose down
```

### Running Locally (Development)

1.  **Start Redis**:
    ```bash
    docker run -d -p 6379:6379 --name redis redis:7-alpine
    ```

2.  **Build and Run API**:
    ```bash
    cd MovieAPI
    dotnet build
    dotnet run
    ```

3.  **Access API**:
    Navigate to https://localhost:5001/swagger

## Architecture

The application follows the CQRS pattern:

-   **Read Path**: `Controller` -> `Service` -> `ReadRepository` -> `Redis Cache` -> `Database`
-   **Write Path**: `Controller` -> `Service` -> `WriteRepository` -> `Database` -> `Invalidate Redis Cache`

### Cache Strategy

-   **Pattern**: Cache-Aside
-   **TTL**: 10 minutes (default)
-   **Invalidation**: Automatic on Create, Update, Delete operations.

### Cache Keys

| Entity | List Key | Item Key |
| :--- | :--- | :--- |
| **Actor** | `Actors:All` | `Actor:{id}` |
| **Genre** | `Genres:All` | `Genre:{id}` |
| **Producer** | `Producers:All` | `Producer:{id}` |
| **Movie** | `Movies:All` | `Movie:{id}` |
| **Review** | `Reviews:All` | `Review:{id}` |
| **User** | N/A | `User:{email}` |

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=MovieDB;...",
    "Redis": "localhost:6379"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  }
}
```

### Environment Variables (Docker)

-   `ASPNETCORE_ENVIRONMENT`: Production
-   `Redis__Configuration`: redis:6379
-   `ConnectionStrings__DefaultConnection`: Your database connection string.

## Useful Commands

### Redis CLI

Connect to the running Redis container:

```bash
docker exec -it movielistingapp-redis-1 redis-cli
```

Common commands:

-   `PING`: Check connection (returns PONG).
-   `KEYS *`: List all keys.
-   `GET <key>`: Get value of a key.
-   `DEL <key>`: Delete a key.
-   `FLUSHDB`: Clear all keys in the current database.
-   `MONITOR`: Monitor commands in real-time.

### Docker

-   `docker ps`: List running containers.
-   `docker logs -f movielistingapp-movieapi-1`: View API logs.
-   `docker logs -f movielistingapp-redis-1`: View Redis logs.

## Troubleshooting

-   **Redis Connection Failed**: Ensure the Redis container is running and the connection string in `appsettings.json` or environment variables is correct.
-   **Cache Not Invalidating**: Check if the write repository is being used and if the invalidation logic matches the cache keys.
-   **API Not Responding**: Check Docker logs for any startup errors.

## Project Structure

```
MovieListingApp/
 MovieAPI/
    Controllers/       # API Controllers
    Services/          # Business Logic (CQRS)
    Repositories/      # Data Access
        Interfaces/    # Read/Write Interfaces
    Models/            # Domain Models
    Extensions/        # DI Extensions
    Dockerfile         # API Container Build
docker-compose.yml     # Docker Orchestration
README.md              # This file
```
