using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Dynamic;

namespace API.Plugins
{
    //this plugin creates a function passed to chat gpt and chat gpt responds with the sql to executue
    public class QueryTablePlugin
    {
        private readonly string _ConnectionString;
        public QueryTablePlugin()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            _ConnectionString = configuration.GetConnectionString("AdventureWorksSales")!;
        }

       
        [KernelFunction, Description("Executes a SQL query against a database table and returns the results.")]
        public IEnumerable<dynamic> ExecutSqlStatement(string sqlQuery)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_ConnectionString))
                {
                    Console.WriteLine($"Chat GPT SQL Generated: {sqlQuery}");
                    Debug.WriteLine(sqlQuery);


                    SqlCommand selectCommand = new SqlCommand(sqlQuery, con);

                    selectCommand.Connection.Open();
                    dynamic results;

                    //execute the sql and return as dynamic list of fields/results back to chat gpt
                    results = SqlCommandReaderToDynamic(selectCommand).ToList();

                    selectCommand.Connection.Close();
                    return results;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new List<dynamic> { "Please respond indicating their query failed and try to rephrase their question and to contact support if this issue persists." };
            }
            


        }

        private IEnumerable<dynamic> SqlCommandReaderToDynamic(SqlCommand selectCommand)
        {
            using (var reader = selectCommand.ExecuteReader()) { 
                while (reader.Read())
                {
                    var expando = new ExpandoObject() as IDictionary<string, object>;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        expando.Add(reader.GetName(i), reader[i]);
                    }
                    yield return expando;
                }
            }
        }
    }
}
