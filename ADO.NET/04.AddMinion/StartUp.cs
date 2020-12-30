using System;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace ado
{
    class StartUp
    {
        static void Main()
        {
            var minionData = Console.ReadLine().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            var villainData = Console.ReadLine().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            string minionName = minionData[1];
            int minionAge = int.Parse(minionData[2]);
            string townName = minionData[3];
            string villainName = villainData[1];

            string connectionString = "Server=.;Database=MinionsDB;Integrated Security=true";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                var sqlTransaction = sqlConnection.BeginTransaction();
                //check for town

                var townQuery = new SqlCommand($@"SELECT Id
	                                                FROM Towns
	                                                WHERE Towns.Name = @townName", sqlConnection);
                townQuery.Parameters.AddWithValue("@townName", townName);
                townQuery.Transaction = sqlTransaction;
                var townId = ExecuteScalarCommand(townQuery, sqlTransaction);

                if (townId == null)
                {
                    var addTownQuery = new SqlCommand($@"INSERT INTO Towns (Name, CountryCode) VALUES(@townName, 2)",
                        sqlConnection);
                    addTownQuery.Parameters.AddWithValue("@townName", townName);
                    addTownQuery.Transaction = sqlTransaction;
                    ExecuteNonQueryCommand(addTownQuery, sqlTransaction);
                    Console.WriteLine($"Town {townName} was added to the database.");
                }

                //check for villain
                var villainQuery = new SqlCommand($@"SELECT Id
	                                                FROM Villains
	                                                WHERE Villains.Name = @villainName", sqlConnection);
                villainQuery.Parameters.AddWithValue("@villainName", villainName);
                villainQuery.Transaction = sqlTransaction;

                //insert villain if not found
                var villainId = ExecuteScalarCommand(villainQuery, sqlTransaction);
                if (villainId == null)
                {
                    var addVillainQuery =
                        new SqlCommand($@"INSERT INTO Villains (Name, EvilnessFactorId) VALUES(@villainName, 4)",
                            sqlConnection);
                    addVillainQuery.Parameters.AddWithValue("@villainName", villainName);
                    addVillainQuery.Transaction = sqlTransaction;
                    ExecuteNonQueryCommand(addVillainQuery, sqlTransaction);
                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }

                //insert minion
                var addMinionQuery =
                    new SqlCommand($@"INSERT INTO Minions (Name, Age, TownId) VALUES(@minionName, @minionAge, 12);",
                        sqlConnection);
                addMinionQuery.Parameters.AddWithValue("@minionName", minionName);
                addMinionQuery.Parameters.AddWithValue("@minionAge", minionAge);
                addMinionQuery.Transaction = sqlTransaction;
                ExecuteNonQueryCommand(addMinionQuery, sqlTransaction);

                //minionId
                var getMinionId = new SqlCommand($@"SELECT Id
	                                                   FROM Minions
	                                                   WHERE Name = @minionName", sqlConnection);
                getMinionId.Parameters.AddWithValue("@minionName", minionName);
                getMinionId.Transaction = sqlTransaction;
                var minionId = ExecuteScalarCommand(townQuery, sqlTransaction);

                var insertRelationship =
                    new SqlCommand(
                        $@"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES(@minionId, @villainId);", sqlConnection);
                insertRelationship.Parameters.AddWithValue("@minionId", minionId);
                insertRelationship.Parameters.AddWithValue("@villainId", villainId);
                insertRelationship.Transaction = sqlTransaction;
                ExecuteNonQueryCommand(insertRelationship,sqlTransaction);

                try
                {
                    sqlTransaction.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }

                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
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