using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.Rabbits.Handlers
{
    public class GetAllRabbitsQueryHandler : IRequestHandler<Queries.GetAllRabbitsQuery, List<Rabbit>>
    {
        private readonly IRabbitRepository _repository;
        public GetAllRabbitsQueryHandler(IRabbitRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<Rabbit>> Handle(Queries.GetAllRabbitsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync(cancellationToken);
        }
    }
}
