import os
import sys

# Füge das aktuelle Verzeichnis (und ggf. das Elternverzeichnis) zum Python-Suchpfad hinzu
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from autodarts_modules.autodarts_web_integration import AutodartsAPI

import streamlit as st

# Beispiel: Initialisierung der API
api = AutodartsAPI()


def main():
    st.title("Autodarts Strafen-Monitor")

    # Beispiel: Verwendung der API
    active_match = api.get_active_match()
    st.write("Aktuelles Spiel:", active_match if active_match else "Kein aktives Spiel gefunden.")

    # Weitere Funktionen der App hinzufügen
    st.sidebar.title("Menü")
    menu = st.sidebar.radio("Wähle eine Option", ["Dashboard", "Spielerverwaltung", "Statistiken"])

    if menu == "Dashboard":
        st.header("📊 Dashboard")
        st.write("Hier könnte dein Echtzeit-Dashboard angezeigt werden.")
    elif menu == "Spielerverwaltung":
        st.header("👥 Spielerverwaltung")
        st.write("Hier kannst du Spieler hinzufügen oder verwalten.")
    elif menu == "Statistiken":
        st.header("📊 Statistiken")
        st.write("Hier könnten Statistiken und Analyse angezeigt werden.")


if __name__ == "__main__":
    main()
