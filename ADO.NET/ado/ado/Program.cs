using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;

namespace ado
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=.;Database=Minions;Integrated Security=true";
            var sqlConnection = new SqlConnection(connectionString);
            using (sqlConnection)
            {
                sqlConnection.Open();
                var minionData = Console.ReadLine().Split(new [] {',', ' '}, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var villainData = Console.ReadLine().Split(new [] {',', ' '}, StringSplitOptions.RemoveEmptyEntries).ToArray();
                string minionName = minionData[1];
                int minionAge = int.Parse(minionData[2]);
                string townName = minionData[3];
                string villainName = villainData[1];

                string commandText =
                    "DECLARE @message VARCHAR(200);" + "DECLARE @villainId INT;" + "DECLARE @minionId INT;" +
                    "BEGIN TRANSACTION;" +
                    $"IF((SELECT COUNT(*) FROM Towns WHERE Name LIKE '{townName}') = 0) " +
                    "BEGIN " +
                    $"INSERT INTO Towns(Name, CountryCode) VALUES('{townName}', 1) " +
                    $"SET @message = 'Town {townName} was added to the database.' + CHAR(13) + CHAR(10) " +
                    "END " +
                    $"IF((SELECT COUNT(*) FROM Villains WHERE Name LIKE '{villainName}') = 0) " +
                    "BEGIN " +
                    $"INSERT INTO Villains(Name, EvilnessFactorId) VALUES('{villainName}', 4) " +
                    $"SET @message += 'Villain {villainName} was added to the database.' + CHAR(13) + CHAR(10) " +
                    "END " +
                    $"SET @villainId = (SELECT Id FROM Villains WHERE Name LIKE '{villainName}') " +
                    $"IF((SELECT COUNT(*) FROM Minions WHERE Name LIKE '{minionName}') = 0) " +
                    "BEGIN " +
                    "DECLARE @TownId INT; " +
                    $"SET @TownId = (SELECT Id FROM Towns WHERE Name = '{townName}'); " +
                    $"INSERT INTO Minions(Name, Age, TownId) VALUES('{minionName}', {minionAge}, @TownId) " +
                    $"SET @minionId = (SELECT Id FROM Minions WHERE Name LIKE '{minionName}') " +
                    "INSERT INTO MinionsVillains(MinionId, VillainId) VALUES(@minionId, @villainId) " +
                    $"SET @message += 'Successfully added {minionName} to be minion of {villainName}.' " +
                    "END " + "SELECT @message AS Message " +
                    "COMMIT;";
                var sqlCommand = new SqlCommand(commandText, sqlConnection);
                var message = sqlCommand.ExecuteScalar();
                Console.WriteLine(message);
            }
        }
    }
}
