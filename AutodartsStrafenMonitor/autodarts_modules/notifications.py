
import streamlit as st

def notify_special_throw(player_name, score, amount):
    """Notify about special throws"""
    message = f"🎯 {player_name} hat eine {score} geworfen! (Strafe: {amount:.2f}€)"
    st.toast(message)

def create_notification_settings():
    """Create notification settings interface"""
    st.subheader("Benachrichtigungen")
    
    # Notification settings
    st.checkbox(
        "Desktop-Benachrichtigungen aktivieren",
        value=True,
        help="Zeigt Desktop-Benachrichtigungen bei besonderen Würfen an"
    )
    
    st.checkbox(
        "Sound-Benachrichtigungen aktivieren",
        value=True,
        help="Spielt Sounds bei besonderen Würfen ab"
    )
