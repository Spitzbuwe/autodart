
import streamlit as st
import json
import pandas as pd
from datetime import datetime
from .database import export_data, import_data

def create_export_import_interface():
    st.header("Daten Export/Import")
    
    col1, col2 = st.columns(2)
    
    with col1:
        st.subheader("Export")
        export_format = st.selectbox(
            "Export Format",
            ["JSON", "CSV", "Excel"],
            key="export_format"
        )
        
        if st.button("Daten exportieren"):
            data = export_data()
            
            if data:
                df = pd.DataFrame(data)
                
                if export_format == "JSON":
                    export_data = df.to_json(orient="records", date_format="iso")
                    st.download_button(
                        "JSON herunterladen",
                        export_data,
                        f"strafen_export_{datetime.now().strftime('%Y%m%d_%H%M%S')}.json",
                        "application/json"
                    )
                elif export_format == "CSV":
                    export_data = df.to_csv(index=False)
                    st.download_button(
                        "CSV herunterladen",
                        export_data,
                        f"strafen_export_{datetime.now().strftime('%Y%m%d_%H%M%S')}.csv",
                        "text/csv"
                    )
                else:  # Excel
                    buffer = pd.ExcelWriter(f"strafen_export_{datetime.now().strftime('%Y%m%d_%H%M%S')}.xlsx")
                    df.to_excel(buffer, index=False)
                    st.download_button(
                        "Excel herunterladen",
                        buffer,
                        f"strafen_export_{datetime.now().strftime('%Y%m%d_%H%M%S')}.xlsx",
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    )
    
    with col2:
        st.subheader("Import")
        uploaded_file = st.file_uploader("Datei zum Import auswählen", type=["json", "csv", "xlsx"])
        
        if uploaded_file is not None:
            try:
                if uploaded_file.type == "application/json":
                    data = pd.read_json(uploaded_file)
                elif uploaded_file.type == "text/csv":
                    data = pd.read_csv(uploaded_file)
                else:  # Excel
                    data = pd.read_excel(uploaded_file)
                
                if st.button("Daten importieren"):
                    if import_data(data.values.tolist()):
                        st.success("Daten erfolgreich importiert!")
                    else:
                        st.error("Fehler beim Importieren der Daten")
            except Exception as e:
                st.error(f"Fehler beim Lesen der Datei: {str(e)}")
