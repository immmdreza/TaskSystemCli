using ModernTlSharp.TLSharp.Extensions.Types;
using ModernTlSharp.TLSharp.Tl.TL;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TaskSystemCli
{
    class UpdateHandler
    {
        public static async Task UpdateCatched(Update update)
        {
            try
            {
                foreach (TLAbsMessage chnnelMessages in update.ChannelMessages)
                {
                    switch (chnnelMessages)
                    {
                        case TLMessage newChannelMessage:
                            {
                                if (newChannelMessage.Out)
                                {
                                    return;
                                }

                                TLPeerChannel chnl = (TLPeerChannel)newChannelMessage.ToId;

                                TLChannel chat = update.Chats.Cast<TLChannel>()
                                    .FirstOrDefault(x => x.Id == chnl.ChannelId);

                                TLUser sender = update.Users.Cast<TLUser>()
                                    .FirstOrDefault(x => x.Id == newChannelMessage.FromId);

                                await Program.MessageHandler(new Message
                                {
                                    TLChannel = chat,
                                    TLMessage = newChannelMessage,
                                    TLUser = sender
                                }).ConfigureAwait(false);

                                break;
                            }

                        case TLMessageService messageService:
                            {
                                if (messageService.Action is TLMessageActionChatAddUser)
                                {
                                    TLPeerChannel chnl = (TLPeerChannel)messageService.ToId;

                                    TLChannel chat = update.Chats.Cast<TLChannel>()
                                        .FirstOrDefault(x => x.Id == chnl.ChannelId);

                                    TLUser sender = update.Users.Cast<TLUser>()
                                        .FirstOrDefault(x => x.Id == messageService.FromId);

                                    await Program.MessageHandler(new Message
                                    {
                                        TLChannel = chat,
                                        TLMessageService = messageService,
                                        TLUser = sender
                                    }).ConfigureAwait(false);
                                }

                                break;
                            }

                        default:
                            break;
                    }

                    await Task.Delay(500);
                }

                foreach (TLAbsMessage item in update.Messages)
                {
                    if (item is TLMessage message)
                    {
                        TLUser sender = update.Users.Cast<TLUser>()
                        .FirstOrDefault(x => x.Id == message.FromId);

                        await Program.MessageHandler(new Message
                        {
                            TLMessage = message,
                            TLUser = sender
                        }).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
