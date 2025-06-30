namespace RabbitAdoption.Worker
{
    public class RabbitRequestMessage
    {
        public string RequestId { get; set; }
        public int Priority { get; set; }
        public string Payload { get; set; }
    }
}
