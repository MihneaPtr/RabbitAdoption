using System;
using MediatR;
using RabbitAdoption.Domain;

namespace RabbitAdoption.Application.Rabbits.Commands
{
    public class AddRabbitCommand : IRequest<Guid>
    {
        public string Size { get; }
        public string Color { get; }
        public int Age { get; }
        public AddRabbitCommand(string size, string color, int age)
        {
            Size = size;
            Color = color;
            Age = age;
        }
    }
}
