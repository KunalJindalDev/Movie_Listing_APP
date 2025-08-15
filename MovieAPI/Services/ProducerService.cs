using System;
using System.Collections.Generic;
using System.Linq;
using MovieApp.Exceptions;
using MovieApp.Models.DBModels;
using MovieApp.Models.Enums;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;
using MovieApp.Repositories.Interfaces;
using MovieApp.Services.Interfaces;

namespace MovieApp.Services
{
    public class ProducerService : IProducerService
    {
        private readonly IProducerRepository _producerRepository;

        public ProducerService(IProducerRepository producerRepository)
        {
            _producerRepository = producerRepository;
        }

        public IList<ProducerResponse> GetAll()
        {
            var producers = _producerRepository.GetAll();
            return producers.Select(p => new ProducerResponse
            {
                Id = p.Id,
                Name = p.Name,
                Bio = p.Bio,
                DOB = p.DOB,
                Gender = p.Gender.ToString()
            }).ToList();
        }

        public ProducerResponse GetById(int id)
        {
            var producer = _producerRepository.GetById(id);
            if (producer == null)
                throw new NotFoundException($"Producer with id {id} not found.");

            return new ProducerResponse
            {
                Id = producer.Id,
                Name = producer.Name,
                Bio = producer.Bio,
                DOB = producer.DOB,
                Gender = producer.Gender.ToString()
            };
        }

        public int Add(ProducerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(request.Bio))
                throw new ArgumentException("Bio is required.");

            if (request.DOB > DateTime.Today)
                throw new ArgumentException("Date of birth cannot be in the future.");

            if (!Enum.IsDefined(typeof(Gender), request.Gender))
                throw new ArgumentException("Invalid gender specified.");

            var producer = new Producer
            {
                Name = request.Name,
                Bio = request.Bio,
                DOB = request.DOB,
                Gender = request.Gender
            };

            return _producerRepository.Add(producer);
        }

        public bool Update(int id, ProducerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(request.Bio))
                throw new ArgumentException("Bio is required.");

            if (request.DOB > DateTime.Today)
                throw new ArgumentException("Date of birth cannot be in the future.");

            if (!Enum.IsDefined(typeof(Gender), request.Gender))
                throw new ArgumentException("Invalid gender specified.");

            var producer = _producerRepository.GetById(id);
            if (producer == null)
                throw new NotFoundException($"Producer with id {id} not found.");

            producer.Name = request.Name;
            producer.Bio = request.Bio;
            producer.DOB = request.DOB;
            producer.Gender = request.Gender;

            _producerRepository.Update(producer);
            return true;
        }

        public bool Delete(int id)
        {
            var producer = _producerRepository.GetById(id);
            if (producer == null)
                throw new NotFoundException($"Producer with id {id} not found.");

            _producerRepository.Delete(id);
            return true;
        }
    }
}
