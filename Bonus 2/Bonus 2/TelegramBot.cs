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

        // Получаем ID чата, в которое пришло сообщение. Полезно, чтобы отличать пользователей друг от друга.
        var chatId = message.Chat.Id;

        // Будем обрабатывать только текстовые сообщения.
        // При желании можно обрабатывать стикеры, фото, голосовые и т. д.
        //
        // Обратите внимание на использованную конструкцию. Она эквивалентна проверке на null, приведённой выше.
        // Подробнее об этом синтаксисе: https://medium.com/@mattkenefick/snippets-in-c-more-ways-to-check-for-null-4eb735594c09

        if (message.Text is not { } messageText)
        {
            // Обработка стикеров
            if (message.Type == MessageType.Sticker)
                await botClient.SendStickerAsync(
                    chatId: chatId,
                    InputFile.FromString("CAACAgIAAxkBAAEFXV5mQR6RXE31a6pF0SMXfhqxWS0jAQACRhcAApdF6UvO9wLFTXTONjUE"),
                    cancellationToken: cancellationToken);

            // Обработка фотографии
            if (message.Type == MessageType.Photo)
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Мне нравятся! Давай продолжим подбор фильма.",
                    cancellationToken: cancellationToken);

            // Обработка голосового сообщения
            if (message.Type == MessageType.Voice)
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Простите, я не умею слушать голосовые сообщения.",
                    cancellationToken: cancellationToken);

            // Обработка видео
            if (message.Type == MessageType.Video)
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "К сожалению, я не могу посмотреть это видео. Давай лучше продолжим.",
                    cancellationToken: cancellationToken);

            // Обработка аудио
            if (message.Type == MessageType.Audio)
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Я еще не научился слушать аудио. Предлагаю продолжить.",
                    cancellationToken: cancellationToken);
            return;
        }

        // Печатаем на консоль факт получения сообщения
        Console.WriteLine($"Получено сообщение в чате {chatId}: '{messageText}'");

        // Обработка стартового сообщения
        if (message.Text.ToLower().Contains("/start"))
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Привет, " + message.Chat.FirstName + "!\nЯ помогу подобрать лучший фильм или сериал специально для тебя.",
                cancellationToken: cancellationToken);

            await botClient.SendStickerAsync(
                    chatId: chatId,
                    InputFile.FromString("CAACAgIAAxkBAAEFXWRmQR7cESlXabHAJr5P91l2nLmOYgACJhcAAoTx6UvUIdqM2XAHjTUE"),
                    cancellationToken: cancellationToken);

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Фильм", "Сериал" },
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
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
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);

            await botClient.SendStickerAsync(
                    chatId: chatId,
                    InputFile.FromString("CAACAgIAAxkBAAEFXWZmQR7z0T7ZTnd5hzg4GlAVzovpTQACuBcAAvpY6EscBhZhYOgEITUE"),
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
        if (message.Text.Contains("-19") || message.Text.Contains("-201") || message.Text.Contains("-202"))
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
        if ((new string[] { "9+", "8+", "7+", "6+", "5+", "4+", "3+", "2+", "1+" }).Contains(message.Text))
        {
            raiting = (int)message.Text.First() - '0';
            characteristic = true;

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Отличный выбор!",
                cancellationToken: cancellationToken);

        }

        // Обработка ответа "Жанр"
        if (message.Text.ToLower().Contains("жанр"))
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Боевик", "Приключения", "Триллер" },
                new KeyboardButton[] { "Криминал", "Научная фантастика", "Драма" },
                new KeyboardButton[] { "Комедия", "Спортивный", "Мультфильм" },
                new KeyboardButton[] { "Ужасы", "Мистика", "Романтика" },
                new KeyboardButton[] { "Исторический", "Биографический", "Семейный" },
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Какой жанр тебя интересует?",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }
        
        // Обработка жанра 
        if ((new string[] { "Боевик", "Приключения", "Триллер", "Криминал", "Научная фантастика", "Драма", "Комедия", "Спортивный",
            "Мультфильм", "Ужасы", "Мистика", "Романтика", "Исторический", "Биографический", "Семейный" }).Contains(message.Text))
        {
            genre = message.Text;
            characteristic = true;

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Хороший выбор!",
                cancellationToken: cancellationToken);

        }

        // Обработка ответов "Кол-во эпизодов"
        if (message.Text.ToLower().Contains("кол-во эпизодов"))
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Меньше 10", "10-15", "16-20" },
                new KeyboardButton[] { "21-25", "26-30", "Больше 30" },
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выбери, какое количество эпизодов тебе подходит.",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        // Обработка кол-ва эпизодов
        if ((new string[] { "Меньше 10", "10-15", "16-20", "21-25", "26-30", "Больше 30" }).Contains(message.Text))
        {
            totalDuration = message.Text.Split(new char[] { '-', ' ' }).ToList();
            characteristic = true;

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Замечательно!",
                cancellationToken: cancellationToken);
        }

        // Обработка ответов "Продолжительность"
        if (message.Text.ToLower().Contains("продолжительность"))
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Менее 60 минут", "60-90 минут"},
                new KeyboardButton[] { "90-120 минут", "120-150 минут"},
                new KeyboardButton[] { "150-180 минут", "Более 180 минут"},
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Выбери, продолжительность фильма, подходящуюю тебе.",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        // Обработка продолжительности
        if ((new string[] { "Менее 60 минут", "60-90 минут", "90-120 минут", "120-150 минут", "150-180 минут", "Более 180 минут" }).Contains(message.Text))
        {
            totalDuration = message.Text.Split(new char[] { '-', ' ' }).ToList();
            characteristic = true;

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Супер!",
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

            if (res_film.Name == "Не найдено")
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Не удалось найти фильм с такими характеристиками. Попробуй изменить их.",
                    cancellationToken: cancellationToken);


            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Идеальный вариант для тебя прямо сейчас: \n" + res_film,
                    cancellationToken: cancellationToken);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "\nМожешь нажать \"Подобранный сериал\", чтобы увидеть другой вариант, подходящий тебе." +
                          "\nТакже можешь изменить характеристики.",
                    cancellationToken: cancellationToken);
            }

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
    List<string> totalDuration = new List<string>();

    /// <summary>
    /// Обрабатывает полученные характеристики и возвращает подобранный фильм
    /// </summary>
    public static Parsing.Film SelectedFilm(string type, List<string> date, int raiting, string genre, List<string> totalDuration)
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

        switch (genre)
        {
            case "Боевик":
                genre = "Action";
                break;
            case "Приключения":
                genre = "Adventures";
                break;
            case "Триллер":
                genre = "Thriller";
                break;
            case "Криминал":
                genre = "Crime";
                break;
            case "Научная фантастика":
                genre = "Sci-Fi";
                break;
            case "Драма":
                genre = "Drama";
                break;
            case "Комедия":
                genre = "Comedy";
                break;
            case "Спортивный":
                genre = "Sport";
                break;
            case "Мультфильм":
                genre = "Animation";
                break;
            case "Ужасы":
                genre = "Horror";
                break;
            case "Мистика":
                genre = "Mystery";
                break;
            case "Романтика":
                genre = "Romance";
                break;
            case "Исторический":
                genre = "History";
                break;
            case "Биографический":
                genre = "Biography";
                break;
            case "Семейный":
                genre = "Family";
                break;
            default:
                genre = "";
                break;
        }

        if (genre != "")
            films = films.Where(x => x.Genre.Contains(genre)).ToList();

        if (totalDuration.Count > 0)
        {
            if ((totalDuration[1] == "10") || (totalDuration[1] == "60"))
                films = films.Where(x => x.TotalDuration < int.Parse(totalDuration[1])).ToList();

            else if ((totalDuration[1] == "30") || (totalDuration[1] == "180"))
                films = films.Where(x => x.TotalDuration > int.Parse(totalDuration[1])).ToList();

            else
                films = films.Where(x => (x.TotalDuration >= int.Parse(totalDuration[0])) && (x.TotalDuration <= int.Parse(totalDuration[1]))).ToList();
        }

        var cnt = films.Count;
        Random r = new Random();
        var i = r.Next(0, cnt);
        if (cnt == 0)
            return new Parsing.Film("Не найдено", "0", 0, "", "", "0", "0");
        else
            return films[i];
    }
}