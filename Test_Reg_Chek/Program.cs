using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Telegram.Bot.Types;
using Telegram.Bot;
using Google.Apis.Sheets.v4;
using Microsoft.VisualBasic;

namespace Test_Reg_Chek
{
    internal class Program
    {
        static List<string> tags;
        static string[] token;
        static void Main(string[] args)
        {
            
            token = System.IO.File.ReadAllText("C:/configs/RegBotConfigs.txt").Split("\r\n");

            //Google API
            UpdateTagsList();
            //API телеграмм
            var client = new TelegramBotClient(token[0]);
            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task UpdateTagsList() 
        {
            var regDock = new GoogleSheetsReader("C:/configs/regchekertelegrambot.json", token[1]);
            await regDock.GetListOfTages("Ответы на форму", "E");
            //Console.WriteLine(regDock.tags[0]);
            tags = regDock.tags;
        }

       async static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine(exception);
        }

        async static Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                switch (message.Text)
                {
                    case "/start":
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Привет я помогу тебе узнать зарегистрирован ли ты на мероприятие N.\r\nПришли мне список телеграмм тегов участников и я проверю их регистрацию.\r\nПравильно  составленный запрос должен выглядеть так:");
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "@TelegramTeg\r\n@TelegramTeg\r\n@TelegramTeg");
                        break;
                    case "/help":
                        await client.SendTextMessageAsync(update.Message.Chat.Id, "Этот Telegram-бот создан для упрощения процесса проверки регистрации участников соревнований. Основная цель бота — предоставить удобный инструмент, с помощью которого участники мероприятий могут быстро и легко проверить свои регистрации.\r\n\r\ngit:");
                        break;
                    default:
                        ChekTages(message.Text, client, update);
                        break;
                }
            }
        }

        async static void ChekTages(string message, ITelegramBotClient client, Update update) 
        {
            string[] rows = message.Split("\n");
            foreach (string row in rows)
            {
                if (tags.Contains(row))
                {
                    await client.SendTextMessageAsync(update.Message.Chat.Id, $"Участник с тегом {row} зарегистрирован");
                }
                else
                {
                    await client.SendTextMessageAsync(update.Message.Chat.Id, $"Участник с тегом {row} не найден");
                }
            }
        }
    }
    
}

