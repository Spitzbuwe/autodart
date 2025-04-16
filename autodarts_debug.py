from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
import time

class AutodartsAPI:
    def __init__(self):
        self.driver = None

    def start_browser(self):
        print("🔁 Starte Chrome für Autodarts...")
        try:
            options = webdriver.ChromeOptions()
            options.add_argument("--start-maximized")
            options.add_argument(r"--user-data-dir=C:\ChromeProfile\AutodartsMonitor")

            service = Service("chromedriver.exe")
            self.driver = webdriver.Chrome(service=service, options=options)

            # Hier: Deine aktuelle Match-URL
            url = "https://play.autodarts.io/matches/01963eb1-e269-79b3-a17d-3bcf594a1efb"
            self.driver.get(url)

            print("✅ Chrome gestartet und Match geladen.")
            time.sleep(3)

        except Exception as e:
            print("❌ Fehler beim Starten von Chrome:", e)

    def detect_players_and_points(self):
        print("▶ Starte Match-Erkennung...")

        try:
            # Spieler suchen
            spieler_elements = self.driver.find_elements(By.XPATH, "//p[@class='chakra-text css-0']")
            print(f"📋 Spieler gefunden: {len(spieler_elements)}")
            for e in spieler_elements:
                print("👤 Spielername:", e.text)

            # Punktzahlen suchen
            punkt_elements = self.driver.find_elements(By.CLASS_NAME, "ad-ext-turn-points")
            print(f"🎯 Punktzahlen gefunden: {len(punkt_elements)}")
            for p in punkt_elements:
                print("➡️ Punktwert:", p.text)

        except Exception as e:
            print("❌ Fehler bei der Analyse:", e)

if __name__ == "__main__":
    api = AutodartsAPI()
    api.start_browser()
    api.detect_players_and_points()
