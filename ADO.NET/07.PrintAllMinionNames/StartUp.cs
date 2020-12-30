using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace _07.PrintAllMinionNames
{
    class StartUp
    {
        static void Main(string[] args)
        {
            //in the following order: first record, last record, first + 1, last - 1, first + 2, last - 2 … first + n, last - n. 
            string connectionString = "Server=.; Database = MinionsDB; Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                var minionsFromDB = new SqlCommand(@$"SELECT Name	
	                                                FROM Minions", sqlConnection);
                var reader = minionsFromDB.ExecuteReader();
                var minions = new List<object>();
                using (reader)
                {
                    while (reader.Read())
                    {
                        minions.Add(reader[0]);
                    }
                }

                var orderedMinions = new List<object>();

                for (int i = 0; i < minions.Count / 2; i++)
                {
                    orderedMinions.Add(minions[i]);
                    orderedMinions.Add(minions[minions.Count - 1 - i]);
                }

                if (orderedMinions.Count % 2 != 0)
                {
                    orderedMinions.Add(minions[minions.Count / 2]);
                }

                foreach (var minion in orderedMinions)
                {
                    Console.WriteLine(minion);
                }
            }
        }
    }
}
