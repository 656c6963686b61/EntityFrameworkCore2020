using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace _09.IncreaseAgeStoredProcedure
{
    class StartUp
    {
        static void Main(string[] args)
        {
            var minionId = int.Parse(Console.ReadLine()!);
            string connectionString = "Server=.; Database = MinionsDB; Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                var parameter = new SqlParameter("@MinionId", SqlDbType.Int);
                parameter.Value = minionId;
                var sqlCommand = new SqlCommand("usp_GetOlder", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Parameters = { parameter }
                };
                sqlCommand.ExecuteNonQuery();

                var minion = new SqlCommand($@"SELECT Name, Age
	                                                  FROM Minions
                                                      WHERE Id = @minionId", sqlConnection);
                minion.Parameters.AddWithValue("@minionId", minionId);
                var reader = minion.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader[0]} - {reader[1]} years old.");
                    }
                   
                }
            }

        }
    }
}
