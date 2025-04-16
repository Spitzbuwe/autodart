
import os
import shutil

def create_download_package():
    try:
        # Definiere Pfade
        dist_dir = "dist"
        zip_path = os.path.join(dist_dir, "AutodartsStrafenMonitor.zip")
        temp_dir = os.path.join(dist_dir, "temp")
        
        # Erstelle Verzeichnisse
        os.makedirs(dist_dir, exist_ok=True)
        os.makedirs(temp_dir, exist_ok=True)
        
        # Lösche alte ZIP falls vorhanden
        if os.path.exists(zip_path):
            os.remove(zip_path)
            
        # Kopiere Dateien in temp Verzeichnis
        files_to_copy = ["app.py", "autodarts_launcher.py", "requirements.txt"]
        for file in files_to_copy:
            if os.path.exists(file):
                shutil.copy2(file, temp_dir)
        
        # Kopiere Verzeichnisse
        for folder in ["autodarts_modules", "data", "resources"]:
            if os.path.exists(folder):
                dst_folder = os.path.join(temp_dir, folder)
                if os.path.exists(dst_folder):
                    shutil.rmtree(dst_folder)
                shutil.copytree(folder, dst_folder)
        
        # Erstelle ZIP aus temp Verzeichnis
        shutil.make_archive(os.path.splitext(zip_path)[0], 'zip', temp_dir)
        
        # Lösche temp Verzeichnis
        shutil.rmtree(temp_dir)
        
        print(f"ZIP-Datei erfolgreich erstellt: {zip_path}")
        return True
        
    except Exception as e:
        print(f"Fehler beim Erstellen der ZIP-Datei: {str(e)}")
        return False

if __name__ == "__main__":
    create_download_package()
