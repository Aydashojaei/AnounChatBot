using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;


namespace AnounChatBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public long TelegramId { get; set; }
        public int Coins { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public long? PartnerId { get; set; }
        public bool IsWating { get; set; }
        

    }
}
