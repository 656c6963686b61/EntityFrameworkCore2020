using System;
using Microsoft.Data.SqlClient;

namespace _01.InitialSetup
{
    class StartUp
    {
        private const string dbName = "MinionsDB";
        static void Main(string[] args)
        {
            string connectionString = "Server=.; Database=master; Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                //create db
                try
                {
                    string sqlCommand = "CREATE DATABASE MinionsDB;";
                    var command = new SqlCommand(sqlCommand, sqlConnection);
                    command.ExecuteNonQuery();
                    connectionString = $"Server=.; Database={dbName}; Integrated Security = true;";
                    Console.WriteLine($"Database created successfully!");
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error while creating the database. Check your connectionString and set the Database to master.");
                    Console.WriteLine(e.Message);
                    return;
                }
            }

            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using (sqlConnection)
            {
                //create tables
                string createTables =
                    @"CREATE TABLE Countries(Id INT PRIMARY KEY IDENTITY(1, 1),Name NVARCHAR(100) NOT NULL); "
                    + @"CREATE TABLE Towns(Id INT PRIMARY KEY IDENTITY(1, 1),Name NVARCHAR(100) NOT NULL, CountryCode INT NOT NULL FOREIGN KEY REFERENCES Countries(Id));"
                    + @"CREATE TABLE Minions(Id INT PRIMARY KEY IDENTITY(1,1),Name NVARCHAR(100) NOT NULL,Age INT NULL,TownId INT NOT NULL FOREIGN KEY REFERENCES Towns(Id));"
                    + @"CREATE TABLE EvilnessFactor(Id INT PRIMARY KEY IDENTITY(1,1),Name NVARCHAR(100) NOT NULL);"
                    + @"CREATE TABLE Villains(Id INT PRIMARY KEY IDENTITY(1,1),Name NVARCHAR(100) NOT NULL,EvilnessFactorId INT NOT NULL FOREIGN KEY REFERENCES EvilnessFactor(Id));"
                    + @"CREATE TABLE MinionsVillains(MinionId INT FOREIGN KEY REFERENCES Minions(Id),VillainId INT FOREIGN KEY REFERENCES Villains(Id),PRIMARY KEY (MinionId, VillainId))";

                var sqlCommand = new SqlCommand(createTables, sqlConnection);
                var rows = sqlCommand.ExecuteNonQuery();
                Console.WriteLine("Tables created successfully!");


                //insert data
                string insertQueries =
                    @"  INSERT INTO Countries (Name)
                        VALUES ('Bulgaria'), ('USA'), ('Japan'), ('UK'), ('Germany');

                        INSERT INTO Towns (Name, CountryCode)
                        VALUES ('Sofia',1), ('New York', 2), ('Tokyo', 3), ('London',4), ('Munich',5); 

                        INSERT INTO Minions (Name, Age, TownId)
                        VALUES ('Toni', 5, 1), ('Mike', 10, 2), ('Sato', 5, 3), ('Jack', 9, 4), ('Max', 3, 5); 

                        INSERT INTO EvilnessFactor (Name)
                        VALUES ('super good'), ('good'), ('bad'), ('evil'), ('super evil');

                        INSERT INTO Villains (Name, EvilnessFactorId)
                        VALUES ('Gru',1), ('Joker', 3), ('Harley Queen', 4), ('Batman',2), ('BadGuy',5); 

                        INSERT INTO MinionsVillains(MinionId, VillainId)
                        VALUES (2,1), (1, 2), (3, 5), (5,4), (4,3); ";

                sqlCommand = new SqlCommand(insertQueries, sqlConnection);
                sqlCommand.ExecuteNonQuery();
                Console.WriteLine($"Data was inserted!");
            }
        }
    }
}
