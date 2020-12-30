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
                //check for town
                var townQuery = new SqlCommand($@"SELECT *
	                                                FROM Towns
	                                                WHERE Towns.Name = @townName", sqlConnection);
                townQuery.Parameters.AddWithValue("@townName", townName);
                var townId = townQuery.ExecuteScalar();

                if (townId == null)
                {
                    var addTownQuery = new SqlCommand($@"INSERT INTO Towns (Name, CountryCode) VALUES(@townName, 2)",
                        sqlConnection);
                    addTownQuery.Parameters.AddWithValue("@townName", townName);
                    addTownQuery.ExecuteNonQuery();
                    Console.WriteLine($"Town {townName} was added to the database.");
                }

                //check for villain
                var villainQuery = new SqlCommand($@"SELECT *
	                                                FROM Villains
	                                                WHERE Villains.Name = @villainName", sqlConnection);
                villainQuery.Parameters.AddWithValue("@villainName", villainName);

                //insert villain if not found
                var villainId = villainQuery.ExecuteScalar();
                if (villainId == null)
                {
                    var addVillainQuery =
                        new SqlCommand($@"INSERT INTO Villains (Name, EvilnessFactorId) VALUES(@villainName, 4)",
                            sqlConnection);
                    addVillainQuery.Parameters.AddWithValue("@villainName", villainName);
                    addVillainQuery.ExecuteNonQuery();
                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }

                //insert minion
                var addMinionQuery =
                    new SqlCommand($@"INSERT INTO Minions (Name, Age, TownId) VALUES('@minionName', @minionAge, @townId);",
                        sqlConnection);
                addMinionQuery.Parameters.AddWithValue("@minionName", minionName);
                addMinionQuery.Parameters.AddWithValue("@minionAge", minionAge);
                addMinionQuery.Parameters.AddWithValue("@townId", townId);
                addMinionQuery.ExecuteNonQuery();

                //minionId
                var getMinionId = new SqlCommand($@"SELECT Id
	                                                   FROM Minions
	                                                   WHERE Name = '@minionName'", sqlConnection);
                getMinionId.Parameters.AddWithValue("@minionName", minionName);
                var minionId = getMinionId.ExecuteScalar();

                var insertRelationship =
                    new SqlCommand(
                        $@"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES(@minionId, @villainId);");
                insertRelationship.Parameters.AddWithValue("@minionId", minionId);
                insertRelationship.Parameters.AddWithValue("@villainId", villainId);
                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }
        }
    }
}