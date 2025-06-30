using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.AdoptionRequests.Queries
{
    public class GetAdoptionRequestStatusQuery : IRequest<string>
    {
        public Guid Id { get; }
        public GetAdoptionRequestStatusQuery(Guid id)
        {
            Id = id;
        }
    }
}
