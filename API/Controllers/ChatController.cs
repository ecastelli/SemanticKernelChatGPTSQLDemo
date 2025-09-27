using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using API.Services;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using API.Models;
using API.Resources;


namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly ChatHistoryService _chatHistoryService;
        public ChatController(Kernel kernel, ChatHistoryService chatHistoryService)
        {
            _kernel = kernel;
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _chatHistoryService = chatHistoryService;
        }

        [HttpPost]
        public async Task<ChatResponse> Post([FromBody] ChatRequest chatRequest)
        {
           

            var chatResponse = new ChatResponse();
            
            chatRequest = _chatHistoryService.InsertUpdateChatSession(chatRequest);

            if (chatRequest.ChatHistory.Count == 0)
            {
                //our chat prompt template describes the tables and relationship and tells chat gpt that we are expecting a sql response to execute the query
                var systemPromptTemplate = EmbeddedResource.Read("ChatPromptTemplate.txt");
                var promptTemplateFactory = new KernelPromptTemplateFactory();

                string systemMessage = await promptTemplateFactory.Create(new PromptTemplateConfig(systemPromptTemplate)).RenderAsync(_kernel);

                chatRequest.ChatHistory.AddSystemMessage(systemMessage);
                
            }


            // Add user input to our chat history 
            chatRequest.ChatHistory.AddUserMessage(chatRequest.Message);
            
            //save the user message before attempting chat message
            _chatHistoryService.InsertUpdateChatSession(chatRequest);

            var openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings
            {
                //tells chat gpt to automatically invoke our qeury table function when needed
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            };

            // Get the response from chat gpt
            var result = await _chatCompletionService.GetChatMessageContentAsync(chatRequest.ChatHistory, executionSettings: openAIPromptExecutionSettings, kernel: _kernel);

            //update the chat history in the db with the repsonse
            chatRequest.ChatHistory.AddAssistantMessage(result.Content ?? string.Empty);
            _chatHistoryService.InsertUpdateChatSession(chatRequest);
            chatResponse.ChatSessionId = chatRequest.ChatSessionId;
            chatResponse.Message = result.Content;

            //return the response back
            return chatResponse;
        }
    }

    
}

