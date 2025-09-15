using System.Windows;
using AutodartsTrainer.Models;

namespace AutodartsTrainer.Services;

public class ThemeService
{
    private readonly Dictionary<ThemeVariant, Uri> _themeUris = new()
    {
        { ThemeVariant.Light, new Uri("Resources/Styles/LightTheme.xaml", UriKind.Relative) },
        { ThemeVariant.Dark, new Uri("Resources/Styles/DarkTheme.xaml", UriKind.Relative) }
    };

    public ThemeVariant CurrentTheme { get; private set; } = ThemeVariant.Dark;

    public void ApplyTheme(ThemeVariant theme)
    {
        if (!_themeUris.TryGetValue(theme, out var uri))
        {
            return;
        }

        if (Application.Current == null)
        {
            return;
        }

        var dictionaries = Application.Current.Resources.MergedDictionaries;
        var existing = dictionaries.FirstOrDefault(d => d.Source != null && _themeUris.Values.Contains(d.Source));
        if (existing != null)
        {
            dictionaries.Remove(existing);
        }

        dictionaries.Add(new ResourceDictionary { Source = uri });
        CurrentTheme = theme;
    }

    public void ToggleTheme() => ApplyTheme(CurrentTheme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark);
}
