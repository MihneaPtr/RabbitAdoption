using System;

namespace RabbitAdoption.Domain
{
    public class Rabbit
    {
        public Guid Id { get; private set; }
        public string Size { get; private set; }
        public string Color { get; private set; }
        public int Age { get; private set; }
        public bool IsAdopted { get; private set; }

        public Rabbit(Guid id, string size, string color, int age)
        {
            Id = id;
            Size = size;
            Color = color;
            Age = age;
            IsAdopted = false;
        }

        public void MarkAsAdopted()
        {
            IsAdopted = true;
        }
    }
}