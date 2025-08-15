using System.Collections.Generic;

namespace MovieApp.Models.ResponseModels
{
    public class MovieResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int YearOfRelease { get; set; }
        public string Plot { get; set; }
        public string Poster { get; set; }

        public ProducerResponse Producer { get; set; }
        public List<ActorResponse> Actors { get; set; } = new();
        public List<string> Genres { get; set; } = new();
    }
}
