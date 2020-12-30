using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;

namespace _03.MinionNames
{
    class StartUp
    {
        static void Main()
        {
            string connectionString = "Server=.; Database=MinionsDB; Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                Console.WriteLine("Enter VillainId: ");
                var villainId = int.Parse(Console.ReadLine()!);
                var sqlCommand = new SqlCommand(@$"SELECT V.Name, M.Name, M.Age
	                                                FROM Minions AS M
	                                                JOIN MinionsVillains AS MV ON MV.MinionId = M.Id
	                                                JOIN Villains AS V ON MV.VillainId = V.Id
	                                                WHERE MV.VillainId = {villainId};", sqlConnection);
                var reader = sqlCommand.ExecuteReader();
                int counter = 1;
                while (reader.Read())
                {
                    var villainName = reader[0];
                    Console.WriteLine($"Villain: {villainName}");
                    var minionName = reader[1];
                    var minionAge = reader[2];
                    Console.WriteLine($"{counter}. {minionName} {minionAge}");
                    counter++;
                }
            }
        }
    }
}
