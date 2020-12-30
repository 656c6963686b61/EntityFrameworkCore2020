using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace _08.IncreaseMinionAge
{
    class StartUp
    {
        static void Main(string[] args)
        {
            var minionIds = Console.ReadLine().Split(" ").Select(int.Parse).ToList();
            string connectionString = "Server=.; Database = MinionsDB; Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                var minionData = new SqlCommand($@"SELECT Name, Id
	                                                  FROM Minions", sqlConnection);
                var reader = minionData.ExecuteReader();
                var minionsToUpdate = new List<object>();

                using (reader)
                {
                    while (reader.Read())
                    {
                        if (minionIds.Contains((int)reader[1]))
                        {
                            minionsToUpdate.Add(reader[0]);
                        }
                    }
                }

                foreach (var minion in minionsToUpdate)
                {
                    var updateName = new SqlCommand(@$"UPDATE Minions
	                                                     SET Name = UPPER(LEFT(Name,1)) + RIGHT(NAME, LEN(NAME) - 1)
	                                                     WHERE Name = @minionName", sqlConnection);
                    updateName.Parameters.AddWithValue("@minionName", minion);
                    updateName.ExecuteNonQuery();

                    var updateAge = new SqlCommand(@$"UPDATE Minions
	                                                     SET Age += 1
	                                                     WHERE Name = @minionName", sqlConnection);
                    updateAge.Parameters.AddWithValue("@minionName", minion);
                    updateAge.ExecuteNonQuery();
                }

                var finalDataToPrint = new SqlCommand($@"SELECT Name, Age
	                                                  FROM Minions", sqlConnection);
                var reader2 = finalDataToPrint.ExecuteReader();
                using (reader2)
                {
                    while (reader2.Read())
                    {
                        Console.WriteLine($"{reader2[0]} {reader2[1]}");
                    }
                }
            }
        }
    }
}
