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
        public List<Accommodatie> GetAccommodaties(int id, int CampingID, bool IncludeCamping, int BoekingID, bool IncludeBoeking)
        {
            var result = new List<Accommodatie>();
            var dict = new Dictionary<int, Accommodatie>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Accommodatie a " +
                "LEFT JOIN Boeking b ON a.AccommodatieID = b.AccommodatieID " +
                "JOIN Camping c ON a.CampingID = c.CampingID " +
                "WHERE @id = 0 OR a.AccommodatieID = @id " +
                "OR (@id = 0 " +
                "AND (@campingID = 0 OR a.CampingID = @campingID) " +
                "AND (@boekingID = 0 OR b.BoekingID = @boekingID)) " +
                "ORDER BY b.Datum DESC", connection);

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@campingID", CampingID);
            command.Parameters.AddWithValue("@boekingID", BoekingID);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int accommodatieId = (int)reader["AccommodatieID"];

                if (!dict.TryGetValue(accommodatieId, out var accommodatie))
                {
                    accommodatie = new Accommodatie
                    {
                        AccommodatieID = accommodatieId,
                        CampingID = (int)reader["CampingID"]
                    };

                    if (IncludeCamping && reader["CampingID"] != DBNull.Value)
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

                    dict.Add(accommodatieId, accommodatie);
                    result.Add(accommodatie);
                }

                if (IncludeBoeking && reader["BoekingID"] != DBNull.Value)
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

                    accommodatie.Boekingen.Add(boeking);
                }
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

