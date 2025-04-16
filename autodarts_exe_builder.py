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
    webbrowser.open('http://0.0.0.0:5000')  # Stelle sicher, dass die URL korrekt ist

def start_streamlit():
    try:
        # Starte den Streamlit-Server
        subprocess.run([sys.executable, "-m", "streamlit", "run", "app.py", 
                      "--server.port", "5000", "--server.headless", "true",
                      "--server.address", "0.0.0.0"])  # Benutze 0.0.0.0 für die Adressbindung
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