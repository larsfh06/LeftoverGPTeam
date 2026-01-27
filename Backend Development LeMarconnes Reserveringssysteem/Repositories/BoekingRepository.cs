using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Development_LeMarconnes_Reserveringssysteem.Repositories
{
    public class BoekingRepository
    {
        private readonly string _connectionString;

        public BoekingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ------------------ Read ------------------
        public List<Boeking> GetBoekingen(int id, int GebruikerID, int AccommodatieID, int BetalingID, DateTime? Datum, bool IncludeGebruiker, bool IncludeAccommodatie, bool IncludeBetalingen)
        {
            var result = new List<Boeking>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * " +
                "FROM Boeking b " +
                "LEFT JOIN Betaling be ON b.BoekingID = be.BoekingID " +
                "JOIN Gebruiker g ON b.GebruikerID = g.GebruikerID " +
                "JOIN Accommodatie a ON b.AccommodatieID = a.AccommodatieID " +
                "WHERE b.BoekingID = @id OR " +
                "(@id = 0 " +
                "AND (@gebruikerID = 0 OR b.GebruikerID = @gebruikerID) " +
                "AND (@accommodatieID = 0 OR b.AccommodatieID = @accommodatieID) " +
                "AND (@betalingID = 0 OR be.BetalingID = @betalingID) " +
                "AND (@datum IS NULL OR (@datum >= b.CheckInDatum AND @datum < b.CheckOutDatum))) " +
                "ORDER BY DatumOrigine DESC;", connection);

            command.Parameters.AddWithValue("@gebruikerID", GebruikerID);
            command.Parameters.AddWithValue("@accommodatieID", AccommodatieID);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@betalingID", BetalingID);
            command.Parameters.AddWithValue("@datum", Datum.HasValue ? Datum.Value : (object)DBNull.Value);

            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
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

                if (IncludeGebruiker && reader["GebruikerID"] != DBNull.Value)
                {
                    boeking.Gebruiker = new Gebruiker
                    {
                        GebruikerID = (int)reader["GebruikerID"],
                        Naam = (string)reader["Naam"],
                        Emailadres = (string)reader["Emailadres"],
                        HashedWachtwoord = (string)reader["HashedWachtwoord"],
                        Salt = (string)reader["Salt"],
                        Telefoon = reader["Telefoon"] as string,
                        Autokenteken = reader["Autokenteken"] as string,
                        Taal = reader["Taal"] as string
                    };
                }

                if (IncludeAccommodatie && reader["AccommodatieID"] != DBNull.Value)
                {
                    boeking.Accommodatie = new Accommodatie
                    {
                        AccommodatieID = (int)reader["AccommodatieID"],
                        CampingID = (int)reader["CampingID"],
                        Prijs = reader["Prijs"] as decimal?
                    };
                }

                if (IncludeBetalingen && reader["BetalingID"] != DBNull.Value)
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
                    boeking.Betalingen.Add(betaling);
                }

                result.Add(boeking);
            }

            return result;
        }

        // ------------------ Write ------------------
        public bool Create(Boeking boeking)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Overlap check
            string overlapCheckSql = @"
                SELECT COUNT(1)
                FROM Boeking
                WHERE AccommodatieID = @AccommodatieID
                  AND @CheckInDatum < CheckOutDatum
                  AND @CheckOutDatum > CheckInDatum
                  AND (Cancelled = 0 OR Cancelled IS NULL)";

            using (var overlapCmd = new SqlCommand(overlapCheckSql, conn))
            {
                overlapCmd.Parameters.AddWithValue("@AccommodatieID", boeking.AccommodatieID);
                overlapCmd.Parameters.AddWithValue("@CheckInDatum", boeking.CheckInDatum);
                overlapCmd.Parameters.AddWithValue("@CheckOutDatum", boeking.CheckOutDatum);

                if ((int)overlapCmd.ExecuteScalar() > 0)
                    return false;
            }

            string sql = @"
                INSERT INTO Boeking 
                (GebruikerID, Datum, AccommodatieID, CheckInDatum, CheckOutDatum, 
                 AantalVolwassenen, AantalJongeKinderen, AantalOudereKinderen, Opmerking, Cancelled)
                VALUES 
                (@GebruikerID, @Datum, @AccommodatieID, @CheckInDatum, @CheckOutDatum, 
                 @AantalVolwassenen, @AantalJongeKinderen, @AantalOudereKinderen, @Opmerking, @Cancelled)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@GebruikerID", boeking.GebruikerID);
            cmd.Parameters.AddWithValue("@Datum", boeking.Datum);
            cmd.Parameters.AddWithValue("@AccommodatieID", boeking.AccommodatieID);
            cmd.Parameters.AddWithValue("@CheckInDatum", boeking.CheckInDatum);
            cmd.Parameters.AddWithValue("@CheckOutDatum", boeking.CheckOutDatum);
            cmd.Parameters.AddWithValue("@AantalVolwassenen", boeking.AantalVolwassenen);
            cmd.Parameters.AddWithValue("@AantalJongeKinderen", boeking.AantalJongeKinderen);
            cmd.Parameters.AddWithValue("@AantalOudereKinderen", boeking.AantalOudereKinderen);
            cmd.Parameters.AddWithValue("@Opmerking", boeking.Opmerking);
            cmd.Parameters.AddWithValue("@Cancelled", boeking.Cancelled);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Update(Boeking boeking)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Overlap check (exclude current booking)
            string overlapCheckSql = @"
                SELECT COUNT(1)
                FROM Boeking
                WHERE AccommodatieID = @AccommodatieID
                  AND BoekingID <> @BoekingID
                  AND @CheckInDatum < CheckOutDatum
                  AND @CheckOutDatum > CheckInDatum
                  AND (Cancelled = 0 OR Cancelled IS NULL)";

            using (var overlapCmd = new SqlCommand(overlapCheckSql, conn))
            {
                overlapCmd.Parameters.AddWithValue("@AccommodatieID", boeking.AccommodatieID);
                overlapCmd.Parameters.AddWithValue("@BoekingID", boeking.BoekingID);
                overlapCmd.Parameters.AddWithValue("@CheckInDatum", boeking.CheckInDatum);
                overlapCmd.Parameters.AddWithValue("@CheckOutDatum", boeking.CheckOutDatum);

                if ((int)overlapCmd.ExecuteScalar() > 0)
                    return false;
            }

            string sql = @"
                UPDATE Boeking
                SET 
                    GebruikerID = @GebruikerID,
                    Datum = @Datum,
                    AccommodatieID = @AccommodatieID,
                    CheckInDatum = @CheckInDatum,
                    CheckOutDatum = @CheckOutDatum,
                    AantalVolwassenen = @AantalVolwassenen,
                    AantalJongeKinderen = @AantalJongeKinderen,
                    AantalOudereKinderen = @AantalOudereKinderen,
                    Opmerking = @Opmerking,
                    Cancelled = @Cancelled
                WHERE BoekingID = @BoekingID";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@BoekingID", boeking.BoekingID);
            cmd.Parameters.AddWithValue("@GebruikerID", boeking.GebruikerID);
            cmd.Parameters.AddWithValue("@Datum", boeking.Datum);
            cmd.Parameters.AddWithValue("@AccommodatieID", boeking.AccommodatieID);
            cmd.Parameters.AddWithValue("@CheckInDatum", boeking.CheckInDatum);
            cmd.Parameters.AddWithValue("@CheckOutDatum", boeking.CheckOutDatum);
            cmd.Parameters.AddWithValue("@AantalVolwassenen", boeking.AantalVolwassenen);
            cmd.Parameters.AddWithValue("@AantalJongeKinderen", boeking.AantalJongeKinderen);
            cmd.Parameters.AddWithValue("@AantalOudereKinderen", boeking.AantalOudereKinderen);
            cmd.Parameters.AddWithValue("@Opmerking", boeking.Opmerking);
            cmd.Parameters.AddWithValue("@Cancelled", boeking.Cancelled);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
