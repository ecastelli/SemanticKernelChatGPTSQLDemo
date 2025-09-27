using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using API.Models;
using System.Data;
using System.Text.Json;
using API.Database;

namespace API.Services
{
    public class ChatHistoryService
    {
        private readonly string _ChatConnectionString;
        public ChatHistoryService(IConfiguration configuration)
        {
            _ChatConnectionString = configuration.GetConnectionString("ChatSessionManager")!;

            //make sure our local sql dbs are attached
            SQLDbConnectionManager.EnsureLocalDatabasesAttached(configuration.GetConnectionString("LocalDbMaster")!);

        }
       

        public ChatRequest InsertUpdateChatSession(ChatRequest chatRequest)
        {
            //ec store chat session in database associated with session id to allow chat session persistence/resume
            using (SqlConnection conn = new SqlConnection(_ChatConnectionString))
            {
                conn.Open();
                string? chatHistoryJson = null;
                int? chatSessionId = null;

                //this stored proc inserts a new chat ai session or updates the existing one based on id and seriliazes the chat history in order to persist the chat so follow up questions can be askd.
                SqlCommand cmd = new SqlCommand("InsertUpdateChatSession", conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ChatSessionId", chatRequest.ChatSessionId));
                cmd.Parameters.Add(new SqlParameter("@ChatHistory", chatRequest.ChatHistory.Count != 0 ? JsonSerializer.Serialize(chatRequest.ChatHistory) : null));



                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        chatHistoryJson = rdr["ChatHistory"] != DBNull.Value ? (string?)rdr["ChatHistory"] :null;
                        chatSessionId = (int)rdr["ChatSessionId"];
                    }
                }
                chatRequest.ChatSessionId = chatSessionId;

                if (chatHistoryJson != null)
                    chatRequest.ChatHistory = JsonSerializer.Deserialize<ChatHistory>(chatHistoryJson)!;


                return chatRequest;
            }


        }
    }
}