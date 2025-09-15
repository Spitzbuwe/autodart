using System.Text.Json.Serialization;

namespace AutodartsTrainer.Models;

public class PlayerProfile
{
    [JsonConstructor]
    public PlayerProfile(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public PlayerProfile(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public Guid Id { get; }

    public string Name { get; set; }
}
