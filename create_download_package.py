
import os
import zipfile
import shutil

def create_download_package():
    # Erstelle temporären Ordner für das Paket
    if not os.path.exists("dist"):
        os.makedirs("dist")
    
    # Erstelle ZIP-Datei
    zip_path = "dist/AutodartsStrafenMonitor.zip"
    with zipfile.ZipFile(zip_path, "w", zipfile.ZIP_DEFLATED) as zf:
        # Füge Hauptdateien hinzu
        files_to_add = ["app.py", "autodarts_launcher.py", "requirements.txt"]
        for file in files_to_add:
            if os.path.exists(file):
                zf.write(file)
        
        # Füge autodarts_modules hinzu
        for root, dirs, files in os.walk("autodarts_modules"):
            for file in files:
                file_path = os.path.join(root, file)
                zf.write(file_path)
        
        # Füge data und resources Ordner hinzu
        for folder in ["data", "resources"]:
            if os.path.exists(folder):
                for root, dirs, files in os.walk(folder):
                    for file in files:
                        file_path = os.path.join(root, file)
                        zf.write(file_path)

if __name__ == "__main__":
    create_download_package()
    print("Download-Paket erstellt: dist/AutodartsStrafenMonitor.zip")
