using MediatR;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.AdoptionRequests.Commands
{
    public class SubmitAdoptionRequestCommand : IRequest<Guid>
    {
        public string AdopterName { get; }
        public string Contact { get; }
        public string Size { get; }
        public string Color { get; }
        public int? Age { get; }
        public AdoptionPriority Priority { get; }
        public int YearsRabbitExperience { get; }

        public SubmitAdoptionRequestCommand(string adopterName, string contact, string size, string color, int? age, AdoptionPriority priority, int yearsRabbitExperience)
        {
            AdopterName = adopterName;
            Contact = contact;
            Size = size;
            Color = color;
            Age = age;
            Priority = priority;
            YearsRabbitExperience = yearsRabbitExperience;
        }
    }
}
