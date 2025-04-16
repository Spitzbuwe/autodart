# Autodarts Strafen-Monitor

Ein Programm zur automatischen Überwachung von Autodarts-Spielen, das Strafen für 26er und 180er Würfe verfolgt und in einer benutzerfreundlichen Oberfläche mit Dunkelmodus darstellt.

## Funktionen

- **Automatische Erkennung**: Erkennt 26er und 180er Würfe in Autodarts-Spielen automatisch
- **Strafenverfolgung**: Berechnet und verfolgt Strafen (10 Cent für 26er, 2 Euro für 180er)
- **Statistik-Dashboard**: Visualisiert die Strafen und Würfe über Zeit
- **Spielerverwaltung**: Verwalte Spieler mit Profilbildern und Gruppenfunktionen
- **Dunkelmodus**: Angenehmes Design für dunkle Umgebungen
- **Sound-Effekte**: Akustische Signale bei besonderen Würfen
- **Benachrichtigungen**: Möglichkeit für Discord und Telegram-Benachrichtigungen
- **Export/Import**: Daten als JSON, CSV oder Excel exportieren und importieren

## Installation

### Option 1: Ausführbare Datei (Windows)

1. Lade die neueste Version von GitHub herunter
2. Entpacke die ZIP-Datei
3. Führe `AutodartsStrafenMonitor.exe` aus

### Option 2: Aus dem Quellcode

1. Installiere Python 3.8 oder höher
2. Klone das Repository: `git clone https://github.com/yourusername/autodarts-strafen-monitor.git`
3. Wechsle in das Verzeichnis: `cd autodarts-strafen-monitor`
4. Installiere die Abhängigkeiten: `pip install -r requirements.txt`
5. Starte die Anwendung: `streamlit run app.py`

## EXE-Datei erstellen

Um eine eigenständige EXE-Datei für Windows zu erstellen:

1. Stelle sicher, dass Python 3.8 oder höher installiert ist
2. Installiere PyInstaller: `pip install pyinstaller`
3. Führe das Builder-Skript aus: `python autodarts_exe_builder.py`
4. Die ausführbare Datei wird im Ordner `dist` erstellt

## Verwendung

1. Starte die Anwendung
2. Die Anwendung versucht automatisch, Autodarts-Spiele zu erkennen
3. Klicke auf "Überwachung starten", um die automatische Erkennung zu aktivieren
4. Bei einem 26er oder 180er Wurf wird automatisch eine Strafe hinzugefügt
5. Du kannst Strafen auch manuell hinzufügen oder zurücksetzen

## Konfiguration

Die Anwendung verwendet folgende Standardeinstellungen:

- Standardspieler: "The Underdog" und "The Wildfly"
- Strafen: 10 Cent für einen 26er Wurf, 2 Euro für eine 180
- Datenbank: SQLite-Datenbank im `data`-Verzeichnis

## Troubleshooting

### Chrome wird nicht erkannt

Die Anwendung benötigt Google Chrome, um Autodarts-Spiele zu erkennen. Stelle sicher, dass:

1. Chrome installiert ist
2. Du die Überwachung über "Überwachung starten" aktiviert hast
3. Du zu autodarts.io navigierst und ein Spiel startest

### Daten werden nicht gespeichert

Die Anwendung speichert Daten in einer SQLite-Datenbank im `data`-Verzeichnis. Stelle sicher, dass:

1. Das Verzeichnis existiert und schreibbar ist
2. Die Anwendung mit ausreichenden Berechtigungen ausgeführt wird

## Lizenz

Dieses Projekt ist unter der MIT-Lizenz lizenziert - siehe die LICENSE-Datei für Details.