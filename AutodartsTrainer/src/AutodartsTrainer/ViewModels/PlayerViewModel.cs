using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using AutodartsTrainer.Models;

namespace AutodartsTrainer.ViewModels;

public class PlayerViewModel : ObservableObject
{
    private readonly PlayerState _state;

    public PlayerViewModel(PlayerState state)
    {
        _state = state;
        Refresh();
    }

    public Guid Id => _state.Profile.Id;

    public string Name
    {
        get => _state.Profile.Name;
        set
        {
            if (_state.Profile.Name != value)
            {
                _state.Profile.Name = value;
                OnPropertyChanged();
            }
        }
    }

    public int RemainingScore
    {
        get => _state.RemainingScore;
        private set
        {
            if (_state.RemainingScore != value)
            {
                _state.RemainingScore = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsActive
    {
        get => _state.IsActive;
        set
        {
            if (_state.IsActive != value)
            {
                _state.IsActive = value;
                OnPropertyChanged();
            }
        }
    }

    public PlayerStatistics Statistics => _state.Statistics;

    public ObservableCollection<DartResult> History { get; } = new();

    public string LastThree => string.Join(" ", History.Reverse().Take(3).Select(h => h.Dart.DisplayText));

    public int LadderTarget
    {
        get => _state.LadderTarget;
        set
        {
            if (_state.LadderTarget != value)
            {
                _state.LadderTarget = value;
                OnPropertyChanged();
            }
        }
    }

    public int TargetFocusNumber
    {
        get => _state.TargetFocusNumber;
        set
        {
            if (_state.TargetFocusNumber != value)
            {
                _state.TargetFocusNumber = value;
                OnPropertyChanged();
            }
        }
    }

    public int TargetFocusRemaining => Math.Max(0, _state.TargetFocusTotalShots - _state.TargetFocusDartsThrown);

    public int TargetFocusHits => _state.TargetFocusHits;

    public int DoubleRoutineTarget
    {
        get => _state.DoubleRoutineTarget;
        set
        {
            if (_state.DoubleRoutineTarget != value)
            {
                _state.DoubleRoutineTarget = value;
                OnPropertyChanged();
            }
        }
    }

    public PlayerState State => _state;

    public void Refresh()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(RemainingScore));
        OnPropertyChanged(nameof(IsActive));
        OnPropertyChanged(nameof(Statistics));
        OnPropertyChanged(nameof(LadderTarget));
        OnPropertyChanged(nameof(TargetFocusNumber));
        OnPropertyChanged(nameof(TargetFocusRemaining));
        OnPropertyChanged(nameof(TargetFocusHits));
        OnPropertyChanged(nameof(DoubleRoutineTarget));
        History.Clear();
        foreach (var entry in _state.DartHistory)
        {
            History.Add(entry);
        }
        OnPropertyChanged(nameof(LastThree));
    }
}
