"""
Script zum Erstellen einer ausführbaren Windows-Datei für den Autodarts-Strafen-Monitor.
"""
import os
import sys
import shutil
import subprocess
import tempfile
import webbrowser
import time

# Basis-Verzeichnis und benötigte Ordner
DATA_DIR = "data"
RESOURCES_DIR = "resources"
SOUNDS_DIR = os.path.join(DATA_DIR, "sounds")
PLAYER_IMAGES_DIR = os.path.join(DATA_DIR, "player_images")
AUTODARTS_MODULES = "autodarts_modules"

def check_dependencies():
    """Prüft, ob PyInstaller installiert ist und installiert es bei Bedarf"""
    try:
        subprocess.run([sys.executable, "-m", "pip", "show", "pyinstaller"], 
                      check=True, capture_output=True)
        print("PyInstaller ist bereits installiert.")
    except subprocess.CalledProcessError:
        print("PyInstaller wird installiert...")
        subprocess.run([sys.executable, "-m", "pip", "install", "pyinstaller"])

def create_starter_script():
    """Erstellt ein Starter-Skript, das den Streamlit-Server startet und den Browser öffnet"""
    starter_script = """
import os
import sys
import webbrowser
import time
import subprocess
import threading

def open_browser():
    # Warte 3 Sekunden, um sicherzustellen, dass der Server gestartet ist
    time.sleep(3)
    # Browser öffnen
    webbrowser.open('http://localhost:5000')

def start_streamlit():
    try:
        # Starte den Streamlit-Server
        subprocess.run([sys.executable, "-m", "streamlit", "run", "app.py", 
                      "--server.port", "5000", "--server.headless", "true",
                      "--server.address", "localhost"])
    except Exception as e:
        print(f"Fehler beim Starten des Streamlit-Servers: {e}")
        input("Drücken Sie eine Taste zum Beenden...")

if __name__ == "__main__":
    # Stelle sicher, dass wir im richtigen Verzeichnis sind
    if getattr(sys, 'frozen', False):
        # Wenn als EXE verpackt, setze das Arbeitsverzeichnis auf das Programmverzeichnis
        os.chdir(os.path.dirname(sys.executable))
    
    # Starte Browser im Hintergrund
    threading.Thread(target=open_browser, daemon=True).start()
    
    # Starte Streamlit
    start_streamlit()
"""

    # Schreibe das Skript in eine Datei
    with open("autodarts_launcher.py", "w") as f:
        f.write(starter_script)
    
    print("Starter-Skript erstellt: autodarts_launcher.py")
    return "autodarts_launcher.py"

def create_exe():
    """Erstellt die ausführbare EXE-Datei mit PyInstaller"""
    # Erstelle das Starter-Skript
    starter_script = create_starter_script()
    
    # Konfiguriere PyInstaller-Argumente
    pyinstaller_args = [
        "pyinstaller",
        "--name=AutodartsStrafenMonitor",
        "--onefile",                  # Alles in einer einzigen EXE-Datei
        "--windowed",                 # Keine Konsole anzeigen
        "--icon=generated-icon.png",  # Icon für die EXE-Datei
        "--add-data=app.py;.",        # Hauptskript
        "--add-data=autodarts_modules;autodarts_modules",  # Module
        f"--add-data=resources;resources",  # Ressourcen
        "--hidden-import=streamlit",  # Versteckte Imports
        "--hidden-import=pandas",
        "--hidden-import=plotly",
        "--hidden-import=matplotlib",
        "--hidden-import=pygame",
        "--hidden-import=PIL",
        starter_script                # Starter-Skript als Haupteintragspunkt
    ]
    
    # Führe PyInstaller aus
    process = subprocess.Popen(pyinstaller_args)
    process.wait()
    
    # Prüfe, ob die EXE-Datei erstellt wurde
    exe_path = os.path.join("dist", "AutodartsStrafenMonitor.exe")
    if os.path.exists(exe_path):
        print(f"\nEXE-Datei erfolgreich erstellt: {exe_path}")
    else:
        print("\nFehler beim Erstellen der EXE-Datei!")

def copy_example_data():
    """Kopiert Beispieldaten und erstellt die Verzeichnisstruktur für die App"""
    # Stelle sicher, dass die benötigten Verzeichnisse existieren
    os.makedirs(DATA_DIR, exist_ok=True)
    os.makedirs(SOUNDS_DIR, exist_ok=True)
    os.makedirs(PLAYER_IMAGES_DIR, exist_ok=True)
    
    # Kopiere die Ressourcen, falls vorhanden
    if os.path.exists(RESOURCES_DIR):
        print("Kopiere Ressourcen...")
        os.makedirs(os.path.join("dist", RESOURCES_DIR), exist_ok=True)
        
        # Kopiere den Inhalt rekursiv
        for item in os.listdir(RESOURCES_DIR):
            source = os.path.join(RESOURCES_DIR, item)
            destination = os.path.join("dist", RESOURCES_DIR, item)
            
            if os.path.isdir(source):
                shutil.copytree(source, destination, dirs_exist_ok=True)
            else:
                shutil.copy2(source, destination)
    
    # Erstelle die Datenverzeichnisse in der dist-Struktur
    os.makedirs(os.path.join("dist", DATA_DIR), exist_ok=True)
    os.makedirs(os.path.join("dist", SOUNDS_DIR), exist_ok=True)
    os.makedirs(os.path.join("dist", PLAYER_IMAGES_DIR), exist_ok=True)

def create_startup_batch():
    """Erstellt eine BAT-Datei, die die EXE startet"""
    batch_content = """@echo off
echo Starte Autodarts Strafen-Monitor...
start "" "AutodartsStrafenMonitor.exe"
"""
    
    batch_path = os.path.join("dist", "Autodarts-Starten.bat")
    with open(batch_path, "w") as f:
        f.write(batch_content)
    
    print(f"Batch-Datei erstellt: {batch_path}")

def main():
    """Hauptfunktion zum Erstellen der EXE-Datei"""
    print("=== Autodarts Strafen-Monitor EXE-Builder ===")
    
    # Prüfe, ob PyInstaller installiert ist
    check_dependencies()
    
    # Erstelle die EXE-Datei
    create_exe()
    
    # Kopiere Beispieldaten
    copy_example_data()
    
    # Erstelle eine Batch-Datei zum Starten
    create_startup_batch()
    
    print("\nEXE-Erstellung abgeschlossen!")
    print("Die Anwendung befindet sich im 'dist'-Ordner")
    print("Zum Starten der Anwendung, öffnen Sie die Datei 'AutodartsStrafenMonitor.exe' oder 'Autodarts-Starten.bat'")
    
    input("\nDrücken Sie eine Taste zum Beenden...")

if __name__ == "__main__":
    main()