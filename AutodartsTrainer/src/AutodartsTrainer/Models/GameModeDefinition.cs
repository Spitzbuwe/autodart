namespace AutodartsTrainer.Models;

public class GameModeDefinition
{
    public GameModeDefinition(GameModeType type, string nameResourceKey, string descriptionResourceKey, bool supportsCameras)
    {
        Type = type;
        NameResourceKey = nameResourceKey;
        DescriptionResourceKey = descriptionResourceKey;
        SupportsCameras = supportsCameras;
    }

    public GameModeType Type { get; }

    public string NameResourceKey { get; }

    public string DescriptionResourceKey { get; }

    public bool SupportsCameras { get; }
}
