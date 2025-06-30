using System;
using MediatR;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.Rabbits.Queries
{
    public class GetRabbitQuery : IRequest<Rabbit?>
    {
        public Guid Id { get; }
        public GetRabbitQuery(Guid id)
        {
            Id = id;
        }
    }
}
