using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AskChatGptConsole
{
    internal class APIRequestProcessor
    {
        public class APIChatResponse
        {
            public int? ChatSessionId { get; set; }

            public string? Message { get; set; }
        }

        public static async Task<APIChatResponse> SendChatMessage(int? chatSessionId, string message, string apiKey)
        {


            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            var postData = new
            {
                chatSessionId,
                message
            };

            //assumes api runing at https://localhost:7255/. update if you are running the api project on a different port
            var postResult = client.PostAsJsonAsync<dynamic>("https://localhost:7255/chat", postData).Result;

            if (postResult.StatusCode != HttpStatusCode.OK)
                throw new Exception($"Unexpected API response: {postResult.ToString()}");

            var response = await postResult.Content.ReadAsStringAsync();

            var chatResponse = JsonConvert.DeserializeObject<APIChatResponse>(response);
            return chatResponse!;



        }

    }


}

