using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;

namespace API.Database
{
    public static class SQLDbConnectionManager
    {

        
        //this demo uses SQLLocalDbs and we need to make sure the mdf files are attached to LocalDb.
        public static void EnsureLocalDatabasesAttached(string masterConnectionString)
        {
            var appDataPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\App_Data\\";
            if (!IsMdfFileAttached(masterConnectionString, "ChatSessionManager", appDataPath + "ChatSessionManager.mdf"))
                AttachMdf(masterConnectionString, "ChatSessionManager", appDataPath + "ChatSessionManager.mdf");

            if (!IsMdfFileAttached(masterConnectionString, "AdventureWorksSales", appDataPath + "AdventureWorksSales.mdf"))
                AttachMdf(masterConnectionString, "AdventureWorksSales", appDataPath + "AdventureWorksSales.mdf");

        }

        //check if our database mdf file is attached to local db
        public static bool IsMdfFileAttached(string masterConnectionString, string databaseName, string mdfFilePath)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(masterConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT COUNT(*) FROM sys.master_files WHERE database_id = DB_ID(@databaseName) AND physical_name = @mdfFilePath";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@databaseName", databaseName);
                        command.Parameters.AddWithValue("@mdfFilePath", mdfFilePath);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error checking MDF file attachment: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return false;
            }
        }

        //attach the database mdf file to local db
        public static void AttachMdf(string masterConnectionString, string databaseName, string mdfFilePath)
        {


            try
            {
                using (SqlConnection connection = new SqlConnection(masterConnectionString))
                {
                    connection.Open();
                    string query = $"CREATE DATABASE {databaseName} on (filename = '{mdfFilePath}') FOR ATTACH";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error checking MDF file attachment: {ex.Message}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }


        }
        


    }
}
