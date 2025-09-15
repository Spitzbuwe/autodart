using System.Collections.ObjectModel;

namespace AutodartsTrainer.Models;

public class PlayerState
{
    public PlayerState(PlayerProfile profile)
    {
        Profile = profile;
        Statistics = new PlayerStatistics();
    }

    public PlayerProfile Profile { get; }

    public int RemainingScore { get; set; }

    public PlayerStatistics Statistics { get; }

    public ObservableCollection<DartResult> DartHistory { get; } = new();

    public bool IsActive { get; set; }

    public int DartsInCurrentTurn { get; set; }

    public int LadderTarget { get; set; } = 123;

    public int LadderAttempts { get; set; }

    public int TargetFocusNumber { get; set; } = 20;

    public int TargetFocusDartsThrown { get; set; }

    public int TargetFocusHits { get; set; }

    public int TargetFocusTotalShots { get; set; } = 30;

    public int DoubleRoutineTarget { get; set; } = 16;

    public int FiftyThreeRemaining { get; set; } = 53;

    public void ResetStatistics()
    {
        Statistics.Reset();
        DartHistory.Clear();
        RemainingScore = 0;
        DartsInCurrentTurn = 0;
        LadderTarget = 123;
        LadderAttempts = 0;
        TargetFocusDartsThrown = 0;
        TargetFocusHits = 0;
        TargetFocusTotalShots = 30;
        DoubleRoutineTarget = 16;
        FiftyThreeRemaining = 53;
    }
}
