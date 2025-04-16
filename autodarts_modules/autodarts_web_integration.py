
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
            if "ad-ext-turn-points" in str(previous_data):
                points = str(previous_data).split('ad-ext-turn-points')[1].split('>')[1].split('<')[0]
                player = str(previous_data).split('css-0">')[1].split('<')[0]
                
                if points in ['26', '180']:
                    special_throws.append({
                        "player": player,
                        "score": int(points)
                    })
        except Exception:
            pass
        return special_throws
