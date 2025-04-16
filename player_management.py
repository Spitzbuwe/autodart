
import streamlit as st
from .database import get_or_create_player, get_player_penalties

def create_player_management_interface():
    """Create the player management interface"""
    st.header("👥 Spielerverwaltung")
    
    # Get current players
    current_players = list(get_player_penalties().keys())
    
    # Add new player
    with st.form("add_player_form"):
        st.subheader("Neuen Spieler hinzufügen")
        new_player = st.text_input("Name")
        submitted = st.form_submit_button("Hinzufügen")
        
        if submitted and new_player:
            if new_player not in current_players:
                get_or_create_player(new_player)
                st.success(f"Spieler {new_player} hinzugefügt!")
            else:
                st.error("Dieser Spieler existiert bereits!")
    
    # List current players
    st.subheader("Aktuelle Spieler")
    for player in current_players:
        st.write(f"• {player}")
