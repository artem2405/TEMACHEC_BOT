using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;

class Bot
{
    private static string token { get; set; } = "6021948713:AAH7zxyV88p8VbiiU1ePr46XPM2oOa7Wwdg";
    private static TelegramBotClient client;
    public static int check = 0;
    public static string DBpath = @"C:\Users\artem\Desktop\PROGS\TEMACHEC_BOT\DataBaseOfRappers";

    static void Main()
    {
        client = new TelegramBotClient(token);
        client.StartReceiving(Update,Error);
        Console.ReadLine();
    }

    public static async Task Update(ITelegramBotClient arg1, Update arg2, CancellationToken arg3)
    {
        Thread.Sleep(1000);
        var message = arg2.Message;

        Console.WriteLine();
        Console.WriteLine($"От пользователя {message.Chat.Username} пришло сообщение с текстом: {message.Text}");
        Console.WriteLine();

        if (message.Text == "1")
        {
            await arg1.SendTextMessageAsync(message.Chat.Id, "Введи никнейм исполнителя");
            check = 1;
        }
        else if (message.Text == "2")
        {
            await arg1.SendTextMessageAsync(message.Chat.Id, "Введи через запятую никнейм исполнителя и название трека");
            check = 2;
        }
        else if (message.Text == "3") // ЗАПИСЫВАЕМ НОВЫЕ ДАННЫЕ В БАЗУ ДАННЫХ
        {
            await arg1.SendTextMessageAsync(message.Chat.Id, "ЗАПИСЬ НОВЫХ ДАННЫХ");
            string NickName = "MAYOT";
            string newAlbum = "GHETTO GARDEN, A, B, C, D, E";
            Artist art = new Artist(NickName, newAlbum);
            SaveToDB(art);
        }
        else if (message.Text != null) 
        {
            if (check == 0) await arg1.SendTextMessageAsync(message.Chat.Id, "Привет! \nВведи 1, если хочешь получить сниппеты исполнителя \nВведи 2, если хочешь узнать из какого альбома трек исполнителя"); 
            
            else if (check == 1)
            {
                IWebDriver driver = new ChromeDriver();
                string video_href;
                video_href = "https://www.youtube.com/results?search_query=" + message.Text + " snippet";
                driver.Url = video_href;
                string videos = driver.FindElement(By.Id("video-title")).ToString();
                Console.WriteLine(videos);
            }
            else if (check == 2)
            {

            }
            check = -1;
        }
        return;
    }

    async static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    {
        throw new NotImplementedException();
    }

    static List<Artist> ReadFromDB()
    {
        string json = System.IO.File.ReadAllText(DBpath);
        List<Artist> CurArt = JsonConvert.DeserializeObject<List<Artist>>(json);
        return CurArt;
    }

    static void SaveToDB(Artist artist)
    {
        List<Artist> CurArt = ReadFromDB();
        CurArt.Add(artist);
        string NewCurArt = JsonConvert.SerializeObject(CurArt);
        System.IO.File.WriteAllText(DBpath, NewCurArt);
    }
}


class Artist
{
    public string nickname { get; set; }
    public string albums { get; set; }

    public Artist(string NickName, string Album)
    {
        albums = Album;
        nickname = NickName;
    }
}




