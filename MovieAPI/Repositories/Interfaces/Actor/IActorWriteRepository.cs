using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IActorWriteRepository
    {
        int Add(Actor actor);
        void Update(Actor actor);
        void Delete(int id);
    }
}
