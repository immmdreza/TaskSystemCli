import json
from typing import List

__filename = "data/sudos.json"

def get_sudos() -> List[int]:
    with open(__filename, 'r', encoding="utf8") as f:
        datastore = json.load(f)
    return datastore

def add_sudo(id):
    old = get_sudos()
    if not any([x for x in old if x == id]):
        old.append(id)

    with open(__filename, 'w', encoding="utf8") as f:
        return f.write(json.dumps(old))