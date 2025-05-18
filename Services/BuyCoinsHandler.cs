using Azure;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace AnounChatBot.Services
{
    public class BuyCoinsHandler//این کلاس و متدش بررسی می‌کنن کاربر روی کدوم پلن کلیک کرده و یه پیام متناسب براش می‌فرستن 
    {
        private readonly ITelegramBotClient _botClient;//اینجا یک شیء از بات تلگرام تعریف می‌کنیم تا بعداً بتونیم پیام بفرستیم. این متغیر رو فقط می‌خونیم (readonly).

        public BuyCoinsHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandlePurchaseAsync(long chatId, string messageText)
        {
            string Response;

            switch (messageText)//این قسمت بررسی می‌کنه متن پیام کاربر چی بوده (کدوم پلن رو انتخاب کرده) و بر اساس اون جواب مناسب می‌ده.
            {
                case "💵 خرید ۵ سکه":
                    Response = "شما پلن ۵ سکه رو انتخاب کردید. لطفاً منتظر لینک پرداخت باشید...";
                    break;

                case "💵 خرید ۱۰ سکه":
                    Response = "شما پلن ۱۰ سکه رو انتخاب کردید. لطفاً منتظر لینک پرداخت باشید...";
                    break;
                case "💵 خرید ۲۰ سکه":
                    Response = "شما پلن ۲۰ سکه رو انتخاب کردید. لطفاً منتظر لینک پرداخت باشید...";
                    break;

                case "⬅️ بازگشت به منو":
                    Response = "شما به منوی اصلی برگشتید";
                    var buildMainMenu = KeyboardBuilder.BuildMainMenu();
                    await _botClient.SendMessage(chatId, Response, replyMarkup: buildMainMenu);
                    return;//تو این قسمت دیگه نیازی به breakنداریم چون breakباعث میشه که کد از switchبه بعد اجرا شه ولی returnکل متد رو متوقف میکنه بنابراین نوشتن breakصرفا یک خط کد اضافی است !

                    default:
                    Response = "پلن انتخابی شما نامعتبر است.";
                    break;

               
            }

            await _botClient.SendMessage(chatId, Response);
        }
    }
}
