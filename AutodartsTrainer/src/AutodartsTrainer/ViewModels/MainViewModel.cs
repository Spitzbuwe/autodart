using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using AutodartsTrainer.Models;
using AutodartsTrainer.Services;

namespace AutodartsTrainer.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private const int MaxTurnDarts = 3;
    private readonly GameCoordinator _gameCoordinator;
    private readonly ThemeService _themeService;
    private readonly LocalizationService _localizationService;
    private readonly AudioService _audioService;
    private readonly CameraService _cameraService;
    private readonly PersistenceService _persistenceService;
    private readonly ExportService _exportService;

    private PlayerViewModel? _selectedPlayer;
    private GameModeDefinition _selectedGameMode;
    private string _manualScoreInput = string.Empty;

    public MainViewModel(GameCoordinator gameCoordinator, ThemeService themeService, LocalizationService localizationService,
        AudioService audioService, CameraService cameraService, PersistenceService persistenceService, ExportService exportService)
    {
        _gameCoordinator = gameCoordinator;
        _themeService = themeService;
        _localizationService = localizationService;
        _audioService = audioService;
        _cameraService = cameraService;
        _persistenceService = persistenceService;
        _exportService = exportService;

        _selectedGameMode = _gameCoordinator.CurrentMode;

        AddPlayerCommand = new RelayCommand(AddPlayer, CanAddPlayer);
        RemovePlayerCommand = new RelayCommand<PlayerViewModel?>(RemovePlayer, player => player != null);
        ClearTurnCommand = new RelayCommand(ClearTurn, () => PendingDarts.Any());
        AddSegmentCommand = new RelayCommand<string>(AddSegment);
        AddManualScoreCommand = new RelayCommand(AddManualScore, CanAddManualScore);
        ConfirmTurnCommand = new AsyncRelayCommand(ConfirmTurnAsync, () => PendingDarts.Any());
        UndoCommand = new RelayCommand(UndoTurn, CanUndo);
        NextPlayerCommand = new RelayCommand(MoveToNextPlayer, () => Players.Count > 1);
        ToggleThemeCommand = new RelayCommand(ToggleTheme);
        ToggleAudioCommand = new RelayCommand(ToggleAudio);
        ToggleLanguageCommand = new RelayCommand(ToggleLanguage);
        ExportJsonCommand = new AsyncRelayCommand(ExportJsonAsync, () => Players.Any());
        ExportCsvCommand = new AsyncRelayCommand(ExportCsvAsync, () => Players.Any());
        DetectCamerasCommand = new RelayCommand(DetectCameras);

        RefreshPlayers();
        DetectCameras();
    }

    public ObservableCollection<PlayerViewModel> Players { get; } = new();

    public ObservableCollection<DartThrow> PendingDarts { get; } = new();

    public ObservableCollection<string> SessionHistory { get; } = new();

    public ObservableCollection<DartResult> RecentDarts { get; } = new();

    public IReadOnlyList<GameModeDefinition> GameModes => _gameCoordinator.Modes;

    public IEnumerable<int> TargetFocusOptions { get; } = new[] { 20, 19, 18, 17 };

    public IEnumerable<int> DoubleRoutineOptions { get; } = new[] { 16, 8, 4, 2, 1 };

    public IReadOnlyList<string> SegmentButtons { get; } = BuildSegmentButtons();

    public PlayerViewModel? SelectedPlayer
    {
        get => _selectedPlayer;
        set
        {
            if (SetProperty(ref _selectedPlayer, value))
            {
                if (value != null)
                {
                    _gameCoordinator.SetActivePlayer(value.State);
                }

                RefreshPlayers();
            }
        }
    }

    public GameModeDefinition SelectedGameMode
    {
        get => _selectedGameMode;
        set
        {
            if (SetProperty(ref _selectedGameMode, value))
            {
                _gameCoordinator.SetMode(value.Type);
                RefreshPlayers();
                OnPropertyChanged(nameof(CurrentModeDescription));
            }
        }
    }

    public string CurrentModeDescription => _localizationService[_selectedGameMode.DescriptionResourceKey];

    public string ManualScoreInput
    {
        get => _manualScoreInput;
        set
        {
            if (SetProperty(ref _manualScoreInput, value))
            {
                AddManualScoreCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string TurnProgress => string.Format(CultureInfo.CurrentCulture, "{0}/3", PendingDarts.Count);

    public bool AudioEnabled
    {
        get => _audioService.IsEnabled;
        set
        {
            if (_audioService.IsEnabled != value)
            {
                _audioService.Enable(value);
                OnPropertyChanged();
            }
        }
    }

    public ThemeVariant CurrentTheme => _themeService.CurrentTheme;

    public string CurrentLanguage => _localizationService.CurrentCulture.TwoLetterISOLanguageName;

    public bool CamerasAvailable { get; private set; }

    public IReadOnlyList<string> ConnectedCameras { get; private set; } = Array.Empty<string>();

    public string CameraStatusText => CamerasAvailable
        ? _localizationService["CameraAvailable"]
        : _localizationService["CameraUnavailable"];

    public RelayCommand AddPlayerCommand { get; }

    public RelayCommand<PlayerViewModel?> RemovePlayerCommand { get; }

    public RelayCommand ClearTurnCommand { get; }

    public RelayCommand<string> AddSegmentCommand { get; }

    public RelayCommand AddManualScoreCommand { get; }

    public AsyncRelayCommand ConfirmTurnCommand { get; }

    public RelayCommand UndoCommand { get; }

    public RelayCommand NextPlayerCommand { get; }

    public RelayCommand ToggleThemeCommand { get; }

    public RelayCommand ToggleAudioCommand { get; }

    public RelayCommand ToggleLanguageCommand { get; }

    public AsyncRelayCommand ExportJsonCommand { get; }

    public AsyncRelayCommand ExportCsvCommand { get; }

    public RelayCommand DetectCamerasCommand { get; }

    private string _newPlayerName = string.Empty;

    public string NewPlayerName
    {
        get => _newPlayerName;
        set
        {
            if (SetProperty(ref _newPlayerName, value))
            {
                AddPlayerCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public async Task InitializeAsync()
    {
        await LoadSessionAsync();
        _themeService.ApplyTheme(_themeService.CurrentTheme);
        RefreshPlayers();
        OnPropertyChanged(nameof(CurrentTheme));
        OnPropertyChanged(nameof(CurrentLanguage));
        OnPropertyChanged(nameof(CameraStatusText));
        AudioEnabled = _audioService.IsEnabled;
        OnPropertyChanged(nameof(AudioEnabled));
    }

    public async Task ShutdownAsync()
    {
        var snapshot = new SessionSnapshot
        {
            Players = _gameCoordinator.Players.Select(p => new PlayerSnapshot(p)).ToList(),
            Settings = new SettingsSnapshot(CurrentLanguage, CurrentTheme.ToString(), AudioEnabled, SelectedGameMode.Type)
        };

        await _persistenceService.SaveAsync(snapshot);
    }

    private async Task LoadSessionAsync()
    {
        var snapshot = await _persistenceService.LoadAsync();
        if (snapshot == null)
        {
            return;
        }

        var theme = Enum.TryParse<ThemeVariant>(snapshot.Settings.Theme, out var parsedTheme) ? parsedTheme : ThemeVariant.Dark;
        _themeService.ApplyTheme(theme);
        _audioService.Enable(snapshot.Settings.AudioEnabled);
        _localizationService.SetLanguage(snapshot.Settings.Language);
        _gameCoordinator.SetMode(snapshot.Settings.ActiveGameMode);
        var players = snapshot.Players.Select(p => p.ToPlayerState()).ToList();
        if (players.Any())
        {
            _gameCoordinator.LoadPlayers(players);
        }
        _selectedGameMode = _gameCoordinator.Modes.First(m => m.Type == snapshot.Settings.ActiveGameMode);
        OnPropertyChanged(nameof(SelectedGameMode));
        OnPropertyChanged(nameof(CurrentModeDescription));
        RefreshPlayers();
        OnPropertyChanged(nameof(CameraStatusText));
    }

    private bool CanAddPlayer() => !string.IsNullOrWhiteSpace(NewPlayerName);

    private void AddPlayer()
    {
        if (string.IsNullOrWhiteSpace(NewPlayerName))
        {
            return;
        }

        var state = _gameCoordinator.AddPlayer(NewPlayerName.Trim());
        Players.Add(new PlayerViewModel(state));
        NewPlayerName = string.Empty;
        AddPlayerCommand.NotifyCanExecuteChanged();
        RefreshPlayers();
    }

    private void RemovePlayer(PlayerViewModel? player)
    {
        if (player == null)
        {
            return;
        }

        _gameCoordinator.RemovePlayer(player.State);
        Players.Remove(player);
        if (SelectedPlayer == player)
        {
            SelectedPlayer = Players.FirstOrDefault();
        }

        RefreshPlayers();
    }

    private void RefreshPlayers()
    {
        foreach (var state in _gameCoordinator.Players)
        {
            var existing = Players.FirstOrDefault(p => p.State == state);
            if (existing == null)
            {
                Players.Add(new PlayerViewModel(state));
            }
            else
            {
                existing.Refresh();
            }
        }

        for (int i = Players.Count - 1; i >= 0; i--)
        {
            if (!_gameCoordinator.Players.Contains(Players[i].State))
            {
                Players.RemoveAt(i);
            }
        }

        foreach (var player in Players)
        {
            player.IsActive = _gameCoordinator.ActivePlayer == player.State;
            player.Refresh();
        }

        var active = _gameCoordinator.ActivePlayer;
        if (active != null)
        {
            var vm = Players.FirstOrDefault(p => p.State == active);
            if (vm != null && !ReferenceEquals(_selectedPlayer, vm))
            {
                _selectedPlayer = vm;
                OnPropertyChanged(nameof(SelectedPlayer));
            }
        }
        else if (_selectedPlayer == null && Players.Any())
        {
            _selectedPlayer = Players.First();
            OnPropertyChanged(nameof(SelectedPlayer));
        }

        ConfirmTurnCommand.NotifyCanExecuteChanged();
        ClearTurnCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        NextPlayerCommand.NotifyCanExecuteChanged();
        ExportJsonCommand.NotifyCanExecuteChanged();
        ExportCsvCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(TurnProgress));
    }

    private void ClearTurn()
    {
        PendingDarts.Clear();
        ManualScoreInput = string.Empty;
        OnPropertyChanged(nameof(TurnProgress));
        ConfirmTurnCommand.NotifyCanExecuteChanged();
        ClearTurnCommand.NotifyCanExecuteChanged();
    }

    private void AddSegment(string? segment)
    {
        if (segment == null || PendingDarts.Count >= MaxTurnDarts)
        {
            return;
        }

        var dart = segment switch
        {
            "BULL" => DartThrow.OuterBull(),
            "D-BULL" => DartThrow.InnerBull(),
            "MISS" => DartThrow.Miss(),
            "BUST" => CreateBustDart(),
            _ => ParseSegment(segment)
        };

        PendingDarts.Add(dart);
        ManualScoreInput = string.Empty;
        OnPropertyChanged(nameof(TurnProgress));
        ConfirmTurnCommand.NotifyCanExecuteChanged();
        ClearTurnCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
    }

    private bool CanAddManualScore()
    {
        if (PendingDarts.Count >= MaxTurnDarts)
        {
            return false;
        }

        return int.TryParse(ManualScoreInput, CultureInfo.InvariantCulture, out var value) && value >= 0 && value <= 60;
    }

    private void AddManualScore()
    {
        if (!int.TryParse(ManualScoreInput, CultureInfo.InvariantCulture, out var value))
        {
            return;
        }

        value = Math.Clamp(value, 0, 60);
        PendingDarts.Add(DartThrow.Manual(value));
        ManualScoreInput = string.Empty;
        OnPropertyChanged(nameof(TurnProgress));
        ConfirmTurnCommand.NotifyCanExecuteChanged();
        ClearTurnCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
    }

    private DartThrow ParseSegment(string segment)
    {
        if (segment.Length < 2)
        {
            return DartThrow.Miss();
        }

        var prefix = segment[0];
        if (!int.TryParse(segment.AsSpan(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            return DartThrow.Miss();
        }

        return prefix switch
        {
            'S' => DartThrow.Single(value),
            'D' => DartThrow.Double(value),
            'T' => DartThrow.Triple(value),
            _ => DartThrow.Miss()
        };
    }

    private DartThrow CreateBustDart()
    {
        var active = SelectedPlayer ?? Players.FirstOrDefault();
        var remaining = active?.State.RemainingScore ?? 0;
        return DartThrow.Manual(remaining + 1);
    }

    private async Task ConfirmTurnAsync()
    {
        if (SelectedPlayer == null || !PendingDarts.Any())
        {
            return;
        }

        var darts = PendingDarts.Select(CloneDart).ToList();
        PendingDarts.Clear();
        OnPropertyChanged(nameof(TurnProgress));
        ConfirmTurnCommand.NotifyCanExecuteChanged();
        ClearTurnCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();

        var result = _gameCoordinator.ApplyTurn(SelectedPlayer.State, darts);
        if (result.Checkout)
        {
            _audioService.PlayCheckout();
        }
        else if (result.Bust)
        {
            _audioService.PlayBust();
        }

        UpdateRecentHistory(SelectedPlayer, result);

        if (!result.Checkout && Players.Count > 1)
        {
            var next = _gameCoordinator.AdvancePlayer();
            if (next != null)
            {
                SelectedPlayer = Players.FirstOrDefault(p => p.State == next);
            }
        }

        RefreshPlayers();
        UndoCommand.NotifyCanExecuteChanged();

        await Task.CompletedTask;
    }

    private void UpdateRecentHistory(PlayerViewModel player, GameTurnResult result)
    {
        var builder = new StringBuilder();
        builder.Append(player.Name);
        builder.Append(':');
        builder.Append(' ');
        builder.Append(string.Join(", ", result.Darts.Select(d => d.Dart.DisplayText)));
        builder.Append(" => ");
        builder.Append(player.State.RemainingScore);
        if (result.Checkout)
        {
            builder.Append(" ✔");
        }
        else if (result.Bust)
        {
            builder.Append(" ✖");
        }

        SessionHistory.Add(builder.ToString());
        while (SessionHistory.Count > 40)
        {
            SessionHistory.RemoveAt(0);
        }

        RecentDarts.Clear();
        foreach (var dart in player.State.DartHistory.TakeLast(9))
        {
            RecentDarts.Add(dart);
        }
    }

    private void UndoTurn()
    {
        if (PendingDarts.Any())
        {
            PendingDarts.RemoveAt(PendingDarts.Count - 1);
            OnPropertyChanged(nameof(TurnProgress));
            ConfirmTurnCommand.NotifyCanExecuteChanged();
            ClearTurnCommand.NotifyCanExecuteChanged();
            UndoCommand.NotifyCanExecuteChanged();
            return;
        }

        var entry = _gameCoordinator.UndoLastTurn();
        if (entry == null)
        {
            return;
        }

        if (SessionHistory.Any())
        {
            SessionHistory.RemoveAt(SessionHistory.Count - 1);
        }

        RefreshPlayers();
        SelectedPlayer = Players.FirstOrDefault(p => p.State == entry.Player);
        UndoCommand.NotifyCanExecuteChanged();
    }

    private bool CanUndo() => PendingDarts.Any() || _gameCoordinator.CanUndo;

    private void MoveToNextPlayer()
    {
        var next = _gameCoordinator.AdvancePlayer();
        if (next != null)
        {
            SelectedPlayer = Players.FirstOrDefault(p => p.State == next);
        }

        RefreshPlayers();
    }

    private void ToggleTheme()
    {
        _themeService.ToggleTheme();
        OnPropertyChanged(nameof(CurrentTheme));
    }

    private void ToggleAudio()
    {
        AudioEnabled = !AudioEnabled;
        OnPropertyChanged(nameof(AudioEnabled));
    }

    private void ToggleLanguage()
    {
        var nextCulture = _localizationService.SupportedCultures
            .SkipWhile(c => c.Name != _localizationService.CurrentCulture.Name)
            .Skip(1)
            .FirstOrDefault() ?? _localizationService.SupportedCultures.First();
        _localizationService.SetLanguage(nextCulture.Name);
        OnPropertyChanged(nameof(CurrentLanguage));
        OnPropertyChanged(nameof(CurrentModeDescription));
        OnPropertyChanged(nameof(CameraStatusText));
        RefreshPlayers();
    }

    private async Task ExportJsonAsync()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "JSON (*.json)|*.json",
            FileName = "autodartstrainer-session.json"
        };

        if (dialog.ShowDialog() == true)
        {
            await _exportService.ExportJsonAsync(_gameCoordinator.Players, SelectedGameMode.Type, dialog.FileName);
        }
    }

    private async Task ExportCsvAsync()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv",
            FileName = "autodartstrainer-session.csv"
        };

        if (dialog.ShowDialog() == true)
        {
            await _exportService.ExportCsvAsync(_gameCoordinator.Players, dialog.FileName);
        }
    }

    private void DetectCameras()
    {
        var devices = _cameraService.DetectConnectedCameras();
        ConnectedCameras = devices;
        CamerasAvailable = devices.Any();
        OnPropertyChanged(nameof(ConnectedCameras));
        OnPropertyChanged(nameof(CamerasAvailable));
        OnPropertyChanged(nameof(CameraStatusText));
    }

    private static DartThrow CloneDart(DartThrow original)
    {
        var clone = new DartThrow(original.Segment, original.Number, original.Score)
        {
            IsBust = original.IsBust,
            IsCheckout = original.IsCheckout,
            IsCheckoutAttempt = original.IsCheckoutAttempt,
            IsDoubleAttempt = original.IsDoubleAttempt,
            IsDoubleHit = original.IsDoubleHit
        };
        return clone;
    }

    private static IReadOnlyList<string> BuildSegmentButtons()
    {
        var result = new List<string>();
        for (var i = 1; i <= 20; i++)
        {
            result.Add($"S{i}");
            result.Add($"D{i}");
            result.Add($"T{i}");
        }

        result.Add("BULL");
        result.Add("D-BULL");
        result.Add("MISS");
        result.Add("BUST");
        return result;
    }
}
