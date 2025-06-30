using RabbitAdoption.Domain;

namespace RabbitAdoption.Domain;
public class AdoptionRequest
{
    public Guid Id { get; private set; }
    public string AdopterName { get; private set; }
    public string Contact { get; private set; }
    public RabbitPreferences Preferences { get; set; }
    public AdoptionPriority Priority { get; private set; }
    public int YearsRabbitExperience { get; private set; }
    public AdoptionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AdoptionRequest(Guid id, string adopterName, string contact, RabbitPreferences preferences, AdoptionPriority priority, int yearsRabbitExperience)
    {
        Id = id;
        AdopterName = adopterName;
        Contact = contact;
        Preferences = preferences;
        Priority = priority;
        YearsRabbitExperience = yearsRabbitExperience;
        Status = AdoptionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetStatus(AdoptionStatus status)
    {
        Status = status;
    }

    private AdoptionRequest() { }
}