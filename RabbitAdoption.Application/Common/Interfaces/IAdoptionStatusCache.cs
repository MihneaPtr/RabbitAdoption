using System;
using System.Threading.Tasks;

namespace RabbitAdoption.Application.Common.Interfaces
{
    public interface IAdoptionStatusCache
    {
        Task<string?> GetStatusAsync(Guid requestId);
        Task SetStatusAsync(Guid requestId, string status);
    }
}
