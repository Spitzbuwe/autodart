import subprocess
import os
import time

def start_streamlit():
    print("[INFO] Starte Streamlit-Dashboard...")

    # ✅ Pfad zur Hauptdatei anpassen, falls nötig
    hauptdatei = os.path.join("AutodartsStrafenMonitor", "app.py")

    # Prüfen, ob Datei existiert
    if not os.path.isfile(hauptdatei):
        print(f"[FEHLER] Datei nicht gefunden: {hauptdatei}")
        return

    # ✅ Streamlit im Headless-Modus starten (kein E-Mail-Fenster)
    cmd = [
        "streamlit",
        "run",
        hauptdatei,
        "--server.headless", "true",
        "--browser.gatherUsageStats", "false"
    ]
    subprocess.Popen(cmd, shell=True)
    time.sleep(2)

def öffne_dashboard_in_chrome():
    print("[INFO] Öffne Dashboard in Chrome...")
    try:
        # Öffnet Google Chrome direkt, nicht Standardbrowser
        subprocess.run(["start", "chrome", "http://localhost:8501"], shell=True)
    except Exception as e:
        print(f"[FEHLER] Konnte Chrome nicht öffnen: {e}")
        print("Bitte öffne manuell: http://localhost:8501")

if __name__ == "__main__":
    start_streamlit()
    öffne_dashboard_in_chrome()
