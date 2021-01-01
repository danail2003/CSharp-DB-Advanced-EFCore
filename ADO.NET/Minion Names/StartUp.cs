using System;
using System.Data.SqlClient;

namespace Minion_Names
{
    class StartUp
    {
        static void Main()
        {
            int villainId = int.Parse(Console.ReadLine());

            SqlConnection connection = new SqlConnection("Server=DESKTOP-HUSD35E\\SQLEXPRESS;" + "Database=MinionsDB;" + "Integrated Security = true;");

            string firstQuery = @"SELECT Name FROM Villains WHERE Id = @Id";
            string secondQuery = @"SELECT ROW_NUMBER() OVER(ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";
                            
            connection.Open();

            using (connection)
            {
                SqlCommand command = new SqlCommand(firstQuery, connection);
                command.Parameters.AddWithValue("@Id", villainId);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();

                    Console.WriteLine($"Villain: {reader["Name"]}");
                }
                else
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                    return;
                }

                reader.Close();

                command = new SqlCommand(secondQuery, connection);
                command.Parameters.AddWithValue("@Id", villainId);

                SqlDataReader sqlReader = command.ExecuteReader();

                if (!sqlReader.HasRows)
                {
                    Console.WriteLine("(no minions)");
                }
                else
                {
                    int minions = 1;

                    while (sqlReader.Read())
                    {
                        Console.WriteLine($"{minions}. {sqlReader["Name"]} {sqlReader["Age"]}");
                        minions++;
                    }
                }
            }
        }
    }
}
