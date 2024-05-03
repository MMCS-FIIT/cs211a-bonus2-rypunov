using Bonus_2;

namespace SimpleTGBot;

public static class Program
{
    // Метод main немного видоизменился для асинхронной работы
    public static async Task Main(string[] args)
    {
        //var p = Parsing.ParsFile();
        TelegramBot telegramBot = new TelegramBot();
        await telegramBot.Run();
    }
}