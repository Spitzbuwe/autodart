import streamlit as st
import pandas as pd
import time
import json
import os
import threading
import sqlite3
from datetime import datetime

# Import the Autodarts integration modules
from autodarts_modules.autodarts_web_integration import AutodartsAPI
from autodarts_modules.database import (
    init_db, get_or_create_player, add_penalty_to_db, 
    get_player_penalties, get_recent_throws, 
    reset_player_penalties, reset_all_penalties,
    export_data, import_data
)

# Import new modules for enhanced features
from autodarts_modules.statistics import create_dashboard
from autodarts_modules.player_management import create_player_management_interface
from autodarts_modules.audio import initialize_sounds, play_throw_sound, create_sound_settings
from autodarts_modules.export_import import create_export_import_interface
from autodarts_modules.notifications import notify_special_throw, create_notification_settings

# Initialize the database
init_db()

# Initialize the sound system
initialize_sounds()

# Set up the page configuration
st.set_page_config(
    page_title="Autodarts Strafen-Monitor",
    page_icon="🎯",
    layout="wide",
    initial_sidebar_state="expanded",
    menu_items={
        'About': "# Autodarts Strafen-Monitor\nVerfolgt 26er und 180er Würfe und berechnet Strafen automatisch."
    }
)

# Initialize session state variables
if 'autodarts_api' not in st.session_state:
    st.session_state.autodarts_api = AutodartsAPI()
    
if 'monitoring_active' not in st.session_state:
    st.session_state.monitoring_active = False
    
if 'current_match' not in st.session_state:
    st.session_state.current_match = None
    
if 'last_check_time' not in st.session_state:
    st.session_state.last_check_time = None
    
if 'dark_mode' not in st.session_state:
    st.session_state.dark_mode = True  # Default to dark mode
    
if 'penalties' not in st.session_state:
    st.session_state.penalties = {}
    
if 'default_players' not in st.session_state:
    st.session_state.default_players = ["The Underdog", "The Wildfly"]
    # Make sure default players are in the database
    for player in st.session_state.default_players:
        get_or_create_player(player)

# Function to check for active matches
def check_for_active_match():
    """Check for active matches in the background"""
    try:
        st.session_state.current_match = st.session_state.autodarts_api.get_active_match()
        st.session_state.last_check_time = datetime.now()
    except Exception as e:
        st.error(f"Fehler bei der Matchüberprüfung: {str(e)}")

# Function to start monitoring
def start_monitoring():
    """Start the monitoring process for Autodarts matches"""
    st.session_state.monitoring_active = True
    st.session_state.autodarts_api.start_chrome_detection()

# Function to stop monitoring
def stop_monitoring():
    """Stop the monitoring process"""
    st.session_state.monitoring_active = False
    st.session_state.autodarts_api.stop_chrome_detection()

# Function to update player stats
def update_player_stats():
    """Update the player statistics from the database"""
    st.session_state.penalties = get_player_penalties()

# Function to add a penalty manually
def add_penalty_manually(player_name, score):
    """Add a penalty manually for a player"""
    if score == 26:
        amount = 0.1  # 10 cents for 26
    elif score == 180:
        amount = 2.0  # 2 euros for 180
    else:
        st.error(f"Ungültiger Score: {score}. Nur 26 oder 180 sind erlaubt.")
        return False
    
    match_id = None
    if st.session_state.current_match:
        match_id = st.session_state.current_match.get("match_id")
    
    success = add_penalty_to_db(player_name, score, amount, match_id)
    if success:
        # Play sound and send notification
        play_throw_sound(score)
        notify_special_throw(player_name, score, amount)
        update_player_stats()
        return True
    return False

# Function to reset penalties for a player
def reset_penalties_for_player(player_name):
    """Reset all penalties for a specific player"""
    success = reset_player_penalties(player_name)
    if success:
        update_player_stats()
        return True
    return False

# Function to reset all penalties
def reset_all_player_penalties():
    """Reset penalties for all players"""
    success = reset_all_penalties()
    if success:
        update_player_stats()
        return True
    return False

# Helper function to format currency
def format_currency(amount):
    """Format an amount as EUR currency"""
    return f"{amount:.2f} €"

# Style application based on dark/light mode
def apply_theme():
    """Apply dark or light theme based on user preference"""
    if st.session_state.dark_mode:
        # Dark mode styles
        st.markdown("""
        <style>
        .stApp {
            background-color: #1E1E1E;
            color: #FFFFFF;
        }
        </style>
        """, unsafe_allow_html=True)
    else:
        # Light mode (default Streamlit)
        pass

# Apply theme based on current mode setting
apply_theme()

