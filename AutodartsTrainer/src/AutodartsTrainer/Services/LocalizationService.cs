using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;

namespace AutodartsTrainer.Services;

public class LocalizationService : INotifyPropertyChanged
{
    private readonly ResourceManager _resourceManager = new("AutodartsTrainer.Resources.Strings.StringResources", typeof(LocalizationService).Assembly);
    private CultureInfo _currentCulture = CultureInfo.GetCultureInfo("de");

    public event PropertyChangedEventHandler? PropertyChanged;

    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        private set
        {
            if (!_currentCulture.Equals(value))
            {
                _currentCulture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            }
        }
    }

    public IReadOnlyList<CultureInfo> SupportedCultures { get; } = new[]
    {
        CultureInfo.GetCultureInfo("de"),
        CultureInfo.GetCultureInfo("en")
    };

    public string GetString(string key)
    {
        var value = _resourceManager.GetString(key, _currentCulture);
        return value ?? key;
    }

    public string this[string key] => GetString(key);

    public void SetLanguage(string cultureName)
    {
        var culture = SupportedCultures.FirstOrDefault(c => c.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase));
        if (culture == null)
        {
            culture = CultureInfo.GetCultureInfo("de");
        }

        CurrentCulture = culture;
    }
}
