using System.Collections.Generic;
using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IProducerRepository
    {
        IList<Producer> GetAll();
        Producer GetById(int id);
        int Add(Producer producer);
        void Update(Producer producer);
        void Delete(int id);
    }
}
