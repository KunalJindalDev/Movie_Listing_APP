using System.Collections.Generic;
using MovieApp.Models.DBModels;

namespace MovieApp.Repositories.Interfaces
{
    public interface IProducerReadRepository
    {
        IList<Producer> GetAll();
        Producer GetById(int id);
    }
}
