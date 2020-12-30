using System;
using Microsoft.Data.SqlClient;

namespace _02.VillainNames
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=.; Database=MinionsDB; Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                var sqlCommand = new SqlCommand(@"SELECT v.Name, COUNT(*) as MinionsCount
	                                                    FROM Villains AS v
	                                                    JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
	                                                    GROUP BY v.Name
                                                        ORDER BY MinionsCount DESC", sqlConnection);

                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    var villainName = reader[0];
                    var minionCount = reader[1];
                    Console.WriteLine($"{villainName} has {minionCount} minion.");
                }
            }
        }
    }
}
