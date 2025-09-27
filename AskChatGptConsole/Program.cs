using Microsoft.Extensions.Configuration;

namespace AskChatGptConsole
{
    internal class Program
    {
        
        private static ManualResetEvent _exitEvent = new ManualResetEvent(false);


        static async Task Main(string[] args)
        {

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            //load our app settings to get the API key.
            IConfiguration configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .Build();

            //read api key from app settings to demonstrate basic api security.
            var apiKey = configuration["ApiKey"];
            int? chatSessionId = null;

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                Console.WriteLine("\nCtrl+C pressed. Exiting...");
                _exitEvent.Set();
            };

            Console.WriteLine("Ask a question related to sales in the Adventure Works database.");
            Console.WriteLine("For example, what was the total dollar amount of sales in 2011.");
            Console.WriteLine("Chat Session state preserved so follow up questions can be asked.");
            Console.WriteLine("Press Ctrl+C to exit.");

            while (!_exitEvent.WaitOne(0))
            {
                Console.Write("> ");
                string? userInput = Console.ReadLine();
                if (userInput != null)
                {
                    try
                    {
                        var chatResponse = await APIRequestProcessor.SendChatMessage(chatSessionId, userInput, apiKey!);
                        chatSessionId = chatResponse.ChatSessionId;
                        Console.WriteLine(chatResponse.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing chat response: " + ex.ToString());
                    }

                }
            }

            Console.WriteLine("Application terminated.");
        }
    }
}
