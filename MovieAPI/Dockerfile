FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MovieAPI/MovieApp.csproj", "MovieAPI/"]
RUN dotnet restore "MovieAPI/MovieApp.csproj"
COPY . .
WORKDIR "/src/MovieAPI"
RUN dotnet publish "MovieApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MovieApp.dll"]
