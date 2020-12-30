using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace _05.ChangeTownNamesCasing
{
    class StartUp
    {
        static void Main(string[] args)
        {
            var countryName = Console.ReadLine();
            string connectionString = "Server=.; Database=MinionsDB; Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                var townsCount = new SqlCommand(@$"SELECT COUNT(*)
	                                                FROM Towns T
	                                                JOIN Countries C ON C.Id = T.CountryCode
	                                                WHERE C.Name = @countryName", sqlConnection);
                townsCount.Parameters.AddWithValue("@countryName", countryName);
                var count = townsCount.ExecuteScalar();

                var townNames = new SqlCommand(@$"SELECT T.Name
	                                                FROM Towns T
	                                                JOIN Countries C ON C.Id = T.CountryCode
	                                                WHERE C.Name = @countryName", sqlConnection);
                townNames.Parameters.AddWithValue("@countryName", countryName);

                var reader = townNames.ExecuteReader();
                var towns = new List<object>();
                using (reader)
                {
                    while (reader.Read())
                    {
                        towns.Add(reader[0]);
                    }
                }
               
                
                if (towns.Count > 0)
                {
                    var updateCommand = new SqlCommand($@"UPDATE T
                                                    SET Name = UPPER(T.Name)
                                                    FROM Towns T
                                                    JOIN Countries C ON t.CountryCode = C.Id
                                                    WHERE C.Name = @countryName", sqlConnection);
                    updateCommand.Parameters.AddWithValue("@countryName", countryName);
                    updateCommand.ExecuteNonQuery();
                    Console.WriteLine($"{count} town names were affected.");
                    Console.WriteLine($"[{string.Join(", ", towns)}]");
                }
                else
                {
                    Console.WriteLine("No town names were affected.");
                }
            }
        }
    }
}
