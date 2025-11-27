using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IProducerWriteRepository
    {
        int Add(Producer producer);
        void Update(Producer producer);
        void Delete(int id);
    }
}
