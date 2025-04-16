
class AutodartsAPI:
    def __init__(self):
        self.current_match = None
        self.chrome = None

    def start_chrome_detection(self):
        """Start monitoring Chrome for Autodarts matches"""
        pass  # Implement Chrome detection logic

    def stop_chrome_detection(self):
        """Stop Chrome monitoring"""
        pass

    def get_active_match(self):
        """Get current active match"""
        return self.current_match

    def fetch_active_game(self):
        """Fetch current game data"""
        return {}

    def check_for_special_throws(self, previous_data):
        """Check for 26 and 180 throws"""
        special_throws = []
        try:
            data_str = str(previous_data)
            if "ad-ext-turn-points" in data_str and "css-1lvci65" in data_str:
                points = data_str.split('css-1lvci65">')[1].split('<')[0]
                # Extrahiere den Spielernamen aus dem HTML
                player_parts = data_str.split('css-0">')
                if len(player_parts) > 1:
                    player = player_parts[1].split('<')[0]
                    if points in ['26', '180']:
                        special_throws.append({
                            "player": player,
                            "score": int(points)
                        })
        except Exception as e:
            print(f"Fehler bei der Wurferkennung: {str(e)}")
        return special_throws
