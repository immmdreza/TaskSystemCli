from typing import Dict
from pyrogram import __version__, Client, idle
from handlers import handlers_list

def main():
    print("starting task system cli...")
    print(__version__)

    app = Client("tasksystem_bot")

    for handler in handlers_list:
        app.add_handler(handler)

    app.start()
    print(app.get_me().username)

    print("done")
    idle()


if __name__ == "__main__":
    main()