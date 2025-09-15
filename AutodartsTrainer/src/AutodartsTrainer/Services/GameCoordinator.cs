using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutodartsTrainer.Models;

namespace AutodartsTrainer.Services;

public class GameCoordinator
{
    private readonly Stack<GameHistoryEntry> _history = new();

    public GameCoordinator()
    {
        Modes = new List<GameModeDefinition>
        {
            new(GameModeType.Checkout170, "Mode.Checkout170.Title", "Mode.Checkout170.Description", true),
            new(GameModeType.Ladder123, "Mode.Ladder123.Title", "Mode.Ladder123.Description", false),
            new(GameModeType.TargetFocus, "Mode.TargetFocus.Title", "Mode.TargetFocus.Description", false),
            new(GameModeType.DoubleFinishRoutine, "Mode.DoubleRoutine.Title", "Mode.DoubleRoutine.Description", false),
            new(GameModeType.FiftyThreeCheckout, "Mode.FiftyThree.Title", "Mode.FiftyThree.Description", false)
        };

        CurrentMode = Modes.First();
    }

    public ObservableCollection<PlayerState> Players { get; } = new();

    public IReadOnlyList<GameModeDefinition> Modes { get; }

    public GameModeDefinition CurrentMode { get; private set; }

    public PlayerState? ActivePlayer { get; private set; }

    public event EventHandler? PlayersChanged;

    public bool CanUndo => _history.Count > 0;

    public void SetMode(GameModeType type)
    {
        var mode = Modes.FirstOrDefault(m => m.Type == type);
        if (mode == null)
        {
            return;
        }

        CurrentMode = mode;
        InitializeMode();
    }

    public PlayerState AddPlayer(string name)
    {
        var profile = new PlayerProfile(name);
        var state = new PlayerState(profile);
        Players.Add(state);
        if (ActivePlayer == null)
        {
            ActivePlayer = state;
            state.IsActive = true;
        }

        InitializeMode();
        PlayersChanged?.Invoke(this, EventArgs.Empty);
        return state;
    }

    public void RemovePlayer(PlayerState player)
    {
        if (!Players.Contains(player))
        {
            return;
        }

        Players.Remove(player);
        if (ActivePlayer == player)
        {
            ActivePlayer = Players.FirstOrDefault();
            if (ActivePlayer != null)
            {
                ActivePlayer.IsActive = true;
            }
        }

        PlayersChanged?.Invoke(this, EventArgs.Empty);
    }

