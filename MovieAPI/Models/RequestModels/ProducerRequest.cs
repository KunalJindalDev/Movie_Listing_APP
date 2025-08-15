using System;
using MovieApp.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models.RequestModels
{
    public class ProducerRequest
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime DOB { get; set; }
        public Gender Gender { get; set; }
    }
}