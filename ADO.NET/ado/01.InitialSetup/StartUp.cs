using Microsoft.Data.SqlClient;

namespace _01.InitialSetup
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=.;Database=MinionsDB;Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            
        }
    }
}
