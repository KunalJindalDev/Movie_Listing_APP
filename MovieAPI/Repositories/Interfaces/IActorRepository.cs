using System.Collections.Generic;
using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IActorRepository
    {
        IList<Actor> GetAll();
        Actor GetById(int id);
        int Add(Actor actor);
        void Update(Actor actor);
        void Delete(int id);
    }
}
