using System;
using MovieApp.Models.Enums;
namespace MovieApp.Models.DBModels
{
    public class Producer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime DOB { get; set; }
        public Gender Gender { get; set; }
    }
}
