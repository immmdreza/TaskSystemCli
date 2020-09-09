using ModernTlSharp.TLSharp.Core;
using ModernTlSharp.TLSharp.Extensions;
using ModernTlSharp.TLSharp.Extensions.Types;
using ModernTlSharp.TLSharp.Tl;
using ModernTlSharp.TLSharp.Tl.TL;
using ModernTlSharp.TLSharp.Tl.TL.Channels;
using ModernTlSharp.TLSharp.Tl.TL.Contacts;
using ModernTlSharp.TLSharp.Tl.TL.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskSystemCli.DataCenter;

namespace TaskSystemCli
{
    class Program
    {
        private static readonly int API_ID = 1010101;
        private static readonly string API_HASH = "API_HASH";
        private static readonly int[] WerewolfBots = new[]
        {
            175844556,
            198626752
        };
        private static readonly string PlayerListHeader = "بازیکن های زنده:";
        private static readonly string FinalListFooter = "مدت زمان بازی:";
        private static readonly string VotingMessage = "خب دیگه شب شده و همه جمع میشن که رای بدن";
        private static readonly string JoinedMessage = "شما با موفقیت وارد بازی در گروه";
        private static readonly string RemainTime = "فقط 30  ثانیه دیگه وقت دارید که وارد بازی شید...";
        private static readonly string[] TaskUpdateList = new string[] { "اوکی لیست بازیکنان آپدیت شد", "لیست نقشها تا به اینجا" };
        private static readonly string FetchedText = "#Roles_fetcher\n\nReply me your roles!";
        private static readonly string[] NewAchive = new string[]
        {
            "Achievement Unlocked!",
            "New Unlocks!"
        };
        private static readonly ConcurrentDictionary<int, bool> NoLynchMode = new ConcurrentDictionary<int, bool>();
        //private static ConcurrentDictionary<int, List<int>> RoleFetcherMsgid = new ConcurrentDictionary<int, List<int>>();
        private const string DataCenterPath = "./DataCenter";
        private const int SuperAdmin = 106296897;
        private const int TaskId = 724104884;
        private const string BotUsername = "@TsWwPlus_Bot";
        private static List<RolesText> RolesTexts;
        private static List<Roles> _Roles;
        private static bool JoinedGame = false;
        private static readonly TLInputChannel tLInputChannel = new TLInputChannel();
        private static int startMessageId = 0;
        private static string myRoles = "";
        private static bool autoPlay = false;
        private static bool autoJoin = false;

        private static readonly Dictionary<string, string[]> Triggers =
            new Dictionary<string, string[]>
            {
                {"joingame", new[] { "/jg", "/joingame" } },
                {"eqmode", new[] { "/em", "/eqmode" } },
                {"autogame", new[] { "/ag", "/autogame" } },
                {"autojoin", new[] { "/aj", "/autojoin" } },
                {"rolegame", new[] { "/rg", "/rolegame" } },
                {"clickgame", new[] { "/cg", "/clickgame" } },
                {"leavegame", new[] { "/lg", "/leavegame" } },
                {"addsudo", new[] { "/as", "/addsudo" } },
                {"ping", new[] { "/p", "/ping", "ping", "پینگ" } },
                {"ts", new[] { "/ta", "/sn", "sr" } }
            };

        public static TelegramClient TelegramClient { get; private set; }

        static async Task Main()
        {
            RolesTexts = await RolesText.GetRolesTextsAsync();

            _Roles = await Roles.GetRolesAsync();

            Console.WriteLine("Waitting for client...");

            TelegramClient = new TelegramClient(API_ID, API_HASH);

            await TelegramClient.ConnectAsync();

            Authorization auth = new Authorization(TelegramClient);

            await auth.ConsoleAuthocate();

            Console.WriteLine("reading messages");

            await TelegramClient.UpdateCatcher(UpdateHandler.UpdateCatched);
        }

