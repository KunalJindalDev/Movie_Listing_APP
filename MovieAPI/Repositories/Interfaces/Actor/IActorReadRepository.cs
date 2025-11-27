using System.Collections.Generic;
using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IActorReadRepository
    {
        IList<Actor> GetAll();
        Actor GetById(int id);
    }
}
