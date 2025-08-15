using System;
using MovieApp.Models.Enums;

namespace MovieApp.Models.ResponseModels
{
    public class ActorResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
    }
}
