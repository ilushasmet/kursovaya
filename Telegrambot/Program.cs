using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net.Http;
using Newtonsoft.Json;

namespace Telegrambot
{
    // 1144558187:AAGugNj1PjNlJIdnckB1Dq50v__fA1rkhnc
    class Program
    {
        static Dictionary<int, string> status = new Dictionary<int, string>();
        static TelegramBotClient Bot;
        static Dictionary<int, int> n = new Dictionary<int, int>();
        static string host = "?????";
        static string localhost = "https://localhost:44338";
        static void Main(string[] args)
        {
            host = localhost; //телеграм бот пересилає запити на локальний комп. Щоб запити йшли на азур, закоментую цю строку 
            Bot = new TelegramBotClient("1228987807:AAFEFboNX-uIIVn3wiZcqISpYhL2S7AAcSY");
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            var me = Bot.GetMeAsync().Result;
            Console.WriteLine(me.FirstName);
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        class log
        {
            public int id { get; set; }
            public string info { get; set; }
        }
        private static async void BotOnCallbackQueryReceived(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var client = new HttpClient();
            string buttonText = e.CallbackQuery.Data;
            string name = $"{ e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} натиснув кнопку {buttonText}");

            log l = new log { id = e.CallbackQuery.From.Id, info = $"{name} натиснув кнопку {buttonText}" };
            var json = JsonConvert.SerializeObject(l);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = host + "/api/Test";
            var response = await client.PostAsync(url, data);
            string result = response.Content.ReadAsStringAsync().Result;

            try
            {
                switch (buttonText)
                {
                    case "пошук гіфки по слову":
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Введи слово, яке шукати на gfycat.com");
                        status[e.CallbackQuery.From.Id] = "пошук гіфки по слову";
                        break;
                    case "пошук mp4 по слову":
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Введи слово, яке шукати на gfycat.com");
                        status[e.CallbackQuery.From.Id] = "пошук mp4 по слову";
                        break;                    
                    case "історія":
                        var content = await client.GetStringAsync(host + "/api/Test/hisory/" + e.CallbackQuery.From.Id);
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, content.Replace("\\n", "\n"));
                        break;
                    case "видалити":
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Введи номер запису, який бажаєш видалити");
                        status[e.CallbackQuery.From.Id] = "видалити";
                        break;
                    case "редагувати":
                        await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, "Введи номер запису, який бажаєш редагувати ");
                        status[e.CallbackQuery.From.Id] = "редагувати";
                        break;
                }
                await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"ви нажали кнопку {buttonText}");
            }
            catch
            {

            }


        }

        private static async void BotOnMessageReceived(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var client = new HttpClient();
            var message = e.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            string name = $"{message.From.FirstName} {message.From.LastName}";
            Console.WriteLine($"{name} відправив повідомлення: '{message.Text}'");

            log l = new log { id = message.From.Id, info = $"{name} відправив повідомлення: '{message.Text}'" };
            var json = JsonConvert.SerializeObject(l);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = host + "/api/Test";
            var response = await client.PostAsync(url, data);
            string result = response.Content.ReadAsStringAsync().Result;
            //Console.WriteLine(result);


            try
            {
                if (status[message.From.Id] == null) status[message.From.Id] = "";
            }
            catch
            {
                status[message.From.Id] = "";
            }
            if (status[message.From.Id] == "")
            {
                switch (message.Text)
                {
                    case "/start":
                        string text =
    @"Список команд:
/start - запуск бота
/callback - вывод меню";
                        //keyboard - вывод клавиатуры";
                        await Bot.SendTextMessageAsync(message.From.Id, text);
                        break;
                    case "/callback":
                        var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("пошук гіфки по слову"),                           
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("пошук mp4 по слову"),
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("історія"),
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("редагувати"),
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("видалити"),
                        },                        
                    });
                        await Bot.SendTextMessageAsync(message.From.Id, "Вибери пункт меню", replyMarkup: inlineKeyboard);
                        break;
                    default:

                        break;
                }
            }
            else
            {
                switch (status[message.From.Id])
                {
                    case "пошук гіфки по слову":
                        string content = await client.GetStringAsync(host + "/api/Test/" + message.Text);
                        await Bot.SendTextMessageAsync(message.From.Id, content.Replace("\\n", "\n"));
                        status[message.From.Id] = "";
                        break;
                    case "пошук mp4 по слову":
                        content = await client.GetStringAsync(host + "/api/Test/mp4/" + message.Text);
                        await Bot.SendTextMessageAsync(message.From.Id, content.Replace("\\n", "\n").Replace("\\\"", "\""));
                        status[message.From.Id] = "";
                        break;
                    case "видалити":
                        await client.DeleteAsync(host + "/api/Test/" + message.Text + "/" + message.From.Id);
                        status[message.From.Id] = "";
                        break;
                    case "редагувати":
                        try
                        {
                            n[message.From.Id] = int.Parse(message.Text);
                            await Bot.SendTextMessageAsync(message.From.Id, "Введи нові дані запису: ");
                            status[message.From.Id] = "продовження редагування";
                        }
                        catch
                        {
                            await Bot.SendTextMessageAsync(message.From.Id, "Не вдалося його перетворити на ціле число, ще раз введи номер запису, який бажаєш редагувати");
                        }
                        break;
                    case "продовження редагування":
                        l = new log { id = message.From.Id, info = message.Text };
                        json = JsonConvert.SerializeObject(l);
                        data = new StringContent(json, Encoding.UTF8, "application/json");
                        url = host + "/api/Test/" + n[message.From.Id];
                        response = await client.PutAsync(url, data);
                        status[message.From.Id] = "";
                        break;
                }
            }
        }
    }
}
