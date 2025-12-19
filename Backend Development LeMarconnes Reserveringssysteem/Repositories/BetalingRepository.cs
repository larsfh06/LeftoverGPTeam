using Microsoft.Data.SqlClient;

namespace Backend_Development_LeMarconnes_Reserveringssysteem.Repositories
{
    public class BetalingRepository
    {
        private readonly string _connectionString;

        public BetalingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // ------------------ Read ------------------
        public List<Betaling> GetBetalingen(int id, string Status, int BoekingID, bool IncludeBoeking)
        {
            var result = new List<Betaling>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Betaling be JOIN Boeking b ON be.BoekingID = b.BoekingID " +
                "WHERE BetalingID = @id " +
                "OR (@id = 0 AND (@status = 'ALL' OR Status = @status) " +
                "AND (@boekingID = 0 OR be.BoekingID = @boekingID)) ", connection);
            command.Parameters.AddWithValue("@status", Status);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@boekingID", BoekingID);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var betaling = new Betaling
                {
                    BetalingID = (int)reader["BetalingID"],
                    BoekingID = (int)reader["BoekingID"],
                    Type = (string)reader["Type"],
                    Bedrag = (decimal)reader["Bedrag"],
                    Methode = reader["Methode"] as string,
                    Status = reader["Status"] as string,
                    Korting = reader["Korting"] as decimal?,
                    DatumOrigine = reader["DatumOrigine"] as DateTime?,
                    DatumBetaald = reader["DatumBetaald"] as DateTime?
                };
                if (IncludeBoeking && reader["GebruikerID"] != DBNull.Value)
                {
                    betaling.Boeking = new Boeking
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

                }
                result.Add(betaling);

            }
            return result;
        }

        // ------------------ Write ------------------
        public bool Create(Betaling betaling)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO Betaling (BoekingID, Type, Bedrag, Methode, Status, Korting, DatumOrigine, DatumBetaald)
                VALUES (@BoekingID, @Type, @Bedrag, @Methode, @Status, @Korting, @DatumOrigine, @DatumBetaald)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@BoekingID", betaling.BoekingID);
            cmd.Parameters.AddWithValue("@Type", betaling.Type);
            cmd.Parameters.AddWithValue("@Bedrag", betaling.Bedrag);
            cmd.Parameters.AddWithValue("@Methode", betaling.Methode);
            cmd.Parameters.AddWithValue("@Status", betaling.Status);
            cmd.Parameters.AddWithValue("@Korting", betaling.Korting);
            cmd.Parameters.AddWithValue("@DatumOrigine", betaling.DatumOrigine);
            cmd.Parameters.AddWithValue("@DatumBetaald", betaling.DatumBetaald);

            return cmd.ExecuteNonQuery() > 0;
        }
        public bool Update(Betaling betaling)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                UPDATE Betaling
                SET 
                    BoekingID = @BoekingID,
                    Type = @Type,
                    Bedrag = @Bedrag,
                    Methode = @Methode,
                    Status = @Status,
                    Korting = @Korting,
                    DatumOrigine = @DatumOrigine,
                    DatumBetaald = @DatumBetaald
                WHERE BetalingID = @id
                ";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", betaling.BetalingID);
            cmd.Parameters.AddWithValue("@BoekingID", betaling.BoekingID);
            cmd.Parameters.AddWithValue("@Type", betaling.Type);
            cmd.Parameters.AddWithValue("@Bedrag", betaling.Bedrag);
            cmd.Parameters.AddWithValue("@Methode", betaling.Methode);
            cmd.Parameters.AddWithValue("@Status", betaling.Status);
            cmd.Parameters.AddWithValue("@Korting", betaling.Korting);
            cmd.Parameters.AddWithValue("@DatumOrigine", betaling.DatumOrigine);
            cmd.Parameters.AddWithValue("@DatumBetaald", betaling.DatumBetaald);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
