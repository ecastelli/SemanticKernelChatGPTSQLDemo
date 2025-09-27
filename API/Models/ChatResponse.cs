using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ChatResponse
    {
        public  int? ChatSessionId { get; set; }

        public  string? Message { get; set; }
    }
}
