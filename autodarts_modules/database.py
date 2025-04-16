import sqlite3
from datetime import datetime

def init_db():
    """Initialize the SQLite database"""
    conn = sqlite3.connect('data/penalties.db')
    c = conn.cursor()
    c.execute('''CREATE TABLE IF NOT EXISTS players
                 (id INTEGER PRIMARY KEY, name TEXT UNIQUE)''')
    c.execute('''CREATE TABLE IF NOT EXISTS penalties
                 (id INTEGER PRIMARY KEY, 
                  player_id INTEGER,
                  score INTEGER,
                  amount REAL,
                  match_id TEXT,
                  timestamp DATETIME,
                  FOREIGN KEY(player_id) REFERENCES players(id))''')
    conn.commit()
    conn.close()

def get_or_create_player(name):
    """Get or create a player by name"""
    conn = sqlite3.connect('data/penalties.db')
    c = conn.cursor()
    c.execute("INSERT OR IGNORE INTO players (name) VALUES (?)", (name,))
    c.execute("SELECT id FROM players WHERE name=?", (name,))
    player_id = c.fetchone()[0]
    conn.commit()
    conn.close()
    return player_id

def add_penalty_to_db(player_name, score, amount, match_id=None):
    """Add a penalty to the database"""
    try:
        player_id = get_or_create_player(player_name)
        conn = sqlite3.connect('data/penalties.db')
        c = conn.cursor()
        c.execute("""INSERT INTO penalties 
                    (player_id, score, amount, match_id, timestamp)
                    VALUES (?, ?, ?, ?, ?)""",
                 (player_id, score, amount, match_id, datetime.now()))
        conn.commit()
        conn.close()
        return True
    except Exception:
        return False

def get_player_penalties():
    """Get all penalties grouped by player"""
    conn = sqlite3.connect('data/penalties.db')
    c = conn.cursor()
    c.execute("""SELECT p.name,
                 COUNT(CASE WHEN pen.score = 26 THEN 1 END) as count_26,
                 COUNT(CASE WHEN pen.score = 180 THEN 1 END) as count_180,
                 SUM(pen.amount) as total_penalties
                 FROM players p
                 LEFT JOIN penalties pen ON p.id = pen.player_id
                 GROUP BY p.name""")
    results = c.fetchall()
    penalties = {}
    for row in results:
        penalties[row[0]] = {
            'count_26': row[1],
            'count_180': row[2],
            'total_penalties': row[3] or 0
        }
    conn.close()
    return penalties

def get_recent_throws(limit=10):
    """Get recent throws"""
    conn = sqlite3.connect('data/penalties.db')
    c = conn.cursor()
    c.execute("""SELECT p.name, pen.score, pen.timestamp
                 FROM penalties pen
                 JOIN players p ON pen.player_id = p.id
                 ORDER BY pen.timestamp DESC
                 LIMIT ?""", (limit,))
    results = c.fetchall()
    conn.close()
    return [{'player': r[0], 'score': r[1], 'timestamp': r[2]} for r in results]

def reset_player_penalties(player_name):
    """Reset penalties for a specific player"""
    try:
        conn = sqlite3.connect('data/penalties.db')
        c = conn.cursor()
        c.execute("""DELETE FROM penalties 
                    WHERE player_id=(SELECT id FROM players WHERE name=?)""",
                 (player_name,))
        conn.commit()
        conn.close()
        return True
    except Exception:
        return False

def reset_all_penalties():
    """Reset all penalties"""
    try:
        conn = sqlite3.connect('data/penalties.db')
        c = conn.cursor()
        c.execute("DELETE FROM penalties")
        conn.commit()
        conn.close()
        return True
    except Exception:
        return False

def export_data():
    """Export all data"""
    conn = sqlite3.connect('data/penalties.db')
    c = conn.cursor()
    c.execute("""SELECT p.name, pen.score, pen.amount, pen.match_id, pen.timestamp
                 FROM penalties pen
                 JOIN players p ON pen.player_id = p.id
                 ORDER BY pen.timestamp""")
    data = c.fetchall()
    conn.close()
    return data

def import_data(data):
    """Import data"""
    try:
        for row in data:
            player_name, score, amount, match_id, timestamp = row
            add_penalty_to_db(player_name, score, amount, match_id)
        return True
    except Exception:
        return False