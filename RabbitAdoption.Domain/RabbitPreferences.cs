namespace RabbitAdoption.Domain;
public class RabbitPreferences
{

    public string Color { get; private set; }
    public string Breed { get; private set; }
    public string AgeRange { get; private set; }
    public string? Size { get; private set; }

    private RabbitPreferences() { }

    public RabbitPreferences(string color, string breed, string ageRange, string? size = null)
    {
        Color = color;
        Breed = breed;
        AgeRange = ageRange;
        Size = size;
    }


    public override bool Equals(object? obj)
    {
        if (obj is not RabbitPreferences other) return false;
        return Color == other.Color && Breed == other.Breed && AgeRange == other.AgeRange && Size == other.Size;
    }

    public override int GetHashCode() => HashCode.Combine(Color, Breed, AgeRange, Size);
}