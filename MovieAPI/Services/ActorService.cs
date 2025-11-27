using System.Collections.Generic;
using System.Linq;
using System;
using MovieApp.Models.DBModels;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;
using MovieApp.Repositories.Interfaces;
using MovieApp.Services.Interfaces;
using MovieApp.Models.Enums;
using MovieApp.Exceptions; 

namespace MovieApp.Services
{
    public class ActorService : IActorService
    {
        private readonly IActorReadRepository _actorReadRepository;
        private readonly IActorWriteRepository _actorWriteRepository;

        public ActorService(IActorReadRepository actorReadRepository, IActorWriteRepository actorWriteRepository)
        {
            _actorReadRepository = actorReadRepository;
            _actorWriteRepository = actorWriteRepository;
        }

        public IList<ActorResponse> GetAll()
        {
            var actors = _actorReadRepository.GetAll();
            return actors.Select(a => new ActorResponse
            {
                Id = a.Id,
                Name = a.Name,
                Bio = a.Bio,
                DOB = a.DOB,
                Gender = a.Gender.ToString()
            }).ToList();
        }

        public ActorResponse GetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Actor ID.");
            }

            var actor = _actorReadRepository.GetById(id);
            if (actor == null)
            {
                throw new NotFoundException($"Actor with ID {id} not found.");
            }

            return new ActorResponse
            {
                Id = actor.Id,
                Name = actor.Name,
                Bio = actor.Bio,
                DOB = actor.DOB,
                Gender = actor.Gender.ToString()
            };
        }

        public int Add(ActorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(request.Bio))
                throw new ArgumentException("Bio is required.");

            if (request.DOB > DateTime.Today)
                throw new ArgumentException("Date of birth cannot be in the future.");

            if (!Enum.IsDefined(typeof(Gender), request.Gender))
                throw new ArgumentException("Invalid gender specified.");

            var actor = new Actor
            {
                Name = request.Name,
                Bio = request.Bio,
                DOB = request.DOB,
                Gender = request.Gender
            };
            return _actorWriteRepository.Add(actor);
        }

        public bool Update(int id, ActorRequest request)
        {
            var actor = _actorReadRepository.GetById(id);
            if (actor == null)
            {
                throw new NotFoundException($"Actor with ID {id} not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(request.Bio))
                throw new ArgumentException("Bio is required.");

            if (request.DOB > DateTime.Today)
                throw new ArgumentException("Date of birth cannot be in the future.");

            if (!Enum.IsDefined(typeof(Gender), request.Gender))
                throw new ArgumentException("Invalid gender specified.");

            actor.Name = request.Name;
            actor.Bio = request.Bio;
            actor.DOB = request.DOB;
            actor.Gender = request.Gender;

            _actorWriteRepository.Update(actor);
            return true;
        }

        public bool Delete(int id)
        {
            var actor = _actorReadRepository.GetById(id);
            if (actor == null)
            {
                throw new NotFoundException($"Actor with ID {id} not found.");
            }

            _actorWriteRepository.Delete(id);
            return true;
        }
    }
}
