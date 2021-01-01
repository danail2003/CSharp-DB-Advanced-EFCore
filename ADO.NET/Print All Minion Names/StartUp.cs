using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Print_All_Minion_Names
{
    class StartUp
    {
        static void Main()
        {
            List<string> names = new List<string>();
            string query = "SELECT Name FROM Minions";

            SqlConnection connection = new SqlConnection("Server=DESKTOP-HUSD35E\\SQLEXPRESS;" + "Database=MinionsDB;" + "Integrated Security = true;");

            connection.Open();

            using (connection)
            {
                SqlCommand command = new SqlCommand(query, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    names.Add((string)reader["Name"]);
                }

                reader.Close();

                int counter = 1;

                for (int i = 0; i < names.Count / 2; i++)
                {
                    Console.WriteLine(names[i]);
                    Console.WriteLine(names[names.Count - counter]);
                    counter++;
                }
            }
        }
    }
}
