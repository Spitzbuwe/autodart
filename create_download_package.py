
import os
import zipfile
import shutil

def create_download_package():
    try:
        # Erstelle dist Ordner falls nicht vorhanden
        if not os.path.exists("dist"):
            os.makedirs("dist")
        
        # Definiere Zip-Pfad
        zip_path = "dist/AutodartsStrafenMonitor.zip"
        
        # Lösche alte ZIP falls vorhanden
        if os.path.exists(zip_path):
            os.remove(zip_path)
        
        # Erstelle neue ZIP-Datei
        with zipfile.ZipFile(zip_path, "w", zipfile.ZIP_DEFLATED) as zf:
            # Hauptdateien
            main_files = ["app.py", "autodarts_launcher.py", "requirements.txt"]
            for file in main_files:
                if os.path.exists(file):
                    zf.write(file, file)
            
            # Module
            for root, _, files in os.walk("autodarts_modules"):
                for file in files:
                    file_path = os.path.join(root, file)
                    zf.write(file_path, file_path)
            
            # Daten und Ressourcen
            for folder in ["data", "resources"]:
                if os.path.exists(folder):
                    for root, _, files in os.walk(folder):
                        for file in files:
                            file_path = os.path.join(root, file)
                            zf.write(file_path, file_path)
        
        print(f"ZIP-Datei erfolgreich erstellt: {zip_path}")
        return True
    except Exception as e:
        print(f"Fehler beim Erstellen der ZIP-Datei: {str(e)}")
        return False

if __name__ == "__main__":
    create_download_package()
