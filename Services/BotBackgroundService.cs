using System.Net;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AnounChatBot.Services
{
    public class BotBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopFactory;
        private readonly ITelegramBotClient _botClient;
        private readonly IConfiguration _configuration;
        

        public BotBackgroundService(IConfiguration configuration,IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopFactory = scopeFactory;
            
            var token = _configuration["TelegramBot:Token"];
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("❌ Bot token is missing in configuration!");
            var proxyHost = _configuration["TelegramBot:ProxySettings:Host"];
            var portStr = _configuration["TelegramBot:ProxySettings:Port"];
            if (string.IsNullOrWhiteSpace(proxyHost)|| !int.TryParse(portStr, out var ProxyPort))
                throw new Exception("❌ Proxy port is missing or invalid in configuration!");

            var Proxy = new WebProxy(proxyHost, ProxyPort);

            var HttpClientHandler = new HttpClientHandler
            {
                Proxy = Proxy,
                UseProxy = true
            };
            var httpClient = new HttpClient(HttpClientHandler, disposeHandler: true);
            _botClient = new TelegramBotClient(token, httpClient);

            

               

        }

        protected override async Task  ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Bot is starting...");

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
            };

            _botClient.StartReceiving(

                async (botClient, update, token) =>
                {
                    using var scope = _scopFactory.CreateScope();
                    var updateHandler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();
                    await updateHandler.HandleUpdateAsync(botClient, update, token);
                },
        async (botClient, exception, token) =>
        {
            Console.WriteLine($"Polling error: {exception.Message}");
            await Task.CompletedTask;
        },
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken

                );
            var me = await _botClient.GetMe(stoppingToken);
            Console.WriteLine($"🤖 Bot started:{me.Username}");

            await Task.Delay(-1, stoppingToken);





        }
    }
}





