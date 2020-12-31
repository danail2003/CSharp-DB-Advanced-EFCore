using System;
using System.Data.SqlClient;

namespace Add_Minion
{
    class StartUp
    {
        static void Main()
        {
            SqlConnection connection = new SqlConnection("Server=DESKTOP-HUSD35E\\SQLEXPRESS;" + "Database=MinionsDB;" + "Integrated Security = true;");

            string[] input = Console.ReadLine().Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
            string[] secondInput = Console.ReadLine().Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);

            string villainIdQuery = "SELECT Id FROM Villains WHERE Name = @Name";
            string minionIdQuery = "SELECT Id FROM Minions WHERE Name = @Name";
            string insertVillainQuery = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
            string idTownQuery = "SELECT Id FROM Towns WHERE Name = @townName";
            string insertTownQuery = "INSERT INTO Towns (Name) VALUES (@townName)";
            string insertMinionQuery = "INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)";
            string insertMinionVillainQuery = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";

            connection.Open();

            using (connection)
            {
                object townId = null;

                SqlCommand command = new SqlCommand(idTownQuery, connection);
                command.Parameters.AddWithValue("townName", input[3]);

                SqlDataReader reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    reader.Close();

                    SqlCommand insertTown = new SqlCommand(insertTownQuery, connection);
                    insertTown.Parameters.AddWithValue("townName", input[3]);

                    SqlDataReader townReader = insertTown.ExecuteReader();
                    townReader.Close();

                    Console.WriteLine($"Town {input[3]} was added to the database.");

                    SqlCommand selectTownId = new SqlCommand(idTownQuery, connection);
                    selectTownId.Parameters.AddWithValue("townName", input[3]);

                    SqlDataReader idTown = selectTownId.ExecuteReader();

                    if (idTown.Read())
                    {
                        townId = idTown["Id"];
                    }

                    idTown.Close();
                }
                else
                {
                    reader.Close();
                }

                SqlCommand villainId = new SqlCommand(villainIdQuery, connection);
                villainId.Parameters.AddWithValue("Name", secondInput[1]);

                SqlDataReader villainIdReader = villainId.ExecuteReader();
                object resultVillain = null;

                if (villainIdReader.Read())
                {
                    resultVillain = villainIdReader["Id"];
                    villainIdReader.Close();
                }
                else
                {
                    villainIdReader.Close();

                    SqlCommand insertVillain = new SqlCommand(insertVillainQuery, connection);
                    insertVillain.Parameters.AddWithValue("villainName", secondInput[1]);

                    SqlDataReader insertVillainReader = insertVillain.ExecuteReader();                  

                    Console.WriteLine($"Villain {secondInput[1]} was added to the database.");

                    insertVillainReader.Close();

                    SqlCommand sqlCommand = new SqlCommand(villainIdQuery, connection);
                    sqlCommand.Parameters.AddWithValue("Name", secondInput[1]);

                    SqlDataReader sqlData = sqlCommand.ExecuteReader();

                    if (sqlData.Read())
                    {
                        resultVillain = sqlData["Id"];
                    }

                    sqlData.Close();
                }


                SqlCommand minionId = new SqlCommand(minionIdQuery, connection);
                minionId.Parameters.AddWithValue("Name", input[1]);

                SqlDataReader minionReader = minionId.ExecuteReader();
                object resultMinion = null;

                if (minionReader.Read())
                {
                    resultMinion = minionReader["Id"];
                    minionReader.Close();
                }
                else
                {
                    minionReader.Close();                    

                    SqlCommand insertMinion = new SqlCommand(insertMinionQuery, connection);
                    insertMinion.Parameters.AddWithValue("nam", input[1]);
                    insertMinion.Parameters.AddWithValue("age", input[2]);
                    insertMinion.Parameters.AddWithValue("townId", townId);

                    SqlDataReader sqlData = insertMinion.ExecuteReader();
                    sqlData.Close();
                }

                SqlCommand minionServant = new SqlCommand(insertMinionVillainQuery, connection);
                minionServant.Parameters.AddWithValue("villainId", resultVillain);
                minionServant.Parameters.AddWithValue("minionId", resultMinion);

                SqlDataReader sqlDataReader = minionServant.ExecuteReader();

                Console.WriteLine($"Successfully added {input[1]} to be minion of {secondInput[1]}.");
                sqlDataReader.Close();
            }
        }
    }
}
