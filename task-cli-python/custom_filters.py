from pyrogram import filters
from pyrogram.types import Message

def forwarded_from_user(username: str):
    def my_filter(flt, _, message: Message): 
        if message.forward_from:
            return message.forward_from.username == flt.username

    return filters.create(my_filter, username=username)

def replied_to_user(identifire: int):
    def my_filter(flt, _, message: Message): 
        if message.reply_to_message:
            return message.reply_to_message.from_user.id == flt.id

    return filters.create(my_filter, id=identifire)