        /// <summary>
        /// Main Handler
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task<bool> MessageHandler(Message message)
        {
            switch (message)
            {
                case { TLChannel: { } channel, TLMessage: { } tlMessage }:
                    {
                        if (message.TLUser != null)
                        {
                            if (tlMessage.ReplyToMsgId != null)
                            {
                                TLChannelMessages msg = await TelegramClient.SendRequestAsync<TLChannelMessages>(
                                    new ModernTlSharp.TLSharp.Tl.TL.Channels.TLRequestGetMessages
                                    {
                                        Id = new TLVector<int>
                                        {
                                            tlMessage.ReplyToMsgId.Value
                                        },
                                        Channel = new TLInputChannel
                                        {
                                            AccessHash = message.TLChannel.AccessHash.Value,
                                            ChannelId = message.TLChannel.Id
                                        },
                                    });

                                TLMessage target = msg.Messages[0] as TLMessage;

                                if (target.FromId == TaskId)
                                {
                                    if (TaskUpdateList.Any(x=> target.Message.StartsWith(x)))
                                    {
                                        if (_Roles.FirstOrDefault(x => x.Kinders.Any(y => y.Trim() == tlMessage.Message.Trim()))
                                            is Roles roles)
                                        {
                                            TLUpdates res = (TLUpdates)await TelegramClient.SendTextMessageAsync(

                                                tLChannelId: channel.Id,
                                                accessHash: (long)channel.AccessHash,
                                                text: "/TS " + roles.Kinders[0],
                                                replyToMessageId: tlMessage.Id
                                            );

                                            int msgid = res.Updates
                                                .Where(x => x is TLUpdateMessageID)
                                                .Cast<TLUpdateMessageID>()
                                                .SingleOrDefault().Id;

                                            await Task.Delay(1000);
                                            
                                            _ = await TelegramClient.SendRequestAsync<TLAffectedMessages>(
                                                new ModernTlSharp.TLSharp.Tl.TL.Channels.TLRequestDeleteMessages
                                                {
                                                    Channel = new TLInputChannel
                                                    {
                                                        ChannelId = channel.Id,
                                                        AccessHash = channel.AccessHash.Value
                                                    },
                                                    Id = new TLVector<int>
                                                    {
                                                    msgid
                                                    }
                                                });

                                            return true;
                                        }
                                        else
                                        {
                                            await TelegramClient.SendTextMessageAsync(

                                                tLChannelId: channel.Id,
                                                accessHash: (long)channel.AccessHash,
                                                text: "invalid role ❌",
                                                replyToMessageId: tlMessage.Id
                                            );
                                        }
                                    }
                                }
                            }

                            if (tlMessage.FwdFrom != null)
                            {
                                if (WerewolfBots.Any(x => x == tlMessage.FwdFrom.FromId))
                                {
                                    if (NewAchive.Any(x => tlMessage.Message.StartsWith(x)))
                                    {
                                        await TelegramClient.SendTextMessageAsync(

                                            tLChannelId: channel.Id,
                                            accessHash: (long)channel.AccessHash,
                                            text: "مبارکههه 🥳",
                                            replyToMessageId: tlMessage.Id
                                        );

                                        return true;
                                    }

                                    if (RolesTexts.FirstOrDefault(x => x.Text.Any(y => y.Trim() == tlMessage.Message.Trim()))
                                        is RolesText rolesText)
                                    {

                                        await TelegramClient.SendTextMessageAsync(

                                            tLChannelId: channel.Id,
                                            accessHash: (long)channel.AccessHash,
                                            text: "/ts " + rolesText.Roles,
                                            replyToMessageId: tlMessage.Id
                                        );

                                        return true;
                                    }
                                }
                            }

                            if (Triggers["eqmode"].Any(x => tlMessage.Message.ToLower() == x))
                            {
                                List<int> mods = await Sudos.GetSudosAsync();

                                if (mods.Any(x => x == message.TLUser.Id))
                                {
                                    if (!NoLynchMode.TryAdd(channel.Id, true))
                                    {
                                        NoLynchMode[channel.Id] = !NoLynchMode[channel.Id];
                                    }

                                    await TelegramClient.SendTextMessageAsync(

                                        tLChannelId: channel.Id,
                                        accessHash: (long)channel.AccessHash,
                                        text: "No lynch mode: " + NoLynchMode[channel.Id].ToString(),
                                        replyToMessageId: tlMessage.Id
                                    );

                                    return true;
                                }

                                return false;
                            }

                            if (Triggers["autogame"].Any(x => tlMessage.Message.ToLower() == x))
                            {
                                List<int> mods = await Sudos.GetSudosAsync();

                                if (mods.Any(x => x == message.TLUser.Id))
                                {
                                    autoPlay = !autoPlay;

                                    await TelegramClient.SendTextMessageAsync(

                                        tLChannelId: channel.Id,
                                        accessHash: (long)channel.AccessHash,
                                        text: "Auto game: " + autoPlay.ToString(),
                                        replyToMessageId: tlMessage.Id
                                    );

                                    return true;
                                }

                                return false;
                            }

                            if (Triggers["autojoin"].Any(x => tlMessage.Message.ToLower() == x))
                            {
                                List<int> mods = await Sudos.GetSudosAsync();

                                if (mods.Any(x => x == message.TLUser.Id))
                                {
                                    autoJoin = !autoJoin;

                                    await TelegramClient.SendTextMessageAsync(

                                        tLChannelId: channel.Id,
                                        accessHash: (long)channel.AccessHash,
                                        text: "Auto join: " + autoJoin.ToString(),
                                        replyToMessageId: tlMessage.Id
                                    );

                                    return true;
                                }

                                return false;
                            }

                            if (Triggers["rolegame"].Any(x => tlMessage.Message.ToLower() == x))
                            {
                                if (!string.IsNullOrEmpty(myRoles))
                                {
                                    await TelegramClient.SendTextMessageAsync(

                                        tLChannelId: channel.Id,
                                        accessHash: (long)channel.AccessHash,
                                        text: myRoles
                                    );
                                }

                                return true;
                            }

                            if (Triggers["clickgame"].Any(x => tlMessage.Message.StartsWith(x)))
                            {
                                List<int> mods = await Sudos.GetSudosAsync();

                                if (mods.Any(x => x == message.TLUser.Id))
                                {
                                    string targetName = "";

                                    if (tlMessage.ReplyToMsgId != null)
                                    {
                                        TLChannelMessages replymsg = await TelegramClient.SendRequestAsync<TLChannelMessages>(
                                            new ModernTlSharp.TLSharp.Tl.TL.Channels.TLRequestGetMessages
                                            {
                                                Channel = new TLInputChannel
                                                {
                                                    AccessHash = message.TLChannel.AccessHash.Value,
                                                    ChannelId = message.TLChannel.Id
                                                },
                                                Id = new TLVector<int> { tlMessage.ReplyToMsgId.Value }
                                            });

                                        TLUser user = replymsg.Users.Where(x => x is TLUser).Cast<TLUser>()
                                            .Where(x => x.Id != message.TLUser.Id).SingleOrDefault();

                                        if (user != null)
                                        {
                                            targetName = user.FirstName;
                                        }
                                    }
                                    else
                                    {
                                        if (tlMessage.Message.Split(' ').Length > 1)
                                        {
                                            string username = tlMessage.Message.Split(' ')[1];

                                            if (username[0] == '@')
                                            {
                                                try
                                                {
                                                    TLResolvedPeer user = await TelegramClient.SendRequestAsync<TLResolvedPeer>(
                                                        new TLRequestResolveUsername
                                                        {
                                                            Username = username.Replace("@", "")
                                                        });

                                                    targetName = ((TLUser)user.Users[0]).FirstName;
                                                }
                                                catch (InvalidOperationException)
                                                {
                                                    targetName = "";
                                                }
                                            }
                                            else
                                            {
                                                targetName = username;
                                            }
                                        }
                                    }

                                    TLDialogsSlice dialogs = (TLDialogsSlice)await TelegramClient.GetUserDialogsAsync(limit: 50);

                                    TLUser bot = dialogs.Users
                                        .Cast<TLUser>()
                                        .FirstOrDefault(x => x.Bot && x.Username.StartsWith("werewolf"));

                                    TLAbsMessages history = await TelegramClient.GetHistoryAsync(
                                        new TLInputPeerUser
                                        {
                                            UserId = bot.Id,
                                            AccessHash = bot.AccessHash.Value
                                        },
                                        limit: 20);

                                    if (history is TLMessagesSlice h1)
                                    {
                                        if (h1.Messages.Cast<TLMessage>().FirstOrDefault(
                                            x => x.ReplyMarkup != null) is TLMessage tmsg)
                                        {
                                            if (tmsg.ReplyMarkup is TLReplyInlineMarkup markup)
                                            {
                                                if (string.IsNullOrEmpty(targetName))
                                                {
                                                    TLAbsKeyboardButton btn = markup.Rows.Last().Buttons.Last();
                                                    if (btn is TLKeyboardButtonCallback callback)
                                                    {
                                                        try
                                                        {
                                                            TLBotCallbackAnswer res = await TelegramClient.SendRequestAsync<TLBotCallbackAnswer>(
                                                                new TLRequestGetBotCallbackAnswer
                                                                {
                                                                    Data = callback.Data,
                                                                    MsgId = tmsg.Id,
                                                                    Peer = new TLInputPeerUser
                                                                    {
                                                                        UserId = bot.Id,
                                                                        AccessHash = bot.AccessHash.Value
                                                                    }
                                                                });
                                                        }
                                                        catch
                                                        {
                                                            await TelegramClient.SendTextMessageAsync(

                                                                tLChannelId: channel.Id,
                                                                accessHash: (long)channel.AccessHash,
                                                                text: "bot dosen't respond. try again."
                                                            );
                                                        }

                                                        await TelegramClient.SendTextMessageAsync(

                                                            tLChannelId: channel.Id,
                                                            accessHash: (long)channel.AccessHash,
                                                            text: "Order proceed ✅"
                                                        );

                                                        return true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (markup.Rows.Any(x => x.Buttons[0] is TLKeyboardButtonCallback callback))
                                                    {
                                                        TLKeyboardButtonRow skiprow = markup.Rows.FirstOrDefault(x =>
                                                        {
                                                            return x.Buttons.Any(y =>
                                                            {
                                                                if (y is TLKeyboardButtonCallback t)
                                                                {
                                                                    if (t.Text == targetName)
                                                                    {
                                                                        return true;
                                                                    }
                                                                    else { return false; }
                                                                }
                                                                else { return false; }
                                                            });
                                                        });

                                                        if (skiprow != null)
                                                        {
                                                            TLBotCallbackAnswer res = await TelegramClient.SendRequestAsync<TLBotCallbackAnswer>(
                                                                new TLRequestGetBotCallbackAnswer
                                                                {
                                                                    Data = ((TLKeyboardButtonCallback)skiprow.Buttons[0]).Data,
                                                                    MsgId = tmsg.Id,
                                                                    Peer = new TLInputPeerUser
                                                                    {
                                                                        UserId = bot.Id,
                                                                        AccessHash = bot.AccessHash.Value
                                                                    }
                                                                });

                                                            await TelegramClient.SendTextMessageAsync(

                                                                tLChannelId: channel.Id,
                                                                accessHash: (long)channel.AccessHash,
                                                                text: "Order proceed ✅"
                                                            );

                                                            return true;
                                                        }
                                                        else
                                                        {
                                                            await TelegramClient.SendTextMessageAsync(

                                                                tLChannelId: channel.Id,
                                                                accessHash: (long)channel.AccessHash,
                                                                text: "Erorr 404 Not Found!⛔"
                                                            );

                                                            return true;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {

                                                await TelegramClient.SendTextMessageAsync(

                                                    tLChannelId: channel.Id,
                                                    accessHash: (long)channel.AccessHash,
                                                    text: "Erorr 404 Not Found!⛔"
                                                );

                                                return true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        TLMessages h2 = (TLMessages)history;
                                        if (h2.Messages.Cast<TLMessage>().FirstOrDefault(
                                            x => x.ReplyMarkup != null) is TLMessage tmsg)
                                        {
                                            if (tmsg.ReplyMarkup is TLReplyInlineMarkup markup)
                                            {
                                                if (string.IsNullOrEmpty(targetName))
                                                {
                                                    TLAbsKeyboardButton btn = markup.Rows.Last().Buttons.Last();
                                                    if (btn is TLKeyboardButtonCallback callback)
                                                    {
                                                        try
                                                        {
                                                            TLBotCallbackAnswer res = await TelegramClient.SendRequestAsync<TLBotCallbackAnswer>(
                                                                new TLRequestGetBotCallbackAnswer
                                                                {
                                                                    Data = callback.Data,
                                                                    MsgId = tmsg.Id,
                                                                    Peer = new TLInputPeerUser
                                                                    {
                                                                        UserId = bot.Id,
                                                                        AccessHash = bot.AccessHash.Value
                                                                    }
                                                                });
                                                        }
                                                        catch
                                                        {
                                                            await TelegramClient.SendTextMessageAsync(

                                                                tLChannelId: channel.Id,
                                                                accessHash: (long)channel.AccessHash,
                                                                text: "bot dosen't respond. try again."
                                                            );
                                                        }

                                                        await TelegramClient.SendTextMessageAsync(

                                                            tLChannelId: channel.Id,
                                                            accessHash: (long)channel.AccessHash,
                                                            text: "Order proceed ✅"
                                                        );

                                                        return true;
                                                    }
                                                }
                                                else
                                                {
                                                    if (markup.Rows.Any(x => x.Buttons[0] is TLKeyboardButtonCallback callback))
                                                    {
                                                        TLKeyboardButtonRow skiprow = markup.Rows.FirstOrDefault(x =>
                                                        {
                                                            return x.Buttons.Any(y =>
                                                            {
                                                                if (y is TLKeyboardButtonCallback t)
                                                                {
                                                                    if (t.Text == targetName)
                                                                    {
                                                                        return true;
                                                                    }
                                                                    else { return false; }
                                                                }
                                                                else { return false; }
                                                            });
                                                        });

                                                        if (skiprow != null)
                                                        {
                                                            TLBotCallbackAnswer res = await TelegramClient.SendRequestAsync<TLBotCallbackAnswer>(
                                                                new TLRequestGetBotCallbackAnswer
                                                                {
                                                                    Data = ((TLKeyboardButtonCallback)skiprow.Buttons[0]).Data,
                                                                    MsgId = tmsg.Id,
                                                                    Peer = new TLInputPeerUser
                                                                    {
                                                                        UserId = bot.Id,
                                                                        AccessHash = bot.AccessHash.Value
                                                                    }
                                                                });

                                                            await TelegramClient.SendTextMessageAsync(

                                                                tLChannelId: channel.Id,
                                                                accessHash: (long)channel.AccessHash,
                                                                text: "Order proceed ✅"
                                                            );

                                                            return true;
                                                        }
                                                        else
                                                        {
                                                            await TelegramClient.SendTextMessageAsync(

                                                                tLChannelId: channel.Id,
                                                                accessHash: (long)channel.AccessHash,
                                                                text: "Erorr 404 Not Found!⛔"
                                                            );

                                                            return true;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {

                                                await TelegramClient.SendTextMessageAsync(

                                                    tLChannelId: channel.Id,
                                                    accessHash: (long)channel.AccessHash,
                                                    text: "Erorr 404 Not Found!⛔"
                                                );

                                                return true;
                                            }
                                        }
                                    }

                                    return true;
                                }

                                return false;
                            }

                            if (Triggers["leavegame"].Any(x => tlMessage.Message.ToLower() == x))
                            {
                                List<int> mods = await Sudos.GetSudosAsync();

                                if (mods.Any(x => x == message.TLUser.Id))
                                {
                                    if (JoinedGame)
                                    {
                                        await TelegramClient.SendTextMessageAsync(

                                            tLChannelId: channel.Id,
                                            accessHash: (long)channel.AccessHash,
                                            text: "/FLEE"
                                        );

                                        JoinedGame = false;
                                    }
                                    else
                                    {
                                        await TelegramClient.SendTextMessageAsync(

                                            tLChannelId: channel.Id,
                                            accessHash: (long)channel.AccessHash,
                                            text: "Oh which game?!"
                                        );
                                    }

                                    return true;
                                }

                                return false;
                            }

                            if (Triggers["joingame"].Any(x => tlMessage.Message.ToLower() == x))
                            {
                                if (JoinedGame)
                                {
                                    return true;
                                }

                                if (tlMessage.ReplyToMsgId != null)
                                {
                                    List<int> mods = await Sudos.GetSudosAsync();

                                    if (mods.Any(x => x == message.TLUser.Id))
                                    {
                                        TLChannelMessages msg = await TelegramClient.SendRequestAsync<TLChannelMessages>(
                                            new ModernTlSharp.TLSharp.Tl.TL.Channels.TLRequestGetMessages
                                            {
                                                Id = new TLVector<int>
                                                {
                                                    tlMessage.ReplyToMsgId.Value
                                                },
                                                Channel = new TLInputChannel
                                                {
                                                    AccessHash = message.TLChannel.AccessHash.Value,
                                                    ChannelId = message.TLChannel.Id
                                                },
                                            });

                                        string joinUrl = "";
                                        TLUser bot = null;
                                        if (WerewolfBots.Any(x => x == ((TLUser)msg.Users[^1]).Id))
                                        {
                                            bot = (TLUser)msg.Users[^1];
                                            TLMessage mess = (TLMessage)msg.Messages[0];
                                            if (mess.Media != null)
                                            {
                                                if (mess.ReplyMarkup is TLReplyInlineMarkup inlineMarkup)
                                                {
                                                    if (inlineMarkup.Rows.Any())
                                                    {
                                                        if (inlineMarkup.Rows[0].Buttons[0] is TLKeyboardButtonUrl url)
                                                        {
                                                            joinUrl = url.Url;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(joinUrl))
                                        {
                                            await TelegramClient.SendTextMessageAsync(

                                                tLChannelId: channel.Id,
                                                accessHash: (long)channel.AccessHash,
                                                text: "Ok i'll send join request, if you can't find me in joined players you can try again."
                                                + "\n\nIf you wanna smite me and you can't, try /leavegame"
                                            );

                                            await TelegramClient.SendRequestAsync<TLUpdates>(
                                                new TLRequestStartBot
                                                {
                                                    Bot = new TLInputUser
                                                    {
                                                        AccessHash = bot.AccessHash.Value,
                                                        UserId = bot.Id
                                                    },
                                                    Peer = new TLInputPeerUser
                                                    {
                                                        AccessHash = bot.AccessHash.Value,
                                                        UserId = bot.Id
                                                    },
                                                    StartParam = joinUrl.Split('=')[^1],
                                                    RandomId = new Random().Next(0, 100000)
                                                });

                                            JoinedGame = true;

                                            tLInputChannel.ChannelId = channel.Id;
                                            tLInputChannel.AccessHash = (long)channel.AccessHash;
                                        }
                                        else
                                        {
                                            await TelegramClient.SendTextMessageAsync(

                                                tLChannelId: channel.Id,
                                                accessHash: (long)channel.AccessHash,
                                                text: "Failed"
                                            );
                                        }

                                        return true;
                                    }
                                }

                                return false;
                            }

                            if (tlMessage.Message == "لیست مخفف ها")
                            {
                                List<int> mods = await Sudos.GetSudosAsync();

                                if (mods.Any(x => x == message.TLUser.Id))
                                {
                                    List<Roles> roles = await Roles.GetRolesAsync();

                                    string text = "لیست نقشها:\n";
                                    foreach (Roles item in roles)
                                    {
                                        text += $"\n~ {item.FullName} ({item.Emoji}):\n" + string.Join(", ", item.Kinders);
                                    }

                                    await TelegramClient.SendTextMessageAsync(

                                        tLChannelId: channel.Id,
                                        accessHash: (long)channel.AccessHash,
                                        text: text,
                                        replyToMessageId: tlMessage.Id
                                    );

                                    return true;
                                }

                                return false;
                            }

                            if (Triggers["addsudo"].Any(x => tlMessage.Message.StartsWith(x)))
                            {
                                if (message.TLUser.Id == SuperAdmin)
                                {
                                    if (int.TryParse(tlMessage.Message.Split(' ')[1] ?? "", out int id))
                                    {
                                        await Sudos.AddSudoAsync(id);


                                        await TelegramClient.SendTextMessageAsync(

                                            tLChannelId: channel.Id,
                                            accessHash: (long)channel.AccessHash,
                                            text: "Added!",
                                            replyToMessageId: tlMessage.Id
                                        );

                                        return true;
                                    }
                                }

                                return false;

                            }

                            if (Triggers["ts"].Any(x => tlMessage.Message.StartsWith(x)))
                            {
                                string arg = "";
                                if(tlMessage.Message.Split(' ').Length > 1)
                                {
                                    arg = tlMessage.Message.Split(' ')[1];
                                }

                                if (string.IsNullOrEmpty(arg))
                                {
                                    return true;
                                }

                                await TelegramClient.SendTextMessageAsync(

                                    tLChannelId: channel.Id,
                                    accessHash: (long)channel.AccessHash,
                                    text: "/ts " + arg,
                                    replyToMessageId: tlMessage.Id
                                );

                                return true;
                            }

                            //process werewolf bots messages.
                            if (message.TLUser.Bot && WerewolfBots.Any(x => x == message.TLUser.Id))
                            {
                                string text = tlMessage.Message.ToLower();

                                if (string.IsNullOrEmpty(text))
                                {
                                    return false;
                                }

                                if (text.StartsWith("#players"))
                                {
                                    await TelegramClient.SendTextMessageAsync(

                                        tLChannelId: channel.Id,
                                        accessHash: (long)channel.AccessHash,
                                        text: "New game ... 😍",
                                        replyToMessageId: tlMessage.Id
                                    );

                                    startMessageId = tlMessage.Id;

                                    try
                                    {
                                        await TelegramClient.SendRequestAsync<TLAbsUpdates>(
                                            new TLRequestUpdatePinnedMessage
                                            {
                                                Channel = new TLInputChannel
                                                {
                                                    AccessHash = channel.AccessHash.Value,
                                                    ChannelId = channel.Id
                                                },
                                                Silent = false,
                                                Id = tlMessage.Id,
                                            });
                                    }
                                    catch { }

                                    return true;
                                }

                                if (text == RemainTime)
                                {
                                    TLChannelMessages startMessage = await TelegramClient.SendRequestAsync<TLChannelMessages>(
                                        new ModernTlSharp.TLSharp.Tl.TL.Channels.TLRequestGetMessages
                                        {
                                            Channel = new TLInputChannel
                                            {
                                                AccessHash = channel.AccessHash.Value,
                                                ChannelId = channel.Id
                                            },
                                            Id = new TLVector<int> { startMessageId }
                                        });

                                    TLMessage m = startMessage.Messages
                                        .Where(x => x is TLMessage)
                                        .Cast<TLMessage>()
                                        .SingleOrDefault();

                                    if (m != null && m.Message.Split('\n').Length < 6)
                                    {
                                        if (m.Mentioned)
                                        {
                                            await TelegramClient.SendTextMessageAsync(

                                                tLChannelId: channel.Id,
                                                accessHash: (long)channel.AccessHash,
                                                text: "/EXTEND 60"
                                            );
                                        }
                                        else
                                        {
                                            if (!autoJoin)
                                            {
                                                return true;
                                            }

                                            string joinUrl = "";
                                            TLUser bot = null;
                                            bot = message.TLUser;

                                            if (tlMessage.ReplyMarkup is TLReplyInlineMarkup inlineMarkup)
                                            {
                                                if (inlineMarkup.Rows.Any())
                                                {
                                                    if (inlineMarkup.Rows[0].Buttons[0] is TLKeyboardButtonUrl url)
                                                    {
                                                        joinUrl = url.Url;
                                                    }
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(joinUrl))
                                            {
                                                await TelegramClient.SendTextMessageAsync(

                                                    tLChannelId: channel.Id,
                                                    accessHash: (long)channel.AccessHash,
                                                    text: "Ok looks like you need some players, joining... Try /joingame if failed."
                                                    + "\n\n**Note that auto play is enabled**, use /autogame to disable."
                                                );

                                                await TelegramClient.SendRequestAsync<TLUpdates>(
                                                    new TLRequestStartBot
                                                    {
                                                        Bot = new TLInputUser
                                                        {
                                                            AccessHash = bot.AccessHash.Value,
                                                            UserId = bot.Id
                                                        },
                                                        Peer = new TLInputPeerUser
                                                        {
                                                            AccessHash = bot.AccessHash.Value,
                                                            UserId = bot.Id
                                                        },
                                                        StartParam = joinUrl.Split('=')[^1],
                                                        RandomId = new Random().Next(0, 100000)
                                                    });

                                                tLInputChannel.ChannelId = channel.Id;
                                                tLInputChannel.AccessHash = (long)channel.AccessHash;

                                                autoPlay = true;
                                            }
                                            else
                                            {
                                                await TelegramClient.SendTextMessageAsync(

                                                    tLChannelId: channel.Id,
                                                    accessHash: (long)channel.AccessHash,
                                                    text: "Failed"
                                                );
                                            }
                                        }
                                    }

                                    return true;
                                }

                                if (NoLynchMode.TryGetValue(channel.Id, out bool mode))
                                {
                                    if (mode)
                                    {
                                        if (text.StartsWith(VotingMessage))
                                        {
                                            await TelegramClient.SendTextMessageAsync(

                                                tLChannelId: channel.Id,
                                                accessHash: (long)channel.AccessHash,
                                                text: "/eq" + BotUsername
                                            );

                                            return true;
                                        }
                                    }
                                }

                                if (text.Contains(PlayerListHeader))
                                {
                                    string[] playerCount = text.Split('\n')[0]
                                        .Remove(0, PlayerListHeader.Length)
                                        .Trim()
                                        .Split('/');

                                    if (text.Split('\n')[^1].StartsWith(FinalListFooter))
                                    {
                                        if (JoinedGame)
                                        {
                                            JoinedGame = !JoinedGame;
                                        }

                                        if (NoLynchMode.TryAdd(channel.Id, false))
                                        {
                                            NoLynchMode[channel.Id] = false;
                                        }

                                        await channel.SendTextMessageAsync(
                                            telegramClient: TelegramClient,
                                            text: "/confirm" + BotUsername,
                                            replyToMessageId: tlMessage.Id
                                        );

                                        await Task.Delay(2000);

                                        await channel.SendTextMessageAsync(
                                            telegramClient: TelegramClient,
                                            text: "/clear" + BotUsername
                                        );

                                        if (!string.IsNullOrEmpty(myRoles))
                                        {
                                            myRoles = "";
                                        }

                                        return true;
                                    }

                                    //Starting players List!
                                    if (playerCount[0] == playerCount[1])
                                    {
                                        await channel.SendTextMessageAsync(
                                            telegramClient: TelegramClient,
                                            text: "/new" + BotUsername,
                                            replyToMessageId: tlMessage.Id
                                        );

                                        if (tlMessage.Mentioned)
                                        {
                                            await Task.Delay(2000);

                                            await channel.SendTextMessageAsync(
                                                telegramClient: TelegramClient,
                                                text: myRoles
                                            );
                                        }

                                        return true;
                                    }
                                    else
                                    {
                                        await channel.SendTextMessageAsync(
                                            telegramClient: TelegramClient,
                                            text: "/stsup" + BotUsername,
                                            replyToMessageId: tlMessage.Id
                                        );

                                        return true;
                                    }
                                }
                            }

                            #region TasksMessages
                            //if (message.TLUser.Id == TaskId)
                            //{
                            //    if (message.TLMessage.Message.StartsWith(TaskUpdateList))
                            //    {
                            //        var res = (TLUpdates)await TelegramClient.SendTextMessageAsync(

                            //            tLChannelId: channel.Id,
                            //            accessHash: (long)channel.AccessHash,
                            //            text: FetchedText
                            //        );

                            //        var msgid = res.Updates
                            //            .Where(x => x is TLUpdateMessageID)
                            //            .Cast<TLUpdateMessageID>()
                            //            .SingleOrDefault().Id;

                            //        if(!RoleFetcherMsgid.TryAdd(channel.Id, new List<int> { msgid }))
                            //        {
                            //            RoleFetcherMsgid[channel.Id].Add(msgid);
                            //        }
                            //    }
                            //}
                            #endregion
                        }

                        if (Triggers["ping"].Any(x => tlMessage.Message.ToLower() == x))
                        {
                            await TelegramClient.SendTextMessageAsync(

                                tLChannelId: channel.Id,
                                accessHash: (long)channel.AccessHash,
                                text: "Pong",
                                replyToMessageId: tlMessage.Id
                            );

                            await TelegramClient.MakeSeenChannel(message.TLChannel.Id,
                                message.TLChannel.AccessHash.Value, tlMessage.Id);
                        }

                        return true;
                    }

                case { TLChannel: { } chnnl, TLMessageService: { } service }:
                    {
                        if (service.Mentioned)
                        {
                            await TelegramClient.SendTextMessageAsync(

                                tLChannelId: chnnl.Id,
                                accessHash: (long)chnnl.AccessHash,
                                text: "Oh looks like someone added me here!"
                            );
                        }

                        return true;
                    }

                default:
                    {
                        if (message.TLMessage.Message == "test")
                        {
                            await message.TLUser.SendTextMessageAsync(
                                 telegramClient: TelegramClient,
                                 text: "/stsup" + BotUsername
                             );
                        }

                        //Message From werewof bot
                        if (WerewolfBots.Any(x => x == message.TLUser.Id))
                        {
                            if (message.TLMessage.Message.StartsWith(JoinedMessage))
                            {
                                JoinedGame = true;
                                return true;
                            }

                            if (RolesTexts.FirstOrDefault(x => x.Text.Any(y => y.Trim() == message.TLMessage.Message.Trim()))
                                is RolesText rolesText)
                            {

                                myRoles = "/TS " + rolesText.Roles;

                                return true;
                            }

                            if (message.TLMessage.ReplyMarkup is TLReplyInlineMarkup inlineMarkup)
                            {
                                if (!autoPlay)
                                {
                                    return true;
                                }

                                if (inlineMarkup.Rows.All(x => x.Buttons.All(b => b is TLKeyboardButtonCallback)))
                                {
                                    Random rnd = new Random();
                                    int rowIndex = rnd.Next(0, inlineMarkup.Rows.Count);
                                    int btnIndex = rnd.Next(0, inlineMarkup.Rows[rowIndex].Buttons.Count);

                                    TLKeyboardButtonCallback btn = inlineMarkup.Rows[rowIndex].Buttons[btnIndex] as TLKeyboardButtonCallback;

                                    TLBotCallbackAnswer res = await TelegramClient.SendRequestAsync<TLBotCallbackAnswer>(
                                        new TLRequestGetBotCallbackAnswer
                                        {
                                            Data = btn.Data,
                                            MsgId = message.TLMessage.Id,
                                            Peer = new TLInputPeerUser
                                            {
                                                UserId = message.TLUser.Id,
                                                AccessHash = message.TLUser.AccessHash.Value
                                            }
                                        });

                                    return true;
                                }
                            }

                            return false;
                        }

                        return false;
                    }
            }
        }

        private static string GetFile(string filename)
        {
            string[] dir = Directory.GetFiles(DataCenterPath);

            return dir.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x).ToLower() == filename);
        }

    }
}
