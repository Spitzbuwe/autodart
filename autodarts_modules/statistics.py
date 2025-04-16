
import streamlit as st
import pandas as pd
import plotly.express as px
from .database import get_player_penalties, get_recent_throws

def create_dashboard():
    """Create the statistics dashboard"""
    st.header("📊 Statistiken")
    
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
