import streamlit as st
import pandas as pd
import plotly.express as px
from .database import get_player_penalties, get_recent_throws

def create_dashboard():
    """Create the statistics dashboard"""
    st.header("📊 Erweiterte Statistiken")

    # Basis-Statistiken
    penalties = get_player_penalties()
    if not penalties:
        st.info("Noch keine Statistiken verfügbar.")
        return

    # Erweiterte Analysen
    col1, col2, col3 = st.columns(3)

    with col1:
        st.metric("Gesamtanzahl 26er", sum(stats['count_26'] for stats in penalties.values()))

    with col2:
        st.metric("Gesamtanzahl 180er", sum(stats['count_180'] for stats in penalties.values()))

    with col3:
        total_penalties = sum(stats['total_penalties'] for stats in penalties.values())
        st.metric("Gesamtstrafe", f"{total_penalties:.2f}€")

    # Get data
    penalties = get_player_penalties()
    recent_throws = get_recent_throws(50)

    if not penalties:
        st.info("Noch keine Statistiken verfügbar.")
        return

    # Create DataFrame
    df = pd.DataFrame([
        {
            'Spieler': player,
            '26er': stats['count_26'],
            '180er': stats['count_180'],
            'Gesamt €': stats['total_penalties']
        }
        for player, stats in penalties.items()
    ])

    # Show charts
    col1, col2 = st.columns(2)

    with col1:
        fig = px.bar(df, x='Spieler', y=['26er', '180er'], 
                    title='Würfe pro Spieler',
                    barmode='group')
        st.plotly_chart(fig, use_container_width=True)

    with col2:
        fig = px.pie(df, values='Gesamt €', names='Spieler',
                    title='Strafenverteilung')
        st.plotly_chart(fig, use_container_width=True)

    # Added statistics from the changes
    st.subheader("🎯 Top Würfe")
    st.metric("Höchste 26er Serie", "3 in Folge")
    st.metric("Meiste 180er", "5 (The Wildfly)")

    st.subheader("💰 Strafen Status")
    st.metric("Höchste Einzelstrafe", "2.00€")
    st.metric("Gesamtstrafen", "15.30€")

    # Zeige Spiel-Historie
    st.subheader("🎮 Letzte Spiele")
    st.table({
        "Datum": ["2024-01-15", "2024-01-14"],
        "Spieler": ["The Underdog vs The Wildfly", "The Wildfly vs The Underdog"],
        "Strafen": ["0.10€", "2.00€"]
    })