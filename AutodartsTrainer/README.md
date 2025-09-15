# AutodartsTrainer

AutodartsTrainer ist eine Windows-WPF-Anwendung auf Basis von .NET 7 zur Unterstützung von Checkout- und Trainingsprogrammen für Darts. Die App ist vollständig manuell bedienbar, erkennt optional angeschlossene Kameras und speichert Sitzungen automatisch.

## Features

- **Spielmodi**
  - 170-Checkout (Double-Out, Neustart bei 170 nach Checkout)
  - 123-Leiter mit Auf-/Abstieg je nach Checkout-Erfolg
  - Trainingsprogramme: Zielschießen (20-17), Doppelroutine (D16-D1) und 53-Punkte-Checkout
- **Mehrspieler**: Spieler hinzufügen, umbenennen, entfernen; automatischer Spielerwechsel nach jeder Aufnahme
- **Manuelle Eingabe**: Zahlentasten (Score), Buttons für S/T/D1-20, Bull, Miss, Bust; Undo pro Dart
- **Statistiken**: Gesamt- und First-9-Average, Checkout- und Doppelquote, Legs gewonnen
- **UX**: Modernes UI mit Dark/Light Theme, Hotkeys (Enter, Backspace, N, F11), Audiobestätigung für Checkout/Bust
- **Datenhaltung**: Automatisches Laden/Speichern der Session (JSON), Export nach JSON/CSV
- **Lokalisierung**: Deutsch/Englisch via Ressourcen

## Projektstruktur

```
AutodartsTrainer/
├── AutodartsTrainer.sln
├── README.md
├── scripts/
│   ├── setup.ps1       # .NET SDK Installation & Restore
│   └── publish.ps1     # Self-Contained Publish (win-x64)
└── src/
    └── AutodartsTrainer/
        ├── App.xaml(+.cs)
        ├── MainWindow.xaml(+.cs)
        ├── Models/
        ├── ViewModels/
        ├── Services/
        ├── Converters/
        ├── Resources/
        └── AutodartsTrainer.csproj
```

## Entwickeln & Bauen

1. **Voraussetzungen**: Windows 10+, PowerShell, .NET 7 SDK (Installationsskript beiliegend).
2. **SDK installieren & Restore ausführen**
   ```powershell
   cd scripts
   .\setup.ps1
   ```
3. **Projekt öffnen**
   - Visual Studio 2022: `AutodartsTrainer.sln`
   - Oder mit VS Code + C# Dev Kit
4. **Starten**
   ```powershell
   dotnet run --project src/AutodartsTrainer/AutodartsTrainer.csproj
   ```
5. **Self-contained Build**
   ```powershell
   cd scripts
   .\publish.ps1 -Configuration Release
   ```
   Das Ergebnis liegt anschließend im Ordner `src/AutodartsTrainer/bin/Release/net7.0-windows10.0.19041.0/win-x64/publish`.

## Hinweise

- Die App speichert Sitzungs- und Spielerdaten unter `%APPDATA%\AutodartsTrainer\session.json`.
- Kameras werden über WMI erkannt; bei fehlenden Berechtigungen läuft die App vollständig im manuellen Modus.
- Themes lassen sich per Button im Header wechseln (Light/Dark).

