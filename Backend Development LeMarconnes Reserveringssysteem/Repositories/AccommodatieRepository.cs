using Microsoft.Data.SqlClient;

namespace Backend_Development_LeMarconnes_Reserveringssysteem.Repositories
{
    public class AccommodatieRepository
    {
        private readonly string _connectionString;

        public AccommodatieRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ------------------ Read ------------------

        public List<Accommodatie> GetAccommodaties(int id, bool IncludeCamping, int CampingID, bool IncludeBoeking, int BoekingID)
        {
            var result = new List<Accommodatie>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Accommodatie a " +
                "JOIN Camping c ON a.CampingID = c.CampingID " +
                "WHERE @id = 0 OR a.AccommodatieID = @id " +
                "OR (@id = 0 AND (@campingID = 0 OR CampingID = @campingID))" + 
                "SELECT * FROM Boeking WHERE BoekingID = @boekingid OR @boekingID = 0 " +
                "ORDER BY Datum DESC", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@campingID", CampingID);
            command.Parameters.AddWithValue("@boekingID", BoekingID);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var accommodatie = new Accommodatie
                {
                    AccommodatieID = (int)reader["AccommodatieID"],
                    CampingID = (int)reader["CampingID"]
                };

                if (IncludeCamping)
                {
                    accommodatie.Camping = new Camping
                    {
                        CampingID = (int)reader["CampingID"],
                        Regels = reader["Regels"] as string,
                        Lengte = reader["Lengte"] as decimal?,
                        Breedte = reader["Breedte"] as decimal?,
                        Stroom = reader["Stroom"] as decimal?,
                        Huisdieren = reader["Huisdieren"] as bool?
                    };
                }
                if (IncludeBoeking)
                {
                    var boeking = new Boeking
                    {
                        BoekingID = (int)reader["BoekingID"],
                        GebruikerID = (int)reader["GebruikerID"],
                        Datum = reader["Datum"] as DateTime?,
                        AccommodatieID = (int)reader["AccommodatieID"],
                        CheckInDatum = (DateTime)reader["CheckInDatum"],
                        CheckOutDatum = (DateTime)reader["CheckOutDatum"],
                        AantalVolwassenen = reader["AantalVolwassenen"] as byte?,
                        AantalJongeKinderen = reader["AantalJongeKinderen"] as byte?,
                        AantalOudereKinderen = reader["AantalOudereKinderen"] as byte?,
                        Opmerking = reader["Opmerking"] as string,
                        Cancelled = reader["Cancelled"] as bool?
                    };
                    accommodatie.Boeking.Add(boeking);
                }
                result.Add(accommodatie);
            }
            return result;
        }
        // ------------------ Write ------------------
        public bool Create(Accommodatie accommodatie)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO Accommodatie (CampingID)
                VALUES (@CampingID)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@CampingID", accommodatie.CampingID);;

            return cmd.ExecuteNonQuery() > 0;
        }
        public bool Update(Accommodatie accommodatie)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                UPDATE Accommodatie
                SET 
                    CampingID = @CampingID,
                WHERE AccommodatieID = @id
                ";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", accommodatie.AccommodatieID);
            cmd.Parameters.AddWithValue("@CampingID", accommodatie.CampingID);

            return cmd.ExecuteNonQuery() > 0;
        }
        // ------------------ Delete ------------------
        public bool Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("DELETE FROM Accommodatie WHERE AccommodatieID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

    }
}

