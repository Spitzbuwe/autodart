using System.Text;
using System.Text.Json;
using AutodartsTrainer.Models;

namespace AutodartsTrainer.Services;

public class ExportService
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    public async Task ExportJsonAsync(IEnumerable<PlayerState> players, GameModeType mode, string path, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Mode = mode.ToString(),
            Timestamp = DateTime.Now,
            Players = players.Select(p => new
            {
                p.Profile.Name,
                p.RemainingScore,
                Statistics = new
                {
                    p.Statistics.TotalScore,
                    p.Statistics.DartsThrown,
                    p.Statistics.OverallAverage,
                    p.Statistics.FirstNineAverage,
                    p.Statistics.CheckoutPercentage,
                    p.Statistics.DoublePercentage,
                    p.Statistics.CheckoutAttempts,
                    p.Statistics.CheckoutSuccess,
                    p.Statistics.DoubleAttempts,
                    p.Statistics.DoubleSuccess
                }
            })
        };

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, payload, _serializerOptions, cancellationToken);
    }

    public async Task ExportCsvAsync(IEnumerable<PlayerState> players, string path, CancellationToken cancellationToken = default)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Name;Remaining;Overall;First9;Checkout%;Double%");
        foreach (var player in players)
        {
            builder.AppendLine(string.Join(';', new[]
            {
                Escape(player.Profile.Name),
                player.RemainingScore.ToString(),
                player.Statistics.OverallAverage.ToString("F2"),
                player.Statistics.FirstNineAverage.ToString("F2"),
                player.Statistics.CheckoutPercentage.ToString("F2"),
                player.Statistics.DoublePercentage.ToString("F2")
            }));
        }

        await File.WriteAllTextAsync(path, builder.ToString(), cancellationToken);
    }

    private static string Escape(string value)
    {
        if (value.Contains(';'))
        {
            return $"\"{value}\"";
        }

        return value;
    }
}
