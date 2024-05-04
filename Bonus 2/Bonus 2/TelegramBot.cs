using System.Reflection.Metadata.Ecma335;

namespace SimpleTGBot;

using Bonus_2;
using System;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


public class TelegramBot
{
    // Токен TG-бота. Можно получить у @BotFather
    private const string BotToken = "6422108501:AAH0BRW5sMGoN9CaESOP7cSA1CTile8YDgw";
    
    /// <summary>
    /// Инициализирует и обеспечивает работу бота до нажатия клавиши Esc
    /// </summary>
    public async Task Run()
    {
        // Если вам нужно хранить какие-то данные во время работы бота (массив информации, логи бота,
        // историю сообщений для каждого пользователя), то это всё надо инициализировать в этом методе.
        // TODO: Инициализация необходимых полей
        
        // Инициализируем наш клиент, передавая ему токен.
        var botClient = new TelegramBotClient(BotToken);
        
        // Служебные вещи для организации правильной работы с потоками
        using CancellationTokenSource cts = new CancellationTokenSource();
        
        // Разрешённые события, которые будет получать и обрабатывать наш бот.
        // Будем получать только сообщения. При желании можно поработать с другими событиями.
        ReceiverOptions receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = new [] { UpdateType.Message }
        };

        // Привязываем все обработчики и начинаем принимать сообщения для бота
        botClient.StartReceiving(
            updateHandler: OnMessageReceived,
            pollingErrorHandler: OnErrorOccured,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        // Проверяем что токен верный и получаем информацию о боте
        var me = await botClient.GetMeAsync(cancellationToken: cts.Token);
        Console.WriteLine($"Бот @{me.Username} запущен.\nДля остановки нажмите клавишу Esc...");
        
        // Ждём, пока будет нажата клавиша Esc, тогда завершаем работу бота
        while (Console.ReadKey().Key != ConsoleKey.Escape){}

        // Отправляем запрос для остановки работы клиента.
        cts.Cancel();
    }
    
