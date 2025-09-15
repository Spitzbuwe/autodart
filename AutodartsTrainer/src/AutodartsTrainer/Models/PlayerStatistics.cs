using System.Text.Json.Serialization;

namespace AutodartsTrainer.Models;

public class PlayerStatistics
{
    [JsonInclude]
    public int TotalScore { get; private set; }

    [JsonInclude]
    public int DartsThrown { get; private set; }

    [JsonInclude]
    public int FirstNineScore { get; private set; }

    [JsonInclude]
    public int FirstNineDarts { get; private set; }

    [JsonInclude]
    public int CheckoutAttempts { get; private set; }

    [JsonInclude]
    public int CheckoutSuccess { get; private set; }

    [JsonInclude]
    public int DoubleAttempts { get; private set; }

    [JsonInclude]
    public int DoubleSuccess { get; private set; }

    [JsonInclude]
    public int LegsWon { get; private set; }

    public double OverallAverage => DartsThrown == 0 ? 0 : Math.Round((double)TotalScore / DartsThrown * 3d, 2);

    public double FirstNineAverage => FirstNineDarts == 0 ? 0 : Math.Round((double)FirstNineScore / FirstNineDarts * 3d, 2);

    public double CheckoutPercentage => CheckoutAttempts == 0 ? 0 : Math.Round((double)CheckoutSuccess / CheckoutAttempts * 100d, 2);

    public double DoublePercentage => DoubleAttempts == 0 ? 0 : Math.Round((double)DoubleSuccess / DoubleAttempts * 100d, 2);

    public void RegisterDart(DartResult result)
    {
        if (result.Dart.IsBust)
        {
            DartsThrown++;
            if (DartsThrown <= 9)
            {
                FirstNineDarts++;
            }
            return;
        }

        TotalScore += result.Dart.Score;
        DartsThrown++;

        if (DartsThrown <= 9)
        {
            FirstNineScore += result.Dart.Score;
            FirstNineDarts++;
        }

        if (result.Dart.IsDoubleAttempt)
        {
            DoubleAttempts++;
            if (result.Dart.IsDoubleHit)
            {
                DoubleSuccess++;
            }
        }

        if (result.Dart.IsCheckoutAttempt)
        {
            CheckoutAttempts++;
            if (result.Dart.IsCheckout)
            {
                CheckoutSuccess++;
            }
        }

        if (result.EndsLeg)
        {
            LegsWon++;
        }
    }

    public void UndoDart(DartResult result)
    {
        if (DartsThrown > 0)
        {
            DartsThrown--;
        }

        if (!result.Dart.IsBust)
        {
            TotalScore -= result.Dart.Score;
        }

        if (result.Dart.IsBust && FirstNineDarts > 0)
        {
            FirstNineDarts--;
        }
        else if (result.Dart.Score > 0 && FirstNineDarts > 0)
        {
            FirstNineDarts--;
            FirstNineScore -= result.Dart.Score;
        }

        if (result.Dart.IsDoubleAttempt && DoubleAttempts > 0)
        {
            DoubleAttempts--;
            if (result.Dart.IsDoubleHit && DoubleSuccess > 0)
            {
                DoubleSuccess--;
            }
        }

        if (result.Dart.IsCheckoutAttempt && CheckoutAttempts > 0)
        {
            CheckoutAttempts--;
            if (result.Dart.IsCheckout && CheckoutSuccess > 0)
            {
                CheckoutSuccess--;
            }
        }

        if (result.EndsLeg && LegsWon > 0)
        {
            LegsWon--;
        }
    }

    public void Reset()
    {
        TotalScore = 0;
        DartsThrown = 0;
        FirstNineScore = 0;
        FirstNineDarts = 0;
        CheckoutAttempts = 0;
        CheckoutSuccess = 0;
        DoubleAttempts = 0;
        DoubleSuccess = 0;
        LegsWon = 0;
    }

    public void CopyFrom(PlayerStatistics other)
    {
        TotalScore = other.TotalScore;
        DartsThrown = other.DartsThrown;
        FirstNineScore = other.FirstNineScore;
        FirstNineDarts = other.FirstNineDarts;
        CheckoutAttempts = other.CheckoutAttempts;
        CheckoutSuccess = other.CheckoutSuccess;
        DoubleAttempts = other.DoubleAttempts;
        DoubleSuccess = other.DoubleSuccess;
        LegsWon = other.LegsWon;
    }
}
