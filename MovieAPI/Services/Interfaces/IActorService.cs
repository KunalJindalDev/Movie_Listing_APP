using System.Collections.Generic;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;

namespace MovieApp.Services.Interfaces
{
    public interface IActorService
    {
        IList<ActorResponse> GetAll();
        ActorResponse GetById(int id);
        int Add(ActorRequest request);
        bool Update(int id, ActorRequest request);
        bool Delete(int id);
    }
}