# Update player stats at the beginning
update_player_stats()

# Main application UI
st.title("🎯 Autodarts Strafen-Monitor")

# Tabs für die verschiedenen Bereiche der App
tabs = st.tabs(["Dashboard", "Statistiken", "Spielerverwaltung", "Einstellungen", "Experten-Werkzeuge"])

# Tab 1: Dashboard - Hauptbereich mit Übersicht und Kontrollelementen
with tabs[0]:
    # Dashboard content

    # Main content - Split into columns
    col1, col2 = st.columns([2, 1])

    # Column 1: Statistics Table
    with col1:
        st.subheader("Strafen-Übersicht")
        
        if st.session_state.penalties:
            # Create a DataFrame for better display
            data = []
            for player, stats in st.session_state.penalties.items():
                data.append({
                    "Spieler": player,
                    "26er": stats.get("count_26", 0),
                    "180er": stats.get("count_180", 0),
                    "26er Betrag": format_currency(stats.get("count_26", 0) * 0.1),
                    "180er Betrag": format_currency(stats.get("count_180", 0) * 2.0),
                    "Gesamt": format_currency(stats.get("total_penalties", 0))
                })
            
            # Sort by total penalties descending
            df = pd.DataFrame(data)
            df = df.sort_values(by="Gesamt", ascending=False)
            
            # Display the table
            st.dataframe(df, use_container_width=True, hide_index=True)
            
            # Summary statistics
            st.subheader("Zusammenfassung")
            total_26 = sum(stats.get("count_26", 0) for stats in st.session_state.penalties.values())
            total_180 = sum(stats.get("count_180", 0) for stats in st.session_state.penalties.values())
            total_penalties = sum(stats.get("total_penalties", 0) for stats in st.session_state.penalties.values())
            
            # Display summary in columns
            sum1, sum2, sum3 = st.columns(3)
            with sum1:
                st.metric("Gesamte 26er", total_26)
            with sum2:
                st.metric("Gesamte 180er", total_180)
            with sum3:
                st.metric("Gesamte Strafen", format_currency(total_penalties))
        else:
            st.info("Keine Strafen-Daten vorhanden. Starte die Überwachung oder füge Strafen manuell hinzu.")

    # Column 2: Recent activity and manual controls
    with col2:
        # Monitoring controls
        st.subheader("Überwachung")
        if not st.session_state.monitoring_active:
            if st.button("Überwachung starten", use_container_width=True):
                start_monitoring()
                st.success("Überwachung gestartet!")
        else:
            if st.button("Überwachung stoppen", use_container_width=True):
                stop_monitoring()
                st.warning("Überwachung gestoppt!")
        
        # Status information
        if st.session_state.monitoring_active:
            st.success("Aktiv - Überwache Spiele")
        else:
            st.warning("Inaktiv - Keine Überwachung")
        
        if st.session_state.current_match:
            st.info(f"Aktuelles Match: {st.session_state.current_match.get('match_id', 'Unbekannt')}")
            if st.button("Match öffnen", use_container_width=True):
                match_url = st.session_state.current_match.get('match_url')
                if match_url:
                    # Use Chrome integration to open the match
                    st.session_state.autodarts_api.chrome.open_match_url(
                        match_id=st.session_state.current_match.get('match_id')
                    )
        
        # Recent activity
        st.subheader("Letzte Aktivitäten")
        recent_throws = get_recent_throws(limit=10)
        
        if recent_throws:
            for throw in recent_throws:
                icon = "🎯" if throw["score"] == 26 else "🔥" if throw["score"] == 180 else "❓"
                score = throw["score"]
                player = throw["player"]
                timestamp = datetime.fromisoformat(throw["timestamp"]) if isinstance(throw["timestamp"], str) else throw["timestamp"]
                time_str = timestamp.strftime("%d.%m.%Y %H:%M")
                
                st.markdown(f"**{icon} {score}** - {player} - {time_str}")
        else:
            st.info("Keine Aktivitäten aufgezeichnet.")
        
        # Manual controls
        st.subheader("Manuelle Steuerung")
        
        # Manual penalty addition
        with st.form("add_penalty_form"):
            st.write("Strafe manuell hinzufügen")
            
            # Player selection dropdown with default players
            all_players = list(st.session_state.penalties.keys())
            # Make sure default players are included
            for player in st.session_state.default_players:
                if player not in all_players:
                    all_players.append(player)
            
            player_name = st.selectbox(
                "Spieler", 
                options=all_players,
                index=0 if all_players else None
            )
            
            # Score selection (26 or 180)
            score = st.radio("Wurf", [26, 180], horizontal=True)
            
            submitted = st.form_submit_button("Strafe hinzufügen", use_container_width=True)
            if submitted and player_name:
                if add_penalty_manually(player_name, score):
                    st.success(f"Strafe für {player_name} ({score}) erfolgreich hinzugefügt!")
                else:
                    st.error("Fehler beim Hinzufügen der Strafe")
        
        # Reset controls
        st.subheader("Zurücksetzen")
        
        # Reset for specific player
        with st.expander("Strafen für einen Spieler zurücksetzen"):
            player_to_reset = st.selectbox(
                "Spieler auswählen", 
                options=list(st.session_state.penalties.keys()),
                key="reset_player_select"
            )
            
            if st.button("Zurücksetzen", key="reset_player_button", use_container_width=True) and player_to_reset:
                if reset_penalties_for_player(player_to_reset):
                    st.success(f"Strafen für {player_to_reset} zurückgesetzt!")
                else:
                    st.error(f"Fehler beim Zurücksetzen der Strafen für {player_to_reset}")
        
        # Reset all penalties
        with st.expander("Alle Strafen zurücksetzen"):
            st.warning("Achtung: Dies setzt alle Strafen für alle Spieler zurück!")
            
            if st.button("Alle Strafen zurücksetzen", key="reset_all_button", use_container_width=True):
                if reset_all_player_penalties():
                    st.success("Alle Strafen wurden zurückgesetzt!")
                else:
                    st.error("Fehler beim Zurücksetzen aller Strafen")

