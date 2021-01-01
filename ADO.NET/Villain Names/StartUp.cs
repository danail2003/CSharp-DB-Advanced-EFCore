using System;
using System.Data.SqlClient;

namespace Villain_Names
{
    class StartUp
    {
        static void Main()
        {
            SqlConnection connection = new SqlConnection("Server=DESKTOP-HUSD35E\\SQLEXPRESS;" + "Database=MinionsDB;" + "Integrated Security=true;");

            string query= @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                            FROM Villains AS v
                            JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
                            GROUP BY v.Id, v.Name
                            HAVING COUNT(mv.VillainId) > 3
                            ORDER BY COUNT(mv.VillainId)";

            connection.Open();

            using (connection)
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Name"]} - {reader["MinionsCount"]}");
                }
            }
        }
    }
}
