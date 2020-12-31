using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Change_Town_Names_Casing
{
    class StartUp
    {
        static void Main()
        {
            SqlConnection connection = new SqlConnection("Server=DESKTOP-HUSD35E\\SQLEXPRESS;" + "Database=MinionsDB;" + "Integrated Security = true;");

            string country = Console.ReadLine();
            List<string> countries = new List<string>();

            string updateQuery = @"UPDATE Towns
                                  SET Name = UPPER(Name)
                                  WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

            string townsQuery = @"SELECT t.Name 
                                 FROM Towns as t
                                 JOIN Countries AS c ON c.Id = t.CountryCode
                                 WHERE c.Name = @countryName";

            connection.Open();

            using (connection)
            {
                SqlCommand checkTowns = new SqlCommand(townsQuery, connection);
                checkTowns.Parameters.AddWithValue("countryName", country);

                SqlDataReader checkTownsReader = checkTowns.ExecuteReader();

                if (!checkTownsReader.HasRows)
                {
                    Console.WriteLine("No town names were affected.");
                    checkTownsReader.Close();
                }
                else
                {
                    checkTownsReader.Close();

                    SqlCommand updateTowns = new SqlCommand(updateQuery, connection);
                    updateTowns.Parameters.AddWithValue("countryName", country);

                    int counter = 0;

                    SqlDataReader reader = updateTowns.ExecuteReader();
                    reader.Close();

                    SqlDataReader townsReader = checkTowns.ExecuteReader();

                    while (townsReader.Read())
                    {
                        counter++;
                        countries.Add((string)townsReader["Name"]);
                    }

                    townsReader.Close();

                    Console.WriteLine($"{counter} town names were affected. ");
                    Console.WriteLine($"[{string.Join(", ", countries)}]");
                }
            }
        }
    }
}
