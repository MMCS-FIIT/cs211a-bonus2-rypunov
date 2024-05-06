using Bonus_2;
using System.Formats.Tar;

namespace SimpleTGBot;

public static class Program
{
    // Метод main немного видоизменился для асинхронной работы
    public static async Task Main(string[] args)
    {
        TelegramBot telegramBot = new TelegramBot();
        await telegramBot.Run();
        //var c = Parsing.ParsFile().Where(x => ((x.TotalDuration <= 15) && (x.TotalDuration >= 10)) && (x.Type == "Series")).Count();
        //Console.WriteLine(c);
    }
}