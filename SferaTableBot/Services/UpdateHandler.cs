using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TransfromService;
using File = System.IO.File;

namespace SferaTableBot.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;

    private static readonly string? LogBasePath;
    private static readonly string? BotCommonResponsePhrase;
    private static readonly string? BotEditorInfoResponsePhrase;

    private static readonly Html2JsonTransformParameters _transformParameters;

    static UpdateHandler()
    {
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Укажите путь к файлу appsettings.json
            .AddJsonFile("appsettings.json");

        var configuration = configBuilder.Build();

        // Получаем путь к лог-файлу из конфигурации
        LogBasePath = configuration["Logging:File:BasePath"];

        // Получаем стандартную фразу для ответа пользователю
        BotCommonResponsePhrase = configuration["BotConfiguration:BotCommonResponsePhrase"];

        // Получаем фразу с информацией по редактору для ответа на соответствующую команду
        BotEditorInfoResponsePhrase = configuration["BotConfiguration:BotEditorInfoResponsePhrase"];

        _transformParameters = new Html2JsonTransformParameters
        {
            TargetFormat = Html2JsonTransformParameters.ValueFormat.Html,
            NeedFormatJsonResult = false,

            NeedDoubleTransformation = Convert.ToBoolean(configuration["TransformParameters:NeedDoubleTransformation"]),
            ProcessTextColor = Convert.ToBoolean(configuration["TransformParameters:ProcessTextColor"])
        };
    }

    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
            { EditedMessage: { } message } => BotOnMessageReceived(message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
            { InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(inlineQuery, cancellationToken),
            { ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken),
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        //_logger.LogInformation("Receive message type: {MessageType}", message.Type);

        Task<Message> action;

        if (message.Type == MessageType.Document)
        {
            _logger.LogInformation(">>> От пользователя '{UserName}' получен файл: '{FileName}'",
                GetUserName(message.Chat), message.Document.FileName);
            action = ProcessReceivedFile(_botClient, message, cancellationToken);
        }
        else
        {
            _logger.LogInformation(">>> От пользователя '{UserName}' получено сообщение: '{MessageText}' с типом '{MessageType}'", GetUserName(message.Chat), message.Text, message.Type);
            action = UsageEx(_botClient, message, cancellationToken);

            //if (message.Text is not { } messageText)
            //    return;

            /*
            action = messageText.Split(' ')[0] switch
            {
                _ => UsageEx(_botClient, message, cancellationToken)
                //"/inline_keyboard" => SendInlineKeyboard(_botClient, message, cancellationToken),
                //"/keyboard" => SendReplyKeyboard(_botClient, message, cancellationToken),
                //"/remove" => RemoveKeyboard(_botClient, message, cancellationToken),
                //"/photo" => SendFile(_botClient, message, cancellationToken),
                //"/request" => RequestContactAndLocation(_botClient, message, cancellationToken),
                //"/inline_mode" => StartInlineQuery(_botClient, message, cancellationToken),
                //"/throw" => FailingHandler(_botClient, message, cancellationToken),
                //_ => Usage(_botClient, message, cancellationToken)
            }; */
        }

        var sentMessage = await action;
        _logger.LogInformation("<<< Пользователю '{UserName}' отправлено ответное сообщение: '{MessageText}' ", GetUserName(message.Chat), sentMessage.Text ?? sentMessage.Caption);

        // Send inline keyboard
        // You can process responses in BotOnCallbackQueryReceived handler
        static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendChatActionAsync(
                chatId: message.Chat.Id,
                chatAction: ChatAction.Typing,
                cancellationToken: cancellationToken);

            // Simulate longer running task
            await Task.Delay(500, cancellationToken);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    },
                });

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }

        static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                })
            {
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        static async Task<Message> RemoveKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Removing keyboard",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        static async Task<Message> SendFile(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendChatActionAsync(
                message.Chat.Id,
                ChatAction.UploadPhoto,
                cancellationToken: cancellationToken);

            const string filePath = "Files/tux.png";
            await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

            return await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: new InputFileStream(fileStream, fileName),
                caption: "Nice Picture",
                cancellationToken: cancellationToken);
        }

        static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            ReplyKeyboardMarkup RequestReplyKeyboard = new(
                new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Who or Where are you?",
                replyMarkup: RequestReplyKeyboard,
                cancellationToken: cancellationToken);
        }

        static async Task<Message> Usage(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            const string usage = "Usage:\n" +
                                 "/inline_keyboard - send inline keyboard\n" +
                                 "/keyboard    - send custom keyboard\n" +
                                 "/remove      - remove custom keyboard\n" +
                                 "/photo       - send a photo\n" +
                                 "/request     - request location or contact\n" +
                                 "/inline_mode - send keyboard with Inline Query";

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        static async Task<Message> UsageEx(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var command = message.Text?.Trim();
            var responseText = command!.Equals("/editor", StringComparison.OrdinalIgnoreCase)
                ? BotEditorInfoResponsePhrase
                : BotCommonResponsePhrase;

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: responseText ?? string.Empty,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken,
                replyToMessageId: message.MessageId);
        }

        static async Task<Message> StartInlineQuery(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = new(
                InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode"));

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Press the button to start Inline Query",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        static Task<Message> FailingHandler(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            throw new IndexOutOfRangeException();
        }
#pragma warning restore IDE0060 // Remove unused parameter
    }

    private static string GetUserName(Chat chat) =>
        chat.FirstName + (string.IsNullOrEmpty(chat.LastName)
            ? string.Empty
            : ' ' + chat.LastName) + " (@" + chat.Username + ")";

    private static async Task<Message> ProcessReceivedFile(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        string resultInfo;

        if (message.Document?.FileSize > 1048576)
            resultInfo = "Размер файла не может превышать 1 Mb";
        else
        {
            string fileData;

            // Вычитываем содержимое файла, полученного от клиента
            using var msRead = new MemoryStream();
#pragma warning disable CS8602
            var file = await botClient.GetInfoAndDownloadFileAsync(message.Document.FileId, msRead,
#pragma warning restore CS8602
                cancellationToken);

            msRead.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(msRead);
            fileData = await reader.ReadToEndAsync();

            var fileGuid =
                Guid.NewGuid(); // Уникальный идентификатор для сопоставления файлов входящих и исходящих запросов

            // Записываем в лог входящий файл
            var baseLogFileName = message.Chat.Username + "_" +
                                  Path.GetFileNameWithoutExtension(message.Document.FileName) + "_" + fileGuid;
            var inputLogFile = baseLogFileName + Path.GetExtension(message.Document.FileName);
            LogFile(msRead, inputLogFile, LogFileType.Input);

            try
            {
                var resultData = new Html2JsonTransformer().Transform(fileData, _transformParameters);

                if (string.IsNullOrEmpty(resultData?.Trim()))
                    resultInfo =
                        $"В файле '{message.Document.FileName}' отсутствуют данные для конвертации. Проверьте наличие тега <table/> в HTML разметке";
                else // Файл успешно обработан, записываем результат в файл и отправляем его клиенту
                {
                    var fileName = fileGuid + ".json";
                    await File.WriteAllTextAsync(fileName, resultData, cancellationToken);

                    using var msTransfer = new MemoryStream();
                    // Читаем содержимое файла в MemoryStream
                    await using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        await fileStream.CopyToAsync(msTransfer, cancellationToken);
                    }

                    // Получаем массив байтов из MemoryStream
                    var fileBytes = msTransfer.ToArray();

                    resultInfo = $"Файл '{message.Document.FileName}' успешно обработан";

                    // Отправляем файл, передав массив байтов
                    var sendMessageTask = await botClient.SendDocumentAsync(
                        chatId: message.Chat.Id,
                        document: new InputFileStream(new MemoryStream(fileBytes), fileName),
                        caption: resultInfo,
                        cancellationToken: cancellationToken,
                        replyToMessageId: message.MessageId);

                    // Записываем в лог исходящий файл
                    var outputLogFile = baseLogFileName + ".json";
                    LogFile(msTransfer, outputLogFile, LogFileType.Output);

                    // Удаляем файл после отправки
                    File.Delete(fileName);

                    //_logger.LogInformation("{ResultInfo}", resultInfo);

                    return sendMessageTask;
                }
            }
            catch (Exception e)
            {
                resultInfo =
                    $"При обработке файла '{message.Document.FileName}' возникла ошибка: {e.Message}";
            }
        }

        //_logger.LogInformation("{ResultInfo}", resultInfo);

        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: resultInfo,
            cancellationToken: cancellationToken,
            replyToMessageId: message.MessageId);
    }

    private static void LogFile(MemoryStream ms, string fileName, LogFileType logFileType)
    {
        try
        {
            var filePath = Path.Combine(LogBasePath, logFileType == LogFileType.Input ? "In" : "Out", fileName);

            using var fs = new FileStream(filePath, FileMode.Create);
            ms.WriteTo(fs);
        }
        catch
        {
            // ignored
        }
    }

    private enum LogFileType
    {
        Input,
        Output
    }

    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

        await _botClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);

        await _botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);
    }

    #region Inline Mode

    private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "1",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent("hello"))
        };

        await _botClient.AnswerInlineQueryAsync(
            inlineQueryId: inlineQuery.Id,
            results: results,
            cacheTime: 0,
            isPersonal: true,
            cancellationToken: cancellationToken);
    }

    private async Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);

        await _botClient.SendTextMessageAsync(
            chatId: chosenInlineResult.From.Id,
            text: $"You chose result with Id: {chosenInlineResult.ResultId}",
            cancellationToken: cancellationToken);
    }

    #endregion

#pragma warning disable IDE0060 // Remove unused parameter
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}
