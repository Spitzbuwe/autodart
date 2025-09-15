namespace AutodartsTrainer.Models;

public class GameTurnResult
{
    public List<DartResult> Darts { get; } = new();

    public bool Bust { get; set; }

    public bool Checkout { get; set; }

    public PlayerState? Winner { get; set; }
}
