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
        self.gebuehren = {"THE UNDERDOG": 0.0, "THE WILDFLY": 0.0}
        self.gesehene_wuerfe = set()

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
        time.sleep(5)

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

    def verarbeite_gebuehren(self):
        try:
            wuerfe = self.driver.find_elements(By.CLASS_NAME, "ad-ext-turn-points")

            for wurf in wuerfe:
                text = wurf.text.strip()
                parent = wurf.find_element(By.XPATH, "./../../..")
                if parent in self.gesehene_wuerfe:
                    continue
                self.gesehene_wuerfe.add(parent)

                punkte = int(text) if text.isdigit() else None
                if punkte == 26 or punkte == 180:
                    spieler_element = parent.find_element(By.XPATH, ".//p[contains(@class, 'chakra-text')]")
                    spielername = spieler_element.text.strip()

                    if spielername not in self.gebuehren:
                        self.gebuehren[spielername] = 0.0

                    if punkte == 26:
                        self.gebuehren[spielername] += 0.10
                    elif punkte == 180:
                        self.gebuehren[spielername] += 2.00

                    print(f"💰 {spielername}: {punkte} → {self.gebuehren[spielername]:.2f} €")

        except Exception as e:
            print("⚠️ Fehler beim Verarbeiten:", e)

    def run(self):
        self.starte_browser()
        self.finde_aktuelles_match()

        while True:
            self.verarbeite_gebuehren()
            time.sleep(3)

# Start
if __name__ == "__main__":
    monitor = AutodartsMonitor()
    monitor.run()
