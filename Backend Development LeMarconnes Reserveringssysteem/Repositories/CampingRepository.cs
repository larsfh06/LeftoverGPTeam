using Microsoft.Data.SqlClient;

namespace Backend_Development_LeMarconnes_Reserveringssysteem.Repositories
{
    public class CampingRepository
    {
        private readonly string _connectionString;

        public CampingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // ------------------ Read ------------------
        public List<Camping> GetCampings(int id, decimal stroom, bool huisdieren, int AccommodatieID, bool IncludeAccommodatie)
        {
            var result = new List<Camping>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Campings c " + 
                "JOIN Accommodatie a ON a.CampingID = @id " + 
                "WHERE CampingID = @id " +
                "OR (@id = 0 AND (@stroom = 0 OR Stroom >= @stroom) " +
                "AND (@huisdieren = false OR Huisdieren = @huisdieren) " +
                "AND (@accomodatieID = 0 OR AccommodatieID = @accommodatieID))", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@stroom", stroom);
            command.Parameters.AddWithValue("@huisdieren", huisdieren);
            command.Parameters.AddWithValue("@accommodateiID", AccommodatieID);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var camping = new Camping
                {
                    CampingID = (int)reader["CampingID"],
                    Regels = reader["Regels"] as string,
                    Lengte = reader["Lengte"] as decimal?,
                    Breedte = reader["Breedte"] as decimal?,
                    Stroom = reader["Stroom"] as decimal?,
                    Huisdieren = reader["Huisdieren"] as bool?
                };
                if (IncludeAccommodatie)
                {
                    camping.Accommodatie = new Accommodatie
                    {
                        AccommodatieID = (int)reader["AccommodatieID"],
                        CampingID = (int)reader["CampingID"]
                    };
                }
                result.Add(camping);
            }
            return result;
        }

        // ------------------ Write ------------------
        public bool Create(Camping camping)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO Camping (Regels, Lengte, Breedte, Stroom, Huisdieren)
                VALUES (@regels, @lengte, @breedte, @stroom, @huisdieren)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@regels", camping.Regels);
            cmd.Parameters.AddWithValue("@lengte", camping.Lengte);
            cmd.Parameters.AddWithValue("@breedte", camping.Breedte);
            cmd.Parameters.AddWithValue("@stroom", camping.Stroom);
            cmd.Parameters.AddWithValue("@huisdieren", camping.Huisdieren);

            return cmd.ExecuteNonQuery() > 0;
        }
        public bool Update(Camping camping)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                UPDATE Camping
                SET 
                    Regels = @regels,
                    Lengte = @lengte,
                    Breedte = @breedte,
                    Stroom = @stroom,
                    Huisdieren = @huisdieren
                WHERE CampingID = @id
                ";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", camping.CampingID);
            cmd.Parameters.AddWithValue("@regels", camping.Regels);
            cmd.Parameters.AddWithValue("@lengte", camping.Lengte);
            cmd.Parameters.AddWithValue("@breedte", camping.Breedte);
            cmd.Parameters.AddWithValue("@stroom", camping.Stroom);
            cmd.Parameters.AddWithValue("@huisdieren", camping.Huisdieren);

            return cmd.ExecuteNonQuery() > 0;
        }
        // ------------------ Delete ------------------
        public bool Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("DELETE FROM Camping WHERE CampingID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

    }
}
