import json
from typing import List

class roles_text:
    def __init__(
        self, 
        text, 
        roles, 
    ):

        self.text = text
        self.roles = roles

    @staticmethod
    def get_roles_texts():
        with open("data/roles_texts.json", 'r', encoding="utf8") as f:
            datastore = json.load(f)
        
        result = []
        for r in datastore:
            result.append(
                roles_text(
                    r["Text"],
                    r["Roles"],
                )
            )
            
        return result