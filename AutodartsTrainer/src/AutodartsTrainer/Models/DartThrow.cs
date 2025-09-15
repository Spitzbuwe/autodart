using System.Globalization;

namespace AutodartsTrainer.Models;

public class DartThrow
{
    public DartThrow(DartSegmentType segment, int number, int score)
    {
        Segment = segment;
        Number = number;
        Score = score;
        Timestamp = DateTime.Now;
    }

    public DartSegmentType Segment { get; }

    /// <summary>
    /// Number on the dartboard for singles/doubles/triples (1-20). For bulls or manual entries this is 0.
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Score awarded for this dart. For busts this is set to 0.
    /// </summary>
    public int Score { get; set; }

    public DateTime Timestamp { get; }

    public bool IsBust { get; set; }

    public bool IsCheckout { get; set; }

    public bool IsCheckoutAttempt { get; set; }

    public bool IsDoubleAttempt { get; set; }

    public bool IsDoubleHit { get; set; }

    public string DisplayText => Segment switch
    {
        DartSegmentType.Single when Number > 0 => $"S{Number}",
        DartSegmentType.Double when Number > 0 => $"D{Number}",
        DartSegmentType.Triple when Number > 0 => $"T{Number}",
        DartSegmentType.OuterBull => "BULL",
        DartSegmentType.InnerBull => "D-BULL",
        DartSegmentType.Miss => "MISS",
        DartSegmentType.Manual => Score.ToString(CultureInfo.InvariantCulture),
        _ => Score.ToString(CultureInfo.InvariantCulture)
    };

    public static DartThrow Miss() => new(DartSegmentType.Miss, 0, 0);

    public static DartThrow Manual(int score) => new(DartSegmentType.Manual, 0, score);

    public static DartThrow Single(int number) => new(DartSegmentType.Single, number, number);

    public static DartThrow Double(int number) => new(DartSegmentType.Double, number, number * 2);

    public static DartThrow Triple(int number) => new(DartSegmentType.Triple, number, number * 3);

    public static DartThrow OuterBull() => new(DartSegmentType.OuterBull, 0, 25);

    public static DartThrow InnerBull() => new(DartSegmentType.InnerBull, 0, 50);
}
