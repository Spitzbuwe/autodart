
import os
import shutil
import zipfile

def create_download_package():
    # Erstelle temporären Ordner für das Paket
    if not os.path.exists("dist"):
        os.makedirs("dist")
    
    # Erstelle ZIP-Datei
    with zipfile.ZipFile("dist/AutodartsStrafenMonitor.zip", "w") as zf:
        # Füge Hauptdateien hinzu
        zf.write("app.py")
        zf.write("autodarts_launcher.py")
        
        # Füge autodarts_modules hinzu
        for root, dirs, files in os.walk("autodarts_modules"):
            for file in files:
                zf.write(os.path.join(root, file))
        
        # Füge data und resources Ordner hinzu
        for folder in ["data", "resources"]:
            if os.path.exists(folder):
                for root, dirs, files in os.walk(folder):
                    for file in files:
                        zf.write(os.path.join(root, file))

if __name__ == "__main__":
    create_download_package()
    print("Download-Paket erstellt: dist/AutodartsStrafenMonitor.zip")