# Tab 2: Statistiken - Grafiken und Analysen
with tabs[1]:
    create_dashboard()  # Statistik-Dashboard aus statistics.py

# Tab 3: Spielerverwaltung - Spieler hinzufügen, bearbeiten, etc.
with tabs[2]:
    create_player_management_interface()  # Spielerverwaltung aus player_management.py

# Tab 4: Einstellungen
with tabs[3]:
    st.header("Einstellungen")

    # Dark/Light mode toggle
    st.subheader("Darstellung")
    dark_mode = st.toggle("Dunkelmodus", value=st.session_state.dark_mode)
    if dark_mode != st.session_state.dark_mode:
        st.session_state.dark_mode = dark_mode
        st.rerun()
    
    # Sound settings
    create_sound_settings()  # Sound-Einstellungen aus audio.py
    
    # Notification settings
    create_notification_settings()  # Benachrichtigungseinstellungen aus notifications.py

# Tab 5: Experten-Werkzeuge
with tabs[4]:
    st.header("Experten-Werkzeuge")
    
    # Data export and import
    create_export_import_interface()  # Export/Import-Funktionen aus export_import.py
    
    # Sidebar as fallback (hidden in regular usage)
with st.sidebar:
    st.header("Schnellzugriff")
    
    # Quick controls for monitoring
    if not st.session_state.monitoring_active:
        if st.button("Überwachung starten", key="sidebar_start", use_container_width=True):
            start_monitoring()
            st.success("Überwachung gestartet!")
    else:
        if st.button("Überwachung stoppen", key="sidebar_stop", use_container_width=True):
            stop_monitoring()
            st.warning("Überwachung gestoppt!")
    
    # Status display
    st.caption("Status: " + ("Aktiv" if st.session_state.monitoring_active else "Inaktiv"))



# Function to check for special throws
def check_for_special_throws():
    """Check for special throws (26 or 180) and add penalties automatically"""
    try:
        api = st.session_state.autodarts_api
        previous_data = getattr(st.session_state, 'previous_game_data', None)
        current_data = api.fetch_active_game()
        
        if current_data and current_data.get("players"):
            # Store current data for next comparison
            st.session_state.previous_game_data = current_data
            
            # If we have previous data, check for special throws
            if previous_data:
                special_throws = api.check_for_special_throws(previous_data)
                
                if special_throws:
                    for throw in special_throws:
                        player_name = throw["player"]
                        score = throw["score"]
                        st.toast(f"Automatisch erkannt: {player_name} hat {score} geworfen")
                        # Add penalty
                        add_penalty_manually(player_name, score)
                    
                    # Update UI to reflect changes
                    update_player_stats()
                    st.rerun()
    except Exception as e:
        st.error(f"Fehler bei der Überprüfung spezieller Würfe: {str(e)}")

# Background task to check for active matches
if st.session_state.monitoring_active:
    # Check match every 10 seconds if monitoring is active
    if not st.session_state.last_check_time or (datetime.now() - st.session_state.last_check_time).total_seconds() > 10:
        check_for_active_match()
        # Also check for special throws and add penalties automatically
        check_for_special_throws()
        st.rerun()
