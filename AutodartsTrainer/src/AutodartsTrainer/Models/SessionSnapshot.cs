using System.Text.Json.Serialization;

namespace AutodartsTrainer.Models;

public class SessionSnapshot
{
    [JsonInclude]
    public List<PlayerSnapshot> Players { get; private set; } = new();

    [JsonInclude]
    public SettingsSnapshot Settings { get; private set; } = new();
}

public class PlayerSnapshot
{
    [JsonInclude]
    public Guid Id { get; private set; }

    [JsonInclude]
    public string Name { get; private set; } = string.Empty;

    [JsonInclude]
    public PlayerStatistics Statistics { get; private set; } = new();

    [JsonInclude]
    public int RemainingScore { get; private set; }

    [JsonInclude]
    public int LadderTarget { get; private set; }

    [JsonInclude]
    public int TargetFocusNumber { get; private set; }

    [JsonInclude]
    public int TargetFocusTotalShots { get; private set; }

    [JsonInclude]
    public int TargetFocusDartsThrown { get; private set; }

    [JsonInclude]
    public int TargetFocusHits { get; private set; }

    [JsonInclude]
    public int DoubleRoutineTarget { get; private set; }

    [JsonInclude]
    public int FiftyThreeRemaining { get; private set; }

    public PlayerSnapshot()
    {
    }

    public PlayerSnapshot(PlayerState state)
    {
        Id = state.Profile.Id;
        Name = state.Profile.Name;
        Statistics = new PlayerStatistics();
        Statistics.CopyFrom(state.Statistics);
        RemainingScore = state.RemainingScore;
        LadderTarget = state.LadderTarget;
        TargetFocusNumber = state.TargetFocusNumber;
        TargetFocusTotalShots = state.TargetFocusTotalShots;
        TargetFocusDartsThrown = state.TargetFocusDartsThrown;
        TargetFocusHits = state.TargetFocusHits;
        DoubleRoutineTarget = state.DoubleRoutineTarget;
        FiftyThreeRemaining = state.FiftyThreeRemaining;
    }

    public PlayerState ToPlayerState()
    {
        var profile = new PlayerProfile(Id, Name);
        var state = new PlayerState(profile)
        {
            RemainingScore = RemainingScore,
            LadderTarget = LadderTarget,
            TargetFocusNumber = TargetFocusNumber,
            TargetFocusTotalShots = TargetFocusTotalShots,
            TargetFocusDartsThrown = TargetFocusDartsThrown,
            TargetFocusHits = TargetFocusHits,
            DoubleRoutineTarget = DoubleRoutineTarget,
            FiftyThreeRemaining = FiftyThreeRemaining
        };

        state.Statistics.CopyFrom(Statistics);
        return state;
    }
}

public class SettingsSnapshot
{
    [JsonInclude]
    public string Language { get; private set; } = "de";

    [JsonInclude]
    public string Theme { get; private set; } = "Dark";

    [JsonInclude]
    public bool AudioEnabled { get; private set; } = true;

    [JsonInclude]
    public GameModeType ActiveGameMode { get; private set; } = GameModeType.Checkout170;

    public SettingsSnapshot()
    {
    }

    public SettingsSnapshot(string language, string theme, bool audioEnabled, GameModeType activeGameMode)
    {
        Language = language;
        Theme = theme;
        AudioEnabled = audioEnabled;
        ActiveGameMode = activeGameMode;
    }
}
