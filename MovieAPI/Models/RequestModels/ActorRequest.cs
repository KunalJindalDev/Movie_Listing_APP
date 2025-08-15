using System;
using System.ComponentModel.DataAnnotations;
using MovieApp.Models.Enums;

namespace MovieApp.Models.RequestModels
{
    public class ActorRequest
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime DOB { get; set; }
        public Gender Gender { get; set; }
    }
}
