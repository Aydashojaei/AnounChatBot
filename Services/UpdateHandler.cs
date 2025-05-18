using AnounChatBot.Data;
using AnounChatBot.Enums;
using AnounChatBot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata.Ecma335;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;




namespace AnounChatBot.Services
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly UserService _userService;
        private readonly IServiceScopeFactory _scopFactory;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public UpdateHandler(UserService userService, IServiceScopeFactory scopFactory, IDbContextFactory<AppDbContext> contextFactory)
        {
            _userService = userService;
            _scopFactory = scopFactory;
            _contextFactory = contextFactory;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            using (var scop = _scopFactory.CreateScope())
            {



                if (update.Type == UpdateType.Message && update.Message != null)
                {
                    Console.WriteLine($"Received message: {update.Message.Text}");
                    if (!string.IsNullOrEmpty(update.Message.Text) && update.Message.Text.Trim().ToLower() == "/start")
                    {
                        var Keyboard = KeyboardBuilder.BuildMainMenu();
                        try
                        {
                            await botClient.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: "👋 به بات من خوش آمدید! برای شروع چت ناشناس روی دکمه‌ی زیر کلیک کنید.",
                            replyMarkup: Keyboard,
                            cancellationToken: cancellationToken
                           );
                        }

                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending message: {ex.Message}");
                        }

                    }

                    if (update.Message?.Text == "🗨️ چت ناشناس")
                    {

                        long telegramId = update.Message!.Chat.Id;

                        using var context = _contextFactory.CreateDbContext();
                        var user = await context.Users.FirstOrDefaultAsync(u => u.TelegramId == update.Message.Chat.Id, cancellationToken);
                        if (user == null)
                        {
                            await botClient.SendMessage
                                (
                                chatId: user!.TelegramId,
                                text: "❌ حساب کاربری شما پیدا نشد. لطفاً مجدداً /start بزنید.",
                                cancellationToken: cancellationToken
                                );
                            return;
                        }

                        if (user.Coins < 1)
                        {
                            await botClient.SendMessage
                                (
                                chatId: user!.TelegramId,
                                  text: "❌ سکه‌های شما تمام شده. برای ادامه گفتگو لطفاً سکه بخرید.",
                                  cancellationToken: cancellationToken

                                );
                            return;
                        }

                        try
                        {

                            await _userService.AddUserToWatingList(telegramId, context);
                            await botClient.SendMessage(
                              chatId: update.Message.Chat.Id,
                              text: "✅ شما به صف انتظار منتقل شدید!",
                              cancellationToken: cancellationToken);
                            var (matchResult, partnerChatId) = await _userService.TryMatchUserAsync(context);

                            switch (matchResult)
                            {
                                case MatchResult.Matched:
                                    var buildLeaveChatMenu = KeyboardBuilder.BuildLeaveChatMenu();
                                    await botClient.SendMessage(telegramId, "✅ شما به یک چت ناشناس متصل شدید!", replyMarkup: buildLeaveChatMenu);
                                    if (partnerChatId != null)
                                    {
                                        await botClient.SendMessage(partnerChatId, "✅ شما به یک چت ناشناس متصل شدید!", replyMarkup: buildLeaveChatMenu);
                                    }

                                    user.Coins -= 1;
                                    await context.SaveChangesAsync(cancellationToken);

                                    break;

                                case MatchResult.WaitingForPartner:
                                    await botClient.SendMessage(telegramId, "⏳ هنوز کسی برای چت ناشناس وجود نداره، به محض ورود متصل می‌شی.");
                                    break;

                                case MatchResult.Error:
                                    await botClient.SendMessage(telegramId, "❌ خطایی در فرآیند اتصال رخ داد.");
                                    break;
                            }


                        }


                        catch (Exception ex)

                        {
                            Console.WriteLine($"خطا در اضافه کردن به صف انتظار: {ex.Message}");
                            await botClient.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: "❌ مشکلی پیش آمده است.",
                            cancellationToken: cancellationToken
                        );



                        }

                    }








                }

                try
                {
                    if (update.Message!.Text == "🚪 خروج از چت")
                    {
                        using var context = _contextFactory.CreateDbContext();
                        long telegramId = update.Message.Chat.Id;
                        var partnerId = await _userService.LeaveChat(telegramId, context);
                        var buildmainmenue = KeyboardBuilder.BuildMainMenu();

                        await botClient.SendMessage(
                            chatId: telegramId,
                            text: "شما از چت خارج شدید! برای شروع چت جدید، مجدداً روی 'چت ناشناس' کلیک کنید.",
                            replyMarkup: buildmainmenue,
                            cancellationToken: cancellationToken);


                        if (partnerId.HasValue)
                        {
                            await botClient.SendMessage(
                            chatId: partnerId.Value,
                            text: "طرف مقابل از چت خارج شد. برای شروع چت جدید، مجدداً روی 'چت ناشناس' کلیک کنید.",
                            replyMarkup: buildmainmenue,
                            cancellationToken: cancellationToken
                            );
                        }


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"مشکلی پیش آمده است: {ex.Message}");
                    await botClient.SendMessage(
                    chatId: update.Message!.Chat.Id,
                    text: "❌ مشکلی پیش آمده است.",
                    cancellationToken: cancellationToken);
                }

                try
                {
                    if (update.Message.Text == "💰 خرید سکه")
                    {

                        long telegramId = update.Message.Chat.Id;
                        var getCoinsPlan = KeyboardBuilder.GetCoinsPlan();
                        await botClient.SendMessage(
                            chatId: telegramId,
                            text: "📦 لطفاً یکی از پلن‌های زیر رو انتخاب کن:\n\n" +
          "1️⃣ ۵ سکه - ۱۰ هزار تومان\n" +
          "2️⃣ ۱۰ سکه - ۱۸ هزار تومان\n" +
          "3️⃣ ۲۰ سکه - ۳۲ هزار تومان",
                            replyMarkup: getCoinsPlan
                            );
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"مشکلی پیش امده است {ex.Message}");
                    await botClient.SendMessage(
                        chatId: update.Message.Chat.Id,
                        text: "❌ مشکلی پیش آمده است.",
                        cancellationToken: cancellationToken
                        );
                }
                try
                {
                    var buyCoinsHandler = new BuyCoinsHandler(botClient);
                    if (update.Message.Text == "💵 خرید ۵ سکه" || update.Message.Text == "💵 خرید ۱۰ سکه" || update.Message.Text == "💵 خرید ۲۰ سکه" || update.Message.Text == "⬅️ بازگشت به منو")
                    {
                        await buyCoinsHandler.HandlePurchaseAsync(update.Message.Chat.Id, update.Message.Text);
                        return;
                    }
                }
                catch
                {
                    await botClient.SendMessage(
                        chatId: update.Message.Chat.Id,
                        text: "❌ مشکلی پیش آمده است.",
                        cancellationToken: cancellationToken

                        );
                }





                await ForwardMessageToPartnerAsync(update, botClient, cancellationToken);


            }


        }


        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"خطایی رخ داد: {exception.Message}");
            return Task.CompletedTask;
        }


        public async Task ForwardMessageToPartnerAsync(Update update, ITelegramBotClient botClient, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();
            var SenderId = update.Message!.Chat.Id;//ایدی تلگرام گسی که پیام فرستادرو میگیریم و ذخیره میکنیم 
            var SenderUser = await context.Users.FirstOrDefaultAsync(u => u.TelegramId == SenderId);//اینجا میره دنباله کاربری میگرده که تلگرام ایدیش برابر همون سندر ایدی که توش ذخیره کردیم.

            if (SenderUser?.PartnerId != null)
            {
                var partnerId = SenderUser.PartnerId.Value;
                var message = update.Message;

                //s/*tring[] blockedText = { "/start", "🗨️ چت ناشناس", "🚪 خروج از چت" };*/

                if (!string.IsNullOrEmpty(message.Text))
                {
                    await botClient.SendMessage(partnerId, message.Text, cancellationToken: cancellationToken);
                }

                else
                {
                    // اگر پیام عکس، ویدیو، وویس و... بود، به این صورت فقط پیام‌ها رو فوروارد می‌کنیم
                    if (message.Photo != null)
                    {
                        var photo = message.Photo.Last();
                        await botClient.SendPhoto(partnerId, photo.FileId, caption: message.Caption, cancellationToken: cancellationToken);
                    }
                    else if (message.Video != null)
                    {
                        await botClient.SendVideo(partnerId, message.Video.FileId, caption: message.Caption, cancellationToken: cancellationToken);
                    }
                    else if (message.Voice != null)
                    {
                        await botClient.SendVoice(partnerId, message.Voice.FileId, caption: message.Caption, cancellationToken: cancellationToken);
                    }
                    else if (message.Document != null)
                    {
                        await botClient.SendDocument(partnerId, message.Document.FileId, caption: message.Caption, cancellationToken: cancellationToken);
                    }
                    else if (message.Sticker != null)
                    {
                        await botClient.SendSticker(partnerId, message.Sticker.FileId, cancellationToken: cancellationToken);
                    }
                    else if (message.Location != null)
                    {
                        await botClient.SendLocation(partnerId, message.Location.Latitude, message.Location.Longitude, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        // در صورتی که هیچ نوع پیام پشتیبانی نشه
                        await botClient.SendMessage(SenderId, "❌ نوع پیام پشتیبانی نمی‌شود.", cancellationToken: cancellationToken);
                    }
                }

                return;
            }
        }



    }
}
