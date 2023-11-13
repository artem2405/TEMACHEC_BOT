using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using Telegram.Bot.Types.ReplyMarkups;

class Bot
{
    private static string token { get; set; } = "6021948713:AAH7zxyV88p8VbiiU1ePr46XPM2oOa7Wwdg";
    private static TelegramBotClient? client;
    private static int check = 0;
    private static string DBpath = @"C:\Users\artem\Desktop\PROGS\TEMACHEC_BOT\DataBaseOfRappers.txt";

    static void Main()
    {
        client = new TelegramBotClient(token);
        client.StartReceiving(Update, Error);
        Console.ReadLine();
    }

    private static async Task WrapUpdate(ITelegramBotClient client, Update update, CancellationToken token)
    {
        try
        {
            await Update(client, update, token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
    {
        Thread.Sleep(1000);
        var message = update.Message;

        Console.WriteLine();
        Console.WriteLine($"От пользователя {message.Chat.Username} пришло сообщение с текстом: {message.Text}");
        Console.WriteLine();

        switch (message.Text)
        {
            case "1":
                await client.SendTextMessageAsync(message.Chat.Id, "Введи никнейм исполнителя");
                check = 1;
                break;
            case "2":
                await client.SendTextMessageAsync(message.Chat.Id, "Введи через запятую никнейм исполнителя и название трека");
                check = 2;
                break;
            case "3": // ЗАПИСЫВАЕМ НОВЫЕ ДАННЫЕ В БАЗУ ДАННЫХ
                await client.SendTextMessageAsync(message.Chat.Id, "ЗАПИСЬ НОВОГО АЛЬБОМА В БАЗУ ДАННЫХ");
                Console.Write("ВВЕДИТЕ НИКНЕЙМ ИСПОЛНИТЕЛЯ: ");
                string NickName = Console.ReadLine();
                Console.Write("ВВЕДИТЕ НАЗВАНИЕ АЛЬБОМА И НАЗВАНИЯ ТРЕКОВ ЧЕРЕЗ ЗАПЯТУЮ И ПРОБЕЛ: ");
                string newAlbum = Console.ReadLine();
                Artist art = new Artist(NickName, newAlbum);
                SaveToDB(art);
                break;
            default:
                if (message.Text != null)
                {
                    switch (check)
                    {
                        case 0:
                            await client.SendTextMessageAsync(message.Chat.Id,
                            "Привет! \nНажми кнопку СНИППЕТЫ, если хочешь получить сниппеты исполнителя " +
                            "\nНажми кнопку ОТКУДА ТРЕК, если хочешь узнать из какого альбома трек исполнителя", replyMarkup: GetButtons());
                            break;

                        case 1:
                            // поиск и проверка сниппетов на содержание и актулаьность
                            // Search_And_Check(message);
                            //for (int i = 0; i < 8; i++)
                            //{
                            //    names_of_videos[i] = elems[i].Text;
                            //    await arg1.SendTextMessageAsync(message.Chat.Id, names_of_videos[i]);
                            //}

                            break;

                        case 2:
                            break;
                    }
                }
                check = -1;
                break;
        }
        return;
    }

    private static IReplyMarkup GetButtons()
    {
        return new ReplyKeyboardMarkup
        {
            Keyboard = new List<List<KeyboardButton>>
            {
                new List<KeyboardButton>{ new KeyboardButton("СНИППЕТЫ"), new KeyboardButton("ОТКУДА ТРЕК") }
            }
        };
    }

    async static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    {
        Console.WriteLine(arg2.Message);
        return;
    }

    static string[] Search_And_Check(Message message)
    {
        IWebDriver driver = new ChromeDriver("C:\\Users\\artem\\Desktop\\PROGS\\TEMACHEC_BOT");
        string video_href = "https://www.youtube.com/results?search_query=" + message.Text + " snippet";
        driver.Url = video_href;

        ReadOnlyCollection<IWebElement> elems = driver.FindElements(By.Id("video-title"));
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine(elems[i].GetAttribute("aria-label"));
        }
        Console.WriteLine();

        int j = 0;
        string[] names_of_videos = new string[elems.Count];
        while (j < 10)
        {
            if (elems[j].Text.IndexOf("snippet")!=0 || elems[j].Text.IndexOf("Snippet")!=0)
            {
                
            }
        }

        driver.Quit();
        return names_of_videos;
    }

    static List<Artist> ReadFromDB()
    {
        string json = System.IO.File.ReadAllText(DBpath);
        if (json != "") 
        { 
            List<Artist> CurArt = JsonConvert.DeserializeObject<List<Artist>>(json);
            return CurArt;
        }
        else 
        { 
            List<Artist> CurArt = new List<Artist>();
            return CurArt;
        }
    }

    static void SaveToDB(Artist artist)
    {
        List<Artist> CurArt = ReadFromDB();
        if (CurArt == null) 
        { 
            
        }
        else CurArt.Add(artist);
        string NewCurArt = JsonConvert.SerializeObject(CurArt);
        System.IO.File.WriteAllText(DBpath, NewCurArt);
    }
}


class Artist
{
    internal string nickname { get; set; }
    internal string albums { get; set; }

    internal Artist(string NickName, string Album)
    {
        albums = Album;
        nickname = NickName;
    }
}
