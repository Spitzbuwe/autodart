using System.Text.Json;
using AutodartsTrainer.Models;

namespace AutodartsTrainer.Services;

public class PersistenceService
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _sessionFile;

    public PersistenceService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder = Path.Combine(appData, "AutodartsTrainer");
        Directory.CreateDirectory(folder);
        _sessionFile = Path.Combine(folder, "session.json");
    }

    public async Task SaveAsync(SessionSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await using var stream = File.Create(_sessionFile);
        await JsonSerializer.SerializeAsync(stream, snapshot, _serializerOptions, cancellationToken);
    }

    public async Task<SessionSnapshot?> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_sessionFile))
        {
            return null;
        }

        await using var stream = File.OpenRead(_sessionFile);
        return await JsonSerializer.DeserializeAsync<SessionSnapshot>(stream, _serializerOptions, cancellationToken);
    }
}
