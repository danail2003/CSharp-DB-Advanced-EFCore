using System;
using System.Data.SqlClient;

namespace Increase_Age_Stored_Procedure
{
    class StartUp
    {
        static void Main()
        {
            int id = int.Parse(Console.ReadLine());

            SqlConnection connection = new SqlConnection("Server=DESKTOP-HUSD35E\\SQLEXPRESS;" + "Database=MinionsDB;" + "Integrated Security = true;");

            connection.Open();

            using (connection)
            {
                SqlCommand increaseAge = new SqlCommand("EXEC usp_GetOlder @Id", connection);
                increaseAge.Parameters.AddWithValue("@Id", id);

                SqlDataReader increaseReader = increaseAge.ExecuteReader();

                increaseReader.Close();

                SqlCommand printMinion = new SqlCommand("SELECT Name, Age FROM Minions WHERE Id = @Id", connection);
                printMinion.Parameters.AddWithValue("@Id", id);

                SqlDataReader print = printMinion.ExecuteReader();
                print.Read();

                Console.WriteLine($"{print["Name"]} – {print["Age"]} years old");

                print.Close();
            }
        }
    }
}