    public void LoadPlayers(IEnumerable<PlayerState> players)
    {
        Players.Clear();
        foreach (var player in players)
        {
            Players.Add(player);
        }

        ActivePlayer = Players.FirstOrDefault();
        foreach (var player in Players)
        {
            player.IsActive = player == ActivePlayer;
        }

        PlayersChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RenamePlayer(PlayerState player, string newName)
    {
        player.Profile.Name = newName;
        PlayersChanged?.Invoke(this, EventArgs.Empty);
    }

    public void InitializeMode()
    {
        foreach (var player in Players)
        {
            switch (CurrentMode.Type)
            {
                case GameModeType.Checkout170:
                    player.RemainingScore = 170;
                    break;
                case GameModeType.Ladder123:
                    player.RemainingScore = player.LadderTarget;
                    player.LadderAttempts = 0;
                    break;
                case GameModeType.TargetFocus:
                    player.TargetFocusDartsThrown = 0;
                    player.TargetFocusHits = 0;
                    break;
                case GameModeType.DoubleFinishRoutine:
                    player.RemainingScore = player.DoubleRoutineTarget * 2;
                    break;
                case GameModeType.FiftyThreeCheckout:
                    player.FiftyThreeRemaining = 53;
                    player.RemainingScore = player.FiftyThreeRemaining;
                    break;
            }

            player.DartsInCurrentTurn = 0;
        }
    }

    public void SetActivePlayer(PlayerState player)
    {
        if (!Players.Contains(player))
        {
            return;
        }

        foreach (var p in Players)
        {
            p.IsActive = p == player;
        }

        ActivePlayer = player;
    }

    public PlayerState? AdvancePlayer()
    {
        if (ActivePlayer == null || Players.Count == 0)
        {
            return null;
        }

        var index = Players.IndexOf(ActivePlayer);
        var nextIndex = (index + 1) % Players.Count;
        SetActivePlayer(Players[nextIndex]);
        return ActivePlayer;
    }

    public GameTurnResult ApplyTurn(PlayerState player, IReadOnlyList<DartThrow> darts)
    {
        var scoreBeforeTurn = player.RemainingScore;
        var ladderTargetBefore = player.LadderTarget;
        var ladderAttemptsBefore = player.LadderAttempts;
        var targetFocusHitsBefore = player.TargetFocusHits;
        var targetFocusDartsBefore = player.TargetFocusDartsThrown;
        var fiftyThreeBefore = player.FiftyThreeRemaining;
        var remainingScoresBefore = Players.ToDictionary(p => p.Profile.Id, p => p.RemainingScore);
        var fiftyThreeScoresBefore = Players.ToDictionary(p => p.Profile.Id, p => p.FiftyThreeRemaining);

        GameTurnResult turnResult = CurrentMode.Type switch
        {
            GameModeType.Checkout170 => ApplyCheckoutTurn(player, darts, 170, true, resetOnCheckout: true),
            GameModeType.Ladder123 => ApplyLadderTurn(player, darts),
            GameModeType.TargetFocus => ApplyTargetFocusTurn(player, darts),
            GameModeType.DoubleFinishRoutine => ApplyCheckoutTurn(player, darts, player.DoubleRoutineTarget * 2, true, resetOnCheckout: false),
            GameModeType.FiftyThreeCheckout => ApplyCheckoutTurn(player, darts, player.FiftyThreeRemaining, true, resetOnCheckout: false, customStart: 53),
            _ => new GameTurnResult()
        };

        switch (CurrentMode.Type)
        {
            case GameModeType.DoubleFinishRoutine when turnResult.Checkout:
                player.RemainingScore = player.DoubleRoutineTarget * 2;
                break;
            case GameModeType.FiftyThreeCheckout:
                if (turnResult.Checkout)
                {
                    player.RemainingScore = 53;
                    player.FiftyThreeRemaining = 53;
                }
                else if (turnResult.Bust)
                {
                    player.FiftyThreeRemaining = 53;
                }
                else
                {
                    player.FiftyThreeRemaining = player.RemainingScore;
                }

                break;
        }

        foreach (var result in turnResult.Darts)
        {
            player.DartHistory.Add(result);
            player.Statistics.RegisterDart(result);
        }

        player.DartsInCurrentTurn = 0;

        _history.Push(new GameHistoryEntry(player, turnResult.Darts, CurrentMode.Type, turnResult.Bust, turnResult.Checkout,
            scoreBeforeTurn, ladderTargetBefore, ladderAttemptsBefore, targetFocusHitsBefore, targetFocusDartsBefore, fiftyThreeBefore,
            remainingScoresBefore, fiftyThreeScoresBefore));

        return turnResult;
    }

    public GameHistoryEntry? UndoLastTurn()
    {
        if (_history.Count == 0)
        {
            return null;
        }

        var entry = _history.Pop();
        var player = entry.Player;
        foreach (var kv in entry.RemainingScoresBefore)
        {
            var target = Players.FirstOrDefault(p => p.Profile.Id == kv.Key);
            if (target != null)
            {
                target.RemainingScore = kv.Value;
            }
        }

        foreach (var kv in entry.FiftyThreeScoresBefore)
        {
            var target = Players.FirstOrDefault(p => p.Profile.Id == kv.Key);
            if (target != null)
            {
                target.FiftyThreeRemaining = kv.Value;
            }
        }

        player.DartsInCurrentTurn = 0;
        player.RemainingScore = entry.ScoreBeforeTurn;
        player.LadderTarget = entry.LadderTargetBefore;
        player.LadderAttempts = entry.LadderAttemptsBefore;
        player.TargetFocusHits = entry.TargetFocusHitsBefore;
        player.TargetFocusDartsThrown = entry.TargetFocusDartsBefore;
        player.FiftyThreeRemaining = entry.FiftyThreeBefore;

        foreach (var dart in entry.Darts.Reverse())
        {
            if (player.DartHistory.Count > 0)
            {
                player.DartHistory.RemoveAt(player.DartHistory.Count - 1);
            }

            player.Statistics.UndoDart(dart);
        }

        return entry;
    }

    private GameTurnResult ApplyCheckoutTurn(PlayerState player, IReadOnlyList<DartThrow> darts, int startingScore, bool requireDoubleOut,
        bool resetOnCheckout, int? customStart = null)
    {
        if (player.RemainingScore == 0)
        {
            player.RemainingScore = startingScore;
        }

        var turnResult = new GameTurnResult();
        var initialScore = player.RemainingScore == 0 && customStart.HasValue ? customStart.Value : player.RemainingScore;
        if (initialScore == 0)
        {
            initialScore = startingScore;
            player.RemainingScore = initialScore;
        }

        int scoreBefore = player.RemainingScore;
        bool bust = false;

        foreach (var dart in darts)
        {
            var result = new DartResult(dart, player.RemainingScore, player.RemainingScore)
            {
                CountsTowardsFirstNine = player.DartHistory.Count + turnResult.Darts.Count < 9
            };

            if (bust)
            {
                dart.IsBust = true;
                dart.Score = 0;
                turnResult.Darts.Add(result);
                continue;
            }

            dart.IsDoubleAttempt = dart.Segment is DartSegmentType.Double or DartSegmentType.InnerBull;
            if (dart.Segment == DartSegmentType.Double || dart.Segment == DartSegmentType.InnerBull)
            {
                dart.IsDoubleHit = true;
            }

            int tentative = player.RemainingScore - dart.Score;
            bool checkoutAttempt = requireDoubleOut && player.RemainingScore <= startingScore;
            dart.IsCheckoutAttempt = checkoutAttempt;

            if (tentative < 0)
            {
                bust = true;
            }
            else if (tentative == 0)
            {
                if (requireDoubleOut && !(dart.Segment == DartSegmentType.Double || dart.Segment == DartSegmentType.InnerBull))
                {
                    bust = true;
                }
                else
                {
                    dart.IsCheckout = true;
                    result.EndsTurn = true;
                    result.EndsLeg = true;
                    player.RemainingScore = 0;
                    result.ScoreAfter = 0;
                    turnResult.Checkout = true;
                    turnResult.Winner = player;
                }
            }
            else if (requireDoubleOut && tentative == 1)
            {
                bust = true;
            }
            else
            {
                player.RemainingScore = tentative;
                result.ScoreAfter = tentative;
            }

            if (bust)
            {
                dart.IsBust = true;
                dart.Score = 0;
                player.RemainingScore = scoreBefore;
                result.ScoreAfter = scoreBefore;
                turnResult.Bust = true;
            }

            turnResult.Darts.Add(result);

            if (turnResult.Checkout)
            {
                break;
            }
        }

        if (turnResult.Checkout && resetOnCheckout)
        {
            foreach (var p in Players)
            {
                p.RemainingScore = startingScore;
                p.DartsInCurrentTurn = 0;
            }
        }
        else if (turnResult.Checkout && customStart.HasValue)
        {
            player.RemainingScore = customStart.Value;
            player.FiftyThreeRemaining = customStart.Value;
        }

        if (turnResult.Bust)
        {
            player.RemainingScore = scoreBefore;
            foreach (var recorded in turnResult.Darts)
            {
                recorded.Dart.IsBust = true;
                recorded.Dart.Score = 0;
                recorded.ScoreAfter = scoreBefore;
            }
        }

        return turnResult;
    }

    private GameTurnResult ApplyLadderTurn(PlayerState player, IReadOnlyList<DartThrow> darts)
    {
        var turnResult = ApplyCheckoutTurn(player, darts, player.LadderTarget, true, resetOnCheckout: false);
        if (turnResult.Checkout)
        {
            player.LadderTarget++;
            player.LadderAttempts = 0;
            player.RemainingScore = player.LadderTarget;
        }
        else
        {
            player.LadderAttempts += darts.Count;
            if (player.LadderAttempts >= 9)
            {
                if (player.LadderTarget > 123)
                {
                    player.LadderTarget--;
                }

                player.LadderAttempts = 0;
                player.RemainingScore = player.LadderTarget;
            }
        }

        return turnResult;
    }

    private GameTurnResult ApplyTargetFocusTurn(PlayerState player, IReadOnlyList<DartThrow> darts)
    {
        var turnResult = new GameTurnResult();
        foreach (var dart in darts)
        {
            var result = new DartResult(dart, player.TargetFocusHits, player.TargetFocusHits)
            {
                CountsTowardsFirstNine = false
            };

            player.TargetFocusDartsThrown++;
            if (dart.Number == player.TargetFocusNumber && dart.Segment != DartSegmentType.Miss && dart.Score > 0)
            {
                player.TargetFocusHits++;
            }

            if (player.TargetFocusDartsThrown >= player.TargetFocusTotalShots)
            {
                result.EndsTurn = true;
            }

            turnResult.Darts.Add(result);
        }

        return turnResult;
    }

    private GameTurnResult ApplyCheckoutTurn(PlayerState player, IReadOnlyList<DartThrow> darts, int startingScore, bool requireDoubleOut,
        bool resetOnCheckout)
    {
        return ApplyCheckoutTurn(player, darts, startingScore, requireDoubleOut, resetOnCheckout, null);
    }
}
