from pyrogram.handlers import MessageHandler
from pyrogram import filters
import callbacks as cb
from const import super_admin, sudos, command_trigger, tasksystem_id
import custom_filters as cm
import re

handlers_list = []

werewolf_forword = MessageHandler(
    cb.werewolf_forworded_call,
    filters.group and
    filters.text and
    cm.forwarded_from_user("werewolfbot")
)
handlers_list.append(werewolf_forword)

werewolf_messages = MessageHandler(
    cb.werewolf_message_call,
    filters.group &
    filters.text &
    ~filters.edited &
    filters.user('werewolfbot')
)
handlers_list.append(werewolf_messages)

super_admin_commands = MessageHandler(
    cb.superadmin_command_call,
    filters.command(['test', 'addsudo']) &
    filters.user(super_admin)
)
handlers_list.append(super_admin_commands)

sudo_commads = MessageHandler(
    cb.sudo_command_call,
    filters.group &
    filters.command(['em', 'eqmode'])
)
handlers_list.append(sudo_commads)

get_direct_roles = MessageHandler(
    cb.direct_rolling_call,
    filters.group &
    cm.replied_to_user(tasksystem_id)
)
handlers_list.append(get_direct_roles)

get_datetime = MessageHandler(
    cb.show_time_call,
    filters.group &
    filters.regex(
        r'^(/)?date$|^(/)?time$|^تاریخ$', 
        re.IGNORECASE
    )
    #filters.command(['time', 'date'])
)
handlers_list.append(get_datetime)