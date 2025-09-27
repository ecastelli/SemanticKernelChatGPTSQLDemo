using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ChatRequest
    {

        public int? ChatSessionId { get; set; }

        [Required]
        public required string Message { get; set; }

        public ChatHistory ChatHistory { get; set; } = new();
    }
}
