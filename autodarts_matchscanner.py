from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
import time

class AutodartsMonitor:
    def __init__(self):
        self.driver = None
        self.match_url = None

    def starte_browser(self):
        print("🔁 Starte Chrome...")

        options = webdriver.ChromeOptions()
        options.add_argument("--start-maximized")
        options.add_argument(r"--user-data-dir=C:\ChromeProfile\AutodartsMonitor")  # Profilordner

        service = Service("chromedriver.exe")
        self.driver = webdriver.Chrome(service=service, options=options)

    def finde_aktuelles_match(self):
        print("🌐 Lade Autodarts-Startseite...")
        self.driver.get("https://play.autodarts.io/")
        time.sleep(5)  # Sicherheitspause

        try:
            match_link = WebDriverWait(self.driver, 10).until(
                EC.presence_of_element_located((By.XPATH, "//a[contains(@href, '/matches/')]"))
            )
            relative_url = match_link.get_attribute("href")
            self.match_url = relative_url
            print(f"✅ Aktuelles Match gefunden: {self.match_url}")

            self.driver.get(self.match_url)
            time.sleep(3)

        except Exception as e:
            print("❌ Kein laufendes Match gefunden:", e)

    def erkenne_spieler_und_punkte(self):
        print("🔍 Scanne Spieler und Punkte...")

        try:
            spieler = self.driver.find_elements(By.XPATH, "//p[contains(@class, 'chakra-text')]")
            print(f"👥 Gefundene Spieler: {len(spieler)}")
            for s in spieler:
                print("→", s.text)

            punkte = self.driver.find_elements(By.CLASS_NAME, "ad-ext-turn-points")
            print(f"🎯 Gefundene Punktzahlen: {len(punkte)}")
            for p in punkte:
                print("➡️", p.text)

        except Exception as e:
            print("❌ Fehler bei der Analyse:", e)

if __name__ == "__main__":
    monitor = AutodartsMonitor()
    monitor.starte_browser()
    monitor.finde_aktuelles_match()
    monitor.erkenne_spieler_und_punkte()
