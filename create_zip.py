import os
import zipfile

# Liste der zu sichernden Dateien und Verzeichnisse
files_to_zip = [
    "app.py",
    "autodarts_launcher.py",
    "requirements.txt",
    "autodarts_modules/",
    "data/"
]

zip_file_name = 'autodarts_monitor.zip'

with zipfile.ZipFile(zip_file_name, 'w') as zipf:
    for item in files_to_zip:
        if os.path.isdir(item):
            # Durchlaufe alle Dateien im Verzeichnis
            for foldername, subfolders, filenames in os.walk(item):
                for filename in filenames:
                    file_path = os.path.join(foldername, filename)
                    zipf.write(file_path, os.path.relpath(file_path, os.path.dirname(item)))
        else:
            zipf.write(item)

print(f"ZIP-Datei '{zip_file_name}' wurde erstellt.")