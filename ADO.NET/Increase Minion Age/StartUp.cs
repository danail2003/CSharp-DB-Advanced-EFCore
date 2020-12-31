using System;
using System.Linq;
using System.Data.SqlClient;

namespace Increase_Minion_Age
{
    class StartUp
    {
        static void Main()
        {
            int[] ids = Console.ReadLine().Split().Select(int.Parse).ToArray();

            SqlConnection connection = new SqlConnection("Server=DESKTOP-HUSD35E\\SQLEXPRESS;" + "Database=MinionsDB;" + "Integrated Security = true;");

            connection.Open();

            using (connection)
            {
                SqlCommand increaseAge = new SqlCommand(@"UPDATE Minions
                                                        SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                                        WHERE Id = @Id", connection);

                for (int i = 0; i < ids.Length; i++)
                {
                    increaseAge.Parameters.AddWithValue("@Id", ids[i]);

                    SqlDataReader increaseReader = increaseAge.ExecuteReader();

                    increaseReader.Close();
                    increaseAge.Parameters.Clear();
                }

                SqlCommand printNamesCommand = new SqlCommand("SELECT Name, Age FROM Minions", connection);

                SqlDataReader print = printNamesCommand.ExecuteReader();

                while (print.Read())
                {
                    Console.WriteLine($"{print["Name"]} {print["Age"]}");
                }
            }
        }
    }
}
