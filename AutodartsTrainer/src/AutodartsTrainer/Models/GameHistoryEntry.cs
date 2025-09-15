using System.Collections.Generic;
using System.Linq;

namespace AutodartsTrainer.Models;

public class GameHistoryEntry
{
    public GameHistoryEntry(PlayerState player, IReadOnlyList<DartResult> darts, GameModeType mode, bool bust, bool checkout,
        int scoreBeforeTurn, int ladderTargetBefore, int ladderAttemptsBefore, int targetFocusHitsBefore,
        int targetFocusDartsBefore, int fiftyThreeBefore, IReadOnlyDictionary<Guid, int> remainingScoresBefore,
        IReadOnlyDictionary<Guid, int> fiftyThreeScoresBefore)
    {
        Player = player;
        Darts = darts.ToList();
        Mode = mode;
        Bust = bust;
        Checkout = checkout;
        ScoreBeforeTurn = scoreBeforeTurn;
        LadderTargetBefore = ladderTargetBefore;
        LadderAttemptsBefore = ladderAttemptsBefore;
        TargetFocusHitsBefore = targetFocusHitsBefore;
        TargetFocusDartsBefore = targetFocusDartsBefore;
        FiftyThreeBefore = fiftyThreeBefore;
        RemainingScoresBefore = remainingScoresBefore;
        FiftyThreeScoresBefore = fiftyThreeScoresBefore;
    }

    public PlayerState Player { get; }

    public IReadOnlyList<DartResult> Darts { get; }

    public GameModeType Mode { get; }

    public bool Bust { get; }

    public bool Checkout { get; }

    public int ScoreBeforeTurn { get; }

    public int LadderTargetBefore { get; }

    public int LadderAttemptsBefore { get; }

    public int TargetFocusHitsBefore { get; }

    public int TargetFocusDartsBefore { get; }

    public int FiftyThreeBefore { get; }

    public IReadOnlyDictionary<Guid, int> RemainingScoresBefore { get; }

    public IReadOnlyDictionary<Guid, int> FiftyThreeScoresBefore { get; }
}
