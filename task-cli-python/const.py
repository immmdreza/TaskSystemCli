werewolf_bots = [175844556, 198626752]
player_list_header = "بازیکن های زنده:"

final_list_footer = "مدت زمان بازی:";
voting_message = "خب دیگه شب شده و همه جمع میشن که رای بدن";
joined_message = "شما با موفقیت وارد بازی در گروه";
remaing_time = "فقط 30  ثانیه دیگه وقت دارید که وارد بازی شید...";
task_list_update = [ 
    "اوکی لیست بازیکنان آپدیت شد",
    "لیست نقشها تا به اینجا" 
]

new_achiv_header = [
    "Achievement Unlocked!", 
    "New Unlocks!"
]

datacenter_path = "data"

super_admin = 106296897;
tasksystem_id = 724104884;
tasksystem_username = "@TsWwPlus_Bot";

command_trigger = {
    
    "joingame":  ["jg", "joingame"],
    "eqmode": [ "em", "eqmode" ],
    "autogame": [ "ag", "autogame" ],
    "autojoin": [ "aj", "autojoin" ],
    "rolegame": [ "rg", "rolegame" ],
    "clickgame": [ "cg", "clickgame" ],
    "leavegame": [ "lg", "leavegame" ],
    "addsudo": [ "as", "addsudo" ],
    "ping": [ "p", "ping", "ping", "پینگ" ],
    "ts": [ "ta", "sn", "sr" ]
}

from data.roles import role
from data.roles_texts import roles_text
from data.sudos import get_sudos

nolynch_mode = {}
roles_texts = roles_text.get_roles_texts()
sudos = get_sudos()
roles = role.get_roles()

