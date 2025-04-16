
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
