import os
import sys
import streamlit as st
from autodarts_modules.database import init_db, get_player_penalties
from autodarts_modules.autodarts_web_integration import AutodartsAPI

# Initialize the database
init_db()

def main():
    api = AutodartsAPI()
    api.start_chrome_detection()
    
    st.set_page_config(page_title="Autodarts Strafen-Monitor", layout="wide", initial_sidebar_state="expanded")
    
    st.title("🎯 Autodarts Strafen-Monitor")

    menu = st.sidebar.radio("Menü", ["Dashboard", "Spielerverwaltung", "Statistiken & Export"])

    if menu == "Dashboard":
        st.header("📊 Dashboard")
        # Aktuelle Strafen anzeigen
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

if __name__ == "__main__":
    main()