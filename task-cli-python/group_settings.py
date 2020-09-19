import os
import json

class GroupSetting:
    __defualt_settiings = {

        "message_len_filter": {
            "on": False, 
            "max_len": 1000,
            "allowed": [
                ("frw", "@A_raS_H"),
                ("nrm", "@A_raS_H")
            ]
        }
    }

    __saved_settings = {}
    settiings = {}
    __datacenter = "data/group_settings"

    def __init__(self, group_id):
        self.group_id = group_id
        self.__datacenter += f"/gs_{self.group_id}.json"

    def __read_file(self) -> dict:
        if os.path.isfile(self.__datacenter):
            with open(self.__datacenter, "r", encoding='utf8') as f:
                return json.load(f)
        else:
            with open(self.__datacenter, "w", encoding='utf8') as f:
                if f.write(json.dumps(self.__defualt_settiings)):
                    return self.__defualt_settiings
    
    def __write_file(self, content):
        with open(self.__datacenter, "w", encoding='utf8') as f:
            f.write(content)

    def load_settings(self):
        content = self.__read_file()
        self.settiings = content
        self.__saved_settings = content
        
    @property
    def saved_settings(self):
        return self.__saved_settings

    def save_settings(self):
        return self.__write_file(
            json.dumps(self.settiings)
        )

    def get_setting(self, setti):
        result: list = []
        if isinstance(setti, str):
            self.load_settings()
            if setti in self.settiings:
                result.append(self.settiings[setti])
        elif isinstance(setti, list):
            for s in setti:
                if s in self.settiings:
                    result.append(self.settiings[s])
        return result
