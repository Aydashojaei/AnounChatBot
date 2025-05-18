using AnounChatBot.Data;
using AnounChatBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.FileSystemGlobbing;
using Telegram.Bot;
using Telegram.Bot.Types;
using AnounChatBot.Enums;
using Microsoft.OpenApi.Writers;

namespace AnounChatBot.Services
{
    public class UserService
    {
        private readonly IDbContextFactory<AppDbContext> _ContextFactory;
        


        public UserService(IDbContextFactory<AppDbContext> ContextFactory)
        {
            _ContextFactory = ContextFactory;
        }




        public async Task<(AnounChatBot.Models.User? user, bool IsNew)> GetORCreatUserAsync(long TelegramId)

        {
            try
            {
                using var _context = _ContextFactory.CreateDbContext();
                Console.WriteLine("در حال تلاش برای اتصال به پایگاه داده...");
                
                var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == TelegramId);
                

                if (user == null)  // اگر کاربر جدید بود
                {
                    user =new AnounChatBot.Models.User
                    {
                        TelegramId = TelegramId,
                        Coins = 5
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"کاربر جدید با TelegramId {TelegramId} اضافه شد.");
                    return (user, true);  // IsNew = true

                }

                else
                {
                    Console.WriteLine($"کاربر با TelegramId {TelegramId} یافت شد.");
                    return (user, false);  // IsNew = false
                }

                // اگر کاربر پیدا شد، یا جدید نبود، به هر حال باید اطلاعات کاربر رو برگردونیم
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطا در اتصال به پایگاه داده: {ex.Message}");
                return (null, false);
            }

        }

        public async Task AddUserToWatingList(long telegramId,AppDbContext context)//قراره زمانی اجرا بشه که کاربر روی شروع چت ناشناس بزنه
        {
            try
            {

                var user= await context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
                if(user==null)
                {
                    user = new AnounChatBot.Models.User 
                    {
                        TelegramId = telegramId,
                        Coins = 5,
                        IsWating = true
                    };
                    context.Users.Add(user);
                }
                else
                {
                    user.IsWating = true;
                }
                await context.SaveChangesAsync();
                Console.WriteLine($"کاربر{telegramId}به صف انتظار اضافه شد");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"خطا:{ex.Message}");
            }
        }
        public async Task<(MatchResult Result, long? PartnerChatId)> TryMatchUserAsync(AppDbContext context)

        {
            try
            {
                var WaitingUsers = await context.Users.Where(u => u.IsWating == true).ToListAsync();


                if (WaitingUsers.Count >= 2)
                {
                    
                    var user1 = WaitingUsers[0];
                    var user2 = WaitingUsers[1];

                    user1.IsWating = false;
                    user2.IsWating = false;


                    user1.PartnerId = user2.TelegramId;
                    user2.PartnerId = user1.TelegramId;

                    await context.SaveChangesAsync();
                    return (MatchResult.Matched, user1.TelegramId);



                }
                else if (WaitingUsers.Count == 1)
                {
                    return (MatchResult.WaitingForPartner, null)
 ;               }

                return (MatchResult.Error, null);
            }
            catch
            {


                return (MatchResult.Error, null);
            }
        }
        public async Task<long?> LeaveChat(long telegramId, AppDbContext context)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId );

           


            var partner = await context.Users.FirstOrDefaultAsync(u => u.TelegramId == user!.PartnerId);

            if (partner != null)
            {
                
                partner.PartnerId = null;
            }

            user!.PartnerId = null;
            await context.SaveChangesAsync();


            return partner?.TelegramId;


        }


    }
}