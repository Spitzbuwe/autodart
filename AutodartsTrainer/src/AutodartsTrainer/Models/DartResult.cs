namespace AutodartsTrainer.Models;

public class DartResult
{
    public DartResult(DartThrow dart, int scoreBefore, int scoreAfter)
    {
        Dart = dart;
        ScoreBefore = scoreBefore;
        ScoreAfter = scoreAfter;
    }

    public DartThrow Dart { get; }

    public int ScoreBefore { get; }

    public int ScoreAfter { get; set; }

    public bool EndsTurn { get; set; }

    public bool EndsLeg { get; set; }

    public bool CountsTowardsFirstNine { get; set; }
}
