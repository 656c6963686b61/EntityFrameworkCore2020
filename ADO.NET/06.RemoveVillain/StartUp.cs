using System;
using Microsoft.Data.SqlClient;

namespace _06.RemoveVillain
{
    class StartUp
    {
        static void Main(string[] args)
        {
            int villainIdToDelete = int.Parse(Console.ReadLine()!);
            string connectionString = "Server=.; Database=MinionsDB; Integrated Security = true;";
            var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();

            using (sqlConnection)
            {
                // Start a local transaction.
                var sqlTransaction = sqlConnection.BeginTransaction();

                // Enlist a command in the current transaction.
                var command = sqlConnection.CreateCommand();
                command.Transaction = sqlTransaction;

                try
                {
                    //execute command
                    command.ExecuteScalar();

                    sqlTransaction.Commit();
                    Console.WriteLine("Both records were written to database.");
                }
                catch (Exception ex)
                {
                    // Handle the exception if the transaction fails to commit.
                    Console.WriteLine(ex.Message);

                    try
                    {
                        // Attempt to roll back the transaction.
                        sqlTransaction.Rollback();
                    }
                    catch (Exception exRollback)
                    {
                        // Throws an InvalidOperationException if the connection
                        // is closed or the transaction has already been rolled
                        // back on the server.
                        Console.WriteLine(exRollback.Message);
                    }
                }
            }
        }
    }
}
