using System;
using System.Data.SqlClient;

namespace Remove_Villain
{
    class StartUp
    {
        static void Main()
        {
            int villainId = int.Parse(Console.ReadLine());

            SqlConnection connection = new SqlConnection("Server=DESKTOP-HUSD35E\\SQLEXPRESS;" + "Database=MinionsDB;" + "Integrated Security=true;");

            string villainIdQuery = "SELECT Name FROM Villains WHERE Id = @villainId";
            string deleteQueryMinionsVillains = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";
            string deleteQueryVillain = "DELETE FROM Villains WHERE Id = @villainId";
            string minionsCountQuery = "SELECT COUNT(*) FROM MinionsVillains WHERE VillainId = @Id";

            connection.Open();

            using (connection)
            {
                SqlCommand idCommand = new SqlCommand(villainIdQuery, connection);
                idCommand.Parameters.AddWithValue("@villainId", villainId);

                string name = (string)idCommand.ExecuteScalar();

                if (name == null)
                {
                    Console.WriteLine("No such villain was found.");
                    return;
                }

                SqlCommand countCommand = new SqlCommand(minionsCountQuery, connection);
                countCommand.Parameters.AddWithValue("@Id", villainId);

                int minionsCount = (int)countCommand.ExecuteScalar();

                SqlCommand deleteMinionsVillains = new SqlCommand(deleteQueryMinionsVillains, connection);
                deleteMinionsVillains.Parameters.AddWithValue("@villainId", villainId);

                SqlDataReader deleteReader = deleteMinionsVillains.ExecuteReader();
                deleteReader.Close();

                SqlCommand deleteVillainCommand = new SqlCommand(deleteQueryVillain, connection);
                deleteVillainCommand.Parameters.AddWithValue("@villainId", villainId);

                SqlDataReader deleteVillain = deleteVillainCommand.ExecuteReader();
                deleteVillain.Close();

                Console.WriteLine($"{name} was deleted.");
                Console.WriteLine($"{minionsCount} minions were released.");
            }
        }
    }
}
