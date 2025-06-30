using System.Collections.Generic;
using RabbitAdoption.Domain;
using MediatR;

namespace RabbitAdoption.Application.Rabbits.Queries
{
    public record GetAllRabbitsQuery() : IRequest<List<Rabbit>>;
}
