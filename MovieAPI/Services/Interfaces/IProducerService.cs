using System.Collections.Generic;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;

namespace MovieApp.Services.Interfaces
{
    public interface IProducerService
    {
        IList<ProducerResponse> GetAll();
        ProducerResponse GetById(int id);
        int Add(ProducerRequest request);
        bool Update(int id, ProducerRequest request);
        bool Delete(int id);
    }
}
