import json
from typing import List

class role:
    def __init__(
        self, 
        emoji, 
        fullName, 
        kinders, 
        short_name, 
        team, 
        achivments
    ):

        self.emoji = emoji
        self.fullName = fullName
        self.kinders = kinders
        self.short_name = short_name
        self.team = team
        self.achivments = achivments

    @staticmethod
    def get_roles():
        with open("data/roles.json", 'r', encoding="utf8") as f:
            datastore = json.load(f)
        
        result = []
        for r in datastore:
            result.append(
                role(
                    r["Emoji"],
                    r["FullName"],
                    r["Kinders"],
                    r["ShortName"],
                    r.get("Team", None),
                    r.get("Achivments", None)
                )
            )
            
        return result