    /// <summary>
    /// Обработчик события получения сообщения.
    /// </summary>
    /// <param name="botClient">Клиент, который получил сообщение</param>
    /// <param name="update">Событие, произошедшее в чате. Новое сообщение, голос в опросе, исключение из чата и т. д.</param>
    /// <param name="cancellationToken">Служебный токен для работы с многопоточностью</param>
    async Task OnMessageReceived(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Работаем только с сообщениями. Остальные события игнорируем
        var message = update.Message;
        if (message is null)
        {
            return;
        }
        // Будем обрабатывать только текстовые сообщения.
        // При желании можно обрабатывать стикеры, фото, голосовые и т. д.
        //
        // Обратите внимание на использованную конструкцию. Она эквивалентна проверке на null, приведённой выше.
        // Подробнее об этом синтаксисе: https://medium.com/@mattkenefick/snippets-in-c-more-ways-to-check-for-null-4eb735594c09
        if (message.Text is not { } messageText)
        {
            return;
        }

        // Получаем ID чата, в которое пришло сообщение. Полезно, чтобы отличать пользователей друг от друга.
        var chatId = message.Chat.Id;
        
        // Печатаем на консоль факт получения сообщения
        Console.WriteLine($"Получено сообщение в чате {chatId}: '{messageText}'");

        // Обработка стартового сообщения
        if (message.Text.ToLower().Contains("/start"))
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Привет, " + message.Chat.FirstName + "!\nЯ помогу подобрать лучший фильм или сериал специально для тебя.",
                cancellationToken: cancellationToken);

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Фильм", "Сериал" },
            })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Что ты хочешь посмотреть?",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        // Обработка конечного сообщения
        if (message.Text.ToLower().Contains("/end"))
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Заглядывайте еще!",
                cancellationToken: cancellationToken);
            return;
        }

        // Обработка ответа "сериал"
        if (message.Text.ToLower() == "сериал")
        {
            type = "Series";
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Год выпуска", "Рейтинг на IMDb" },
                new KeyboardButton[] { "Жанр", "Кол-во эпизодов" },
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Отлично! Выбери характеристику, которую хочешь настроить.",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        // Обработка ответа "фильм"
        if (message.Text.ToLower() == "фильм")
        {
            type = "Film";
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Год выпуска", "Рейтинг на IMDb" },
                new KeyboardButton[] { "Жанр", "Продолжительность" },
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Прекрасно! Выбери характеристику, которую хочешь настроить.",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        // Обработка ответа "Год выпуска"
        if (message.Text.ToLower().Contains("год выпуска"))
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "1921-1940", "1941-1960" },
                new KeyboardButton[] { "1961-1980", "1981-2000" },
                new KeyboardButton[] { "2001-2010", "2010-2015" },
                new KeyboardButton[] { "2016-2020", "2022-2023" },
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выбери предложенный промежуток или введите конкретный год самостоятельно.",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        bool characteristic = false;
        // Обработка полученной даты
        if (message.Text.Contains("19") || message.Text.Contains("20"))
        {
            date = message.Text.Split('-').ToList();
            characteristic = true;

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Супер!",
                cancellationToken: cancellationToken);
            
        }

        // Обработка ответа "Рейтинг на IMDb"
        if (message.Text.ToLower().Contains("рейтинг на imdb"))
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "9+", "8+", "7+" },
                new KeyboardButton[] { "6+", "5+", "4+" },
                new KeyboardButton[] { "3+", "3+", "1+" },
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выбери предложенный порог рейтинга, либо введи его самостоятельно",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        // Обработка рейтинга IMDb 
        if ((new string[9] { "9+", "8+", "7+", "6+", "5+", "4+", "3+", "2+", "1+" }).Contains(message.Text))
        {
            raiting = (int)message.Text.First() - '0';
            Console.WriteLine(raiting);
            characteristic = true;

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Отличный выбор!",
                cancellationToken: cancellationToken);

        }

        // Обработка ответа после указания характеристики, если тип является фильмом
        if (characteristic && (type == "Film"))
        {
            characteristic = false;
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Подобранный фильм" },
                new KeyboardButton[] { "Год выпуска", "Рейтинг на IMDb" },
                new KeyboardButton[] { "Жанр", "Продолжительность" },
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Можешь выбрать дополнительную характеристику, изменить прошлую или нажать \"Подобранный фильм\" и увидеть результат подбора",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        // Обработка ответа после указания характеристики, если тип является сериалом
        if (characteristic && (type == "Series"))
        {
            characteristic = false;
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Подобранный сериал" },
                new KeyboardButton[] { "Год выпуска", "Рейтинг на IMDb" },
                new KeyboardButton[] { "Жанр", "Кол-во эпизодов" },
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Можешь выбрать дополнительную характеристику, изменить прошлую или нажать \"Подобранный сериал\" и увидеть результат подбора",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        // Обработка ответа "Подобранный фильм/сериал"
        if (message.Text.ToLower().Contains("подобранный фильм") || message.Text.ToLower().Contains("подобранный сериал"))
        {
            var res_film = SelectedFilm(type, date, raiting, genre, totalDuration);
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Идеальный вариант для тебя прямо сейчас: \n" + res_film,
                cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Обработчик исключений, возникших при работе бота
    /// </summary>
    /// <param name="botClient">Клиент, для которого возникло исключение</param>
    /// <param name="exception">Возникшее исключение</param>
    /// <param name="cancellationToken">Служебный токен для работы с многопоточностью</param>
    /// <returns></returns>
    Task OnErrorOccured(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // В зависимости от типа исключения печатаем различные сообщения об ошибке
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        
        // Завершаем работу
        return Task.CompletedTask;
    }

    string type = "";
    List<string> date = new List<string>();
    int raiting = -1;
    string genre = "";
    int totalDuration = -1;

    /// <summary>
    /// Обрабатывает полученные характеристики и возвращает подобранный фильм
    /// </summary>
    public static Parsing.Film SelectedFilm(string type, List<string> date, int raiting, string genre, int totalDuration)
    {
        var films = Parsing.ParsFile();

        if (type != "")
            films = films.Where(x => x.Type == type).ToList();

        if (date.Count == 1)
            films = films.Where(x => x.Date == int.Parse(date[0])).ToList();
        if (date.Count == 2)
        {
            var date1 = int.Parse(date[0]);
            var date2 = int.Parse(date[1]);
            films = films.Where(x => (x.Date >= date1) && (x.Date <= date2)).ToList();
        }

        if (raiting != -1)
            films = films.Where(x => x.Rating > raiting).ToList();

        if (genre != "")
            films = films.Where(x => x.Genre.Contains(genre)).ToList();

        if (totalDuration != -1)
            films = films.Where(x => x.TotalDuration > totalDuration).ToList();

        var cnt = films.Count;
        Random r = new Random();
        var i = r.Next(cnt);
        return films[i];
    }
}