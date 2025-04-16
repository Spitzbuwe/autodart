
# Wechsle ins Downloads-Verzeichnis
Set-Location -Path "C:\Users\mielersch\Downloads"

# Erstelle Hauptverzeichnisse
New-Item -ItemType Directory -Path "programm" -Force
New-Item -ItemType Directory -Path "programm\autodarts_modules" -Force
New-Item -ItemType Directory -Path "programm\data" -Force
New-Item -ItemType Directory -Path "programm\data\player_images" -Force
New-Item -ItemType Directory -Path "programm\data\sounds" -Force
New-Item -ItemType Directory -Path "programm\resources" -Force

# Erstelle Python-Module
New-Item -ItemType File -Path "autodarts_modules\__init__.py" -Force
New-Item -ItemType File -Path "autodarts_modules\audio.py" -Force
New-Item -ItemType File -Path "autodarts_modules\autodarts_web_integration.py" -Force
New-Item -ItemType File -Path "autodarts_modules\database.py" -Force
New-Item -ItemType File -Path "autodarts_modules\export_import.py" -Force
New-Item -ItemType File -Path "autodarts_modules\notifications.py" -Force
New-Item -ItemType File -Path "autodarts_modules\player_management.py" -Force
New-Item -ItemType File -Path "autodarts_modules\statistics.py" -Force

# Erstelle Hauptdateien
New-Item -ItemType File -Path "app.py" -Force
New-Item -ItemType File -Path "autodarts_launcher.py" -Force
New-Item -ItemType File -Path "requirements.txt" -Force

Write-Host "Verzeichnisstruktur wurde erfolgreich erstellt!"
