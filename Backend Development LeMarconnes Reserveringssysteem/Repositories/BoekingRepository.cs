using Microsoft.Data.SqlClient;

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
        public List<Boeking> GetFiltered(int id, int GebruikerID, bool IncludeBetalingen, bool IncludeGebruiker, bool IncludeAccommodatie)
        {
            var result = new List<Boeking>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Boeking b " +
                "JOIN Gebruiker g ON b.GebruikerID = g.GebruikerID " +
                "JOIN Acommodatie a ON b.AccommodatieID = a.AccommodatieID " +
                "WHERE BoekingID = @id " +
                "OR (@id = 0 AND (@gebruikerID = 0 OR GebruikerID = @gebruikerID) AND (@accommodatieID = 0 OR AccommodatieID = @accommodatieID)) " +
                "SELECT * FROM Betaling WHERE BoekingID = @id OR @id = 0 " +
                "ORDER BY DatumOrigine DESC", connection);
            command.Parameters.AddWithValue("@gebruikerID", GebruikerID);
            command.Parameters.AddWithValue("@accomodatieID", AccommodatieID);
            command.Parameters.AddWithValue("@id", id);
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

                if (IncludeGebruiker)
                {
                    boeking.Gebruiker = new Gebruiker
                    {
                        GebruikerID = (int)reader["GebruikerID"],
                        Naam = reader["Naam"] as string,
                        Emailadres = reader["Emailadres"] as string,
                        HashedWachtwoord = reader["HashedWachtwoord"] as string,
                        Salt = reader["Salt"] as string,
                        Telefoon = reader["Telefoon"] as string,
                        Autokenteken = reader["Autokenteken"] as string,
                        Taal = reader["Taal"] as string
                    };
                }

                if (IncludeAccommodatie)
                {
                    boeking.Accommodatie = new Accommodatie
                    {
                        AccommodatieID = (int)reader["AccommodatieID"],
                        CampingID = reader["CampingID"] as int
                    };
                }


                if (IncludeBetalingen)
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

            string sql = @"
                INSERT INTO Boeking (GebruikerID, Datum, AccommodatieID, CheckInDatum, CheckOutDatum, AantalVolwassenen, AantalJongeKinderen, AantalOudereKinderen, Opmerking, Cancelled)
                VALUES (@GebruikerID, @Datum, @AccommodatieID, @CheckInDatum, @CheckOutDatum, @AantalVolwassenen, @AantalJongeKinderen, @AantalOudereKinderen, @Opmerking, @Cancelled)";

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
                WHERE BoekingID = @id
                ";

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
