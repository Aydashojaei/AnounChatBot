using Microsoft.Identity.Client;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;//کتابخانه ای است ک اجازه میده دکمه توی تلگرام درست کنیم 



namespace AnounChatBot.Services
{
    public static class KeyboardBuilder
    {
        public static ReplyKeyboardMarkup BuildMainMenu()
        {
            return new ReplyKeyboardMarkup(new[]

            {
                new KeyboardButton[] { "🗨️ چت ناشناس", "👤 پروفایل" },
                new KeyboardButton[] { "🎁 دعوت دوستان", "💰 خرید سکه" }
            }

            )
            {

                ResizeKeyboard = true,
                OneTimeKeyboard = false

            };


        }
        public static ReplyKeyboardMarkup BuildLeaveChatMenu()
        {
            return new ReplyKeyboardMarkup(new[]
            {



                new KeyboardButton[]{ "🚪 خروج از چت" }



            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };
        }

        public static ReplyKeyboardMarkup GetCoinsPlan()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[]{ "💵 خرید ۵ سکه" },
                new KeyboardButton[]{ "💵 خرید ۱۰ سکه" },
                new KeyboardButton[]{ "💵 خرید ۲۰ سکه" },
                new KeyboardButton[]{ "⬅️ بازگشت به منو" }

            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };


        }

        

       

    }
}
