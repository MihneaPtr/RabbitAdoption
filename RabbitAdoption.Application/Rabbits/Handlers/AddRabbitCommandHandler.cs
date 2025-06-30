using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RabbitAdoption.Application.Rabbits.Commands;
using RabbitAdoption.Application.Common.Interfaces;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.Rabbits.Handlers
{
    public class AddRabbitCommandHandler : IRequestHandler<AddRabbitCommand, Guid>
    {
        private readonly IRabbitRepository _repository;
        public AddRabbitCommandHandler(IRabbitRepository repository)
        {
            _repository = repository;
        }
        public async Task<Guid> Handle(AddRabbitCommand request, CancellationToken cancellationToken)
        {
            var rabbit = new Rabbit(Guid.NewGuid(), request.Size, request.Color, request.Age);
            await _repository.AddAsync(rabbit, cancellationToken);
            return rabbit.Id;
        }
    }
}
