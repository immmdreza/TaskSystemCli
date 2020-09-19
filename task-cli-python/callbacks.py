from pyrogram import Client
from pyrogram.types import Message
import time
from data.sudos import add_sudo, get_sudos
from const import (
    new_achiv_header, 
    roles_texts, 
    tasksystem_username,
    nolynch_mode, 
    voting_message,
    player_list_header,
    final_list_footer,
    command_trigger as ct,
    task_list_update,
    roles, super_admin
)
from persiantools.jdatetime import JalaliDateTime
import pytz

def show_time_call(cli: Client, msg: Message):
    print("show_time_call")
    today = JalaliDateTime.now(pytz.timezone("Asia/Tehran"));

    msg.reply_text(today.strftime("%c" + "\n\nTime zone: **Asia/Tehran**"))

def direct_rolling_call(cli: Client, msg: Message):
    print("direct_rolling_call")
    if msg.reply_to_message.text:
        if any([x for x in task_list_update if msg.reply_to_message.text.startswith(x)]):
            for x in roles:
                for k in x.kinders:
                    if msg.text == k:
                        rs = msg.reply("/ts " + x.kinders[0])
                        time.sleep(1)
                        rs.delete()
                        return

def sudo_command_call(cli: Client, msg: Message):
    print("sudo_command_call")
    sudos = get_sudos()
    if not any([x for x in sudos if x == msg.from_user.id]):
        msg.reply("sudo only")
        return

    if any([x for x in ct["eqmode"] if f"/{x}" == msg.text.lower()]):
        nolynch_mode[msg.chat.id] = not nolynch_mode.get(msg.chat.id, False)
        msg.reply(f'eqmode: **{nolynch_mode[msg.chat.id]}**')

def werewolf_message_call(cli: Client, msg: Message):
    print("werewolf_message_call")
    if not msg.text:
        return

    if nolynch_mode.get(msg.chat.id, False):
        if msg.text.startswith(voting_message):
            msg.reply("/EQ" + tasksystem_username)
            return

    if msg.text.startswith("#players"):
        msg.reply("New game...")
        msg.pin(False)
        return

    elif player_list_header in msg.text:
        player_count = msg.text.splitlines()[0].replace(player_list_header, "", 1).strip().split('/')
        if final_list_footer in msg.text:
            msg.reply("/CONFIRM" + tasksystem_username)
            time.sleep(2)
            cli.send_message(msg.chat.id, "/CLEAR" + tasksystem_username)
        elif player_count[0] == player_count[1]:
            msg.reply("/NEW" + tasksystem_username)
        else:
            msg.reply("/STSUP" + tasksystem_username)
            

def werewolf_forworded_call(cli: Client, msg: Message):
    print("werewolf_forworded_call")
    if not msg.text:
        return
    
    if any([x for x in new_achiv_header if msg.text.startswith(x)]):
        msg.reply("مبارکههه 🥳")
        return

    else:
        for x in roles_texts:
            for t in x.text:
                if msg.text.startswith(t):
                    rs = msg.reply("/ts " + x.roles)
                    time.sleep(1)
                    rs.delete()
                    return

def superadmin_command_call(cli: Client, msg: Message):
    print("superadmin_command_call")
    if msg.from_user.id != super_admin:
        msg.reply("super only!")
        return

    if msg.text == "/test":
        msg.reply("tested!")

    elif msg.text.startswith('/addsudo'):
        if msg.reply_to_message:
            add_sudo(msg.reply_to_message.from_user.id)
            msg.reply(f"added **{msg.reply_to_message.from_user.first_name}**")
        
        elif msg.text.split(' ').__len__() > 1:
            try:
                add_sudo(int(msg.text.split(' ')[1]))
                msg.reply("added!")
            except:
                msg.reply('failed')
        else:
            msg.reply('unknown!')