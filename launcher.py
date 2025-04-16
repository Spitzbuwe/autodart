import os
import sys
import webbrowser
import time
import subprocess
import threading

def open_browser():
    time.sleep(3)
    webbrowser.open('http://localhost:5000')

def start_streamlit():
    try:
        subprocess.run([sys.executable, "-m", "streamlit", "run", "app.py", 
                      "--server.port", "5000", "--server.headless", "true",
                      "--server.address", "localhost"])
    except Exception as e:
        print(f"Fehler beim Starten des Streamlit-Servers: {e}")
        input("Drücken Sie eine Taste zum Beenden...")

if __name__ == "__main__":
    if getattr(sys, 'frozen', False):
        os.chdir(os.path.dirname(sys.executable))
    
    threading.Thread(target=open_browser, daemon=True).start()
    start_streamlit()