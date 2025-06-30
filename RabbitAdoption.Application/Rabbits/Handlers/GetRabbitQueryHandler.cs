using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RabbitAdoption.Application.Rabbits.Queries;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.Rabbits.Handlers
{
    public class GetRabbitQueryHandler : IRequestHandler<GetRabbitQuery, Rabbit?>
    {
        private readonly IRabbitRepository _repository;
        public GetRabbitQueryHandler(IRabbitRepository repository)
        {
            _repository = repository;
        }
        public async Task<Rabbit?> Handle(GetRabbitQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
