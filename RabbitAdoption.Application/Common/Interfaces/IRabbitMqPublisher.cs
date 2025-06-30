using System.Threading.Tasks;

namespace RabbitAdoption.Application.Common.Interfaces
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync(string queueName, object message, int priority = 0);
    }
}
