using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace _06.RemoveVillain
{
    class StartUp
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine()!);
            string connectionString = "Server=.; Database=MinionsDB; Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                var sqlTransaction = sqlConnection.BeginTransaction();

                var getVillainName = new SqlCommand(@$"SELECT Name 
	                                                    FROM Villains
	                                                    WHERE Id = @villainId;", sqlConnection);
                getVillainName.Parameters.AddWithValue("@villainId", villainId);
                getVillainName.Transaction = sqlTransaction;
                var name = ExecuteScalarCommand(getVillainName, sqlTransaction);

                if (name == null)
                {
                    Console.WriteLine("No such villain was found.");
                }


                var findMinions = new SqlCommand(@$"SELECT M.Id
	                                                  FROM Minions AS M
	                                                  JOIN MinionsVillains AS MV ON MV.MinionId = M.Id
	                                                  JOIN Villains AS V ON V.Id = MV.VillainId
	                                                  WHERE V.Id = @villainId", sqlConnection);
                findMinions.Parameters.AddWithValue("@villainId",villainId);
                findMinions.Transaction = sqlTransaction;
                
                var reader = (SqlDataReader)ExecuteReaderCommand(findMinions, sqlTransaction);
                var minionIds = new List<object>();

                using (reader)
                {
                    while (reader.Read())
                    {
                        minionIds.Add(reader[0]);
                    }
                }
                

                foreach (var id in minionIds)
                {
                    var deleteFromConnectionTable = new SqlCommand(@$"DELETE 
	                                                                 FROM MinionsVillains
	                                                                 WHERE MinionId = @minionId", sqlConnection);
                    deleteFromConnectionTable.Parameters.AddWithValue("@minionId", id);
                    deleteFromConnectionTable.Transaction = sqlTransaction;
                    ExecuteNonQueryCommand(deleteFromConnectionTable, sqlTransaction);


                    var deleteMinions = new SqlCommand(@$"DELETE 
	                                                    FROM Minions
	                                                    WHERE Id = @minionId;", sqlConnection);
                    deleteMinions.Parameters.AddWithValue("@minionId", id);
                    deleteMinions.Transaction = sqlTransaction;
                    ExecuteNonQueryCommand(deleteMinions, sqlTransaction);
                }

                

                var deleteVillainFromConnectionTable = new SqlCommand(@$"DELETE 
	                                                                 FROM MinionsVillains
	                                                                 WHERE VillainId = @villainId", sqlConnection);
                deleteVillainFromConnectionTable.Parameters.AddWithValue("@villainId", villainId);
                deleteVillainFromConnectionTable.Transaction = sqlTransaction;
                ExecuteNonQueryCommand(deleteVillainFromConnectionTable, sqlTransaction);

                var deleteVillain = new SqlCommand(@$"DELETE 
	                                                    FROM Villains
	                                                    WHERE Id = @villainId;", sqlConnection);
                deleteVillain.Parameters.AddWithValue("@villainId", villainId);
                deleteVillain.Transaction = sqlTransaction;
                ExecuteNonQueryCommand(deleteVillain, sqlTransaction);

               
                Console.WriteLine($"{name} was deleted.");
                Console.WriteLine($"{minionIds.Count} minions were released.");
            }
        }

        private static object ExecuteScalarCommand(SqlCommand command, SqlTransaction transaction)
        {
            try
            {
                return command.ExecuteScalar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                transaction.Rollback();
                return null;
            }
        }

        private static object ExecuteReaderCommand(SqlCommand command, SqlTransaction transaction)
        {
            try
            {
                return command.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                transaction.Rollback();
                return null;
            }
        }

        private static void ExecuteNonQueryCommand(SqlCommand command, SqlTransaction transaction)
        {
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                transaction.Rollback();
            }
        }
    }
}
