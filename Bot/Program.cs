using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace TelgramBotCS
{
    struct BotUpdate
    {
        public string text;
        public long ID;
        public string User;
        public int MessageId;
    }

    internal class Program
    {
        static TelegramBotClient bot = new TelegramBotClient("5475225482:AAFr-kBo2nO9YV_Lnc7-BxdvpLXF2YM9QQ8");
        static string UpdateFileName = "updates.json";
        static string ResponsesFileName = "responses.json";
        static public List<string> Responses = new List<string>();
        static public List<BotUpdate> botUpdates = new List<BotUpdate>();
        static void Main(string[] args)
        {
            try
            {
                var BotUpdateString = System.IO.File.ReadAllText(UpdateFileName);
                var ResponsesString = System.IO.File.ReadAllText(ResponsesFileName);
                botUpdates = JsonConvert.DeserializeObject<List<BotUpdate>>(BotUpdateString) ?? botUpdates;
                Responses = JsonConvert.DeserializeObject<List<string>>(ResponsesString) ?? Responses;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
                {
                    UpdateType.Message,
                    UpdateType.EditedMessage,
                    UpdateType.ChatMember,
                }
            };

            bot.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);
            Console.ReadLine();
        }

        private static async Task ErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
           
        }

        private static async Task UpdateHandler(ITelegramBotClient Bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message)
            {
                if (update.Message.Type == MessageType.Text)
                {
                    BotUpdate _update = new BotUpdate
                    {
                        text = update.Message.Text,
                        User = update.Message.Chat.Username,
                        ID = update.Message.Chat.Id,
                        MessageId = update.Message.MessageId,
                    };

                    botUpdates.Add(_update);
                    string botUpdateString = JsonConvert.SerializeObject(botUpdates);
                    
                    if (_update.text.Contains("/help"))
                    {
                        await bot.SendTextMessageAsync(_update.ID, "هیچ کمکی نیست.", replyToMessageId: _update.MessageId);
                        System.IO.File.WriteAllText(UpdateFileName, botUpdateString);
                    } else if (_update.text.Contains("/youtube"))
                    {
                        await bot.SendTextMessageAsync(_update.ID, "هیچ یوتیوبی نیست.", replyToMessageId: _update.MessageId);
                        System.IO.File.WriteAllText(UpdateFileName, botUpdateString);
                    } else if (_update.text.Contains("/add"))
                    {
                        Responses.Add(_update.text.Replace("/add", ""));
                        await bot.SendTextMessageAsync(_update.ID, "الان خوشحالی؟", replyToMessageId: _update.MessageId);
                        System.IO.File.WriteAllText(UpdateFileName, botUpdateString);
                        string ResponsesString = JsonConvert.SerializeObject(Responses);
                        System.IO.File.WriteAllText(ResponsesFileName, ResponsesString);
                        Console.WriteLine(_update.text);
                    } else
                    {
                        await bot.SendTextMessageAsync(_update.ID, RandomResponse(), replyToMessageId: _update.MessageId);
                        
                    }

                }

            }
            if (update.Type == UpdateType.EditedMessage)
            {
                BotUpdate _update = new BotUpdate
                {
                    text = update.EditedMessage.Text,
                    User = update.EditedMessage.Chat.Username,
                    ID = update.EditedMessage.Chat.Id,
                    MessageId = update.EditedMessage.MessageId,
                };
                await bot.SendTextMessageAsync(_update.ID, "ادیت نزن کصکش. این تقلبه.", replyToMessageId: _update.MessageId);
            }
            
        }
        public static string RandomResponse()
        {
            Random rand = new Random();
            return Responses[rand.Next(0, Responses.Count)];
        }
    }

}
