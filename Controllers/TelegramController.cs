using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using AnounChatBot.Services;
using Telegram.Bot;


namespace AnounChatBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelegramController:ControllerBase
    {
        private readonly UpdateHandler _updateHandler;
        public TelegramController(UpdateHandler updateHandler)
        {
            _updateHandler = updateHandler;
        }
        [HttpPost]
        public async Task<IActionResult> post([FromBody] Update update,TelegramBotClient botClient,CancellationToken cancellationToken)
        {
            if(update==null)
            {
                return BadRequest("پیام دریافتی نامعتبر است");
            }
            await _updateHandler.HandleUpdateAsync(botClient,update,cancellationToken);
            return Ok();
        }


    }
}
