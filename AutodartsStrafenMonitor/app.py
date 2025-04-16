import os
import sys

# Füge das aktuelle Verzeichnis (und ggf. das Elternverzeichnis) zum Python-Suchpfad hinzu
sys.path.append(os.path.abspath(os.path.dirname(__file__)))

from autodarts_modules.autodarts_web_integration import AutodartsAPI

import streamlit as st

# API initialisieren und Chrome-Erkennung starten
api = AutodartsAPI()
api.start_chrome_detection()
st.set_page_config(page_title="Autodarts Strafen-Monitor", layout="wide", initial_sidebar_state="expanded", menu_items=None)

# Dark Mode aktivieren
st.markdown("""
    <style>
    .stApp {
        background-color: #1E1E1E;
        color: #FFFFFF;
    }
    .stButton>button {
        background-color: #2E2E2E;
        color: #FFFFFF;
        border: 1px solid #3E3E3E;
    }
    .stTextInput>div>div>input {
        background-color: #2E2E2E;
        color: #FFFFFF;
    }
    .stTable {
        background-color: #2E2E2E;
    }
    .stDataFrame {
        background-color: #2E2E2E;
    }
    .stSelectbox>div>div {
        background-color: #2E2E2E;
        color: #FFFFFF;
    }
    .stButton>button {
        background-color: #2E2E2E;
        color: #FFFFFF;
        border: 1px solid #3E3E3E;
    }
    .stTextInput>div>div>input {
        background-color: #2E2E2E;
        color: #FFFFFF;
    }
    .stTable {
        background-color: #2E2E2E;
    }
    </style>
""", unsafe_allow_html=True)

st.title("🎯 Autodarts Strafen-Monitor")

def main():
    
    menu = st.sidebar.radio("Menü", ["Dashboard", "Spielerverwaltung", "Statistiken & Export"])

    if menu == "Dashboard":
        st.header("📊 Dashboard")

        # Aktuelle Strafen anzeigen
        from autodarts_modules.database import get_player_penalties
        penalties = get_player_penalties()

        # Übersicht der Strafen
        st.subheader("🎯 Aktuelle Strafen")

        for player, stats in penalties.items():
            col1, col2, col3, col4 = st.columns(4)
            with col1:
                st.write(f"**{player}**")
            with col2:
                st.write(f"26er: {stats['count_26']}")
            with col3:
                st.write(f"180er: {stats['count_180']}")
            with col4:
                st.write(f"Gesamt: {stats['total_penalties']:.2f}€")

        # Aktives Spiel anzeigen
        st.subheader("🎮 Aktuelles Spiel")
        active_match = api.get_active_match()
        if active_match:
            st.success(f"Aktives Spiel gefunden: {active_match}")
        else:
            st.warning("Kein aktives Spiel gefunden")
    elif menu == "Spielerverwaltung":
        st.header("👥 Spielerverwaltung")
        st.write("Hier kannst du Spieler hinzufügen oder verwalten.")
    elif menu == "Statistiken & Export":
        st.header("📊 Statistiken & Export")
        st.write("Hier könnten Statistiken und Analyse angezeigt werden.  Export-Funktionalität fehlt noch.")


if __name__ == "__main__":
    main()