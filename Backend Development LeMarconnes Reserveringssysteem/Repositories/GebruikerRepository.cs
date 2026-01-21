using Microsoft.Data.SqlClient;

namespace Backend_Development_LeMarconnes_Reserveringssysteem.Repositories
{
    public class GebruikerRepository
    {
        private readonly string _connectionString;

        public GebruikerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // ------------------ Read ------------------
        public List<Gebruiker> GetGebruikers(int id, string naam, string telefoon, int BoekingID, bool IncludeBoekingen)
        {
            var result = new List<Gebruiker>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Gebruiker g LEFT JOIN Boeking b ON b.GebruikerID = g.GebruikerID " +
                "WHERE g.GebruikerID = @id " +
                "OR (@id = 0 " +
                "AND (@naam = 'ALL' OR Naam = @naam) " +
                "AND (@telefoon = 'ALL' OR Telefoon = @telefoon) " +
                "AND (@boekingID = 0 OR b.BoekingID = @boekingID))", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@naam", naam);
            command.Parameters.AddWithValue("@telefoon", telefoon);
            command.Parameters.AddWithValue("@boekingID", BoekingID);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var gebruiker = new Gebruiker
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
                if (IncludeBoekingen && reader["BoekingID"] != DBNull.Value)
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

                    #pragma warning disable CS8602 // Dereference of a possibly null reference.
                    gebruiker.Boekingen.Add(boeking);

                }
                result.Add(gebruiker);
            }
            return result;
        }

        // ------------------ Write ------------------
        public bool Create(Gebruiker gebruiker)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO Gebruiker (Naam, Emailadres, HashedWachtwoord, Salt, Telefoon, Autokenteken, Taal)
                VALUES (@Naam, @Emailadres, @HashedWachtwoord, @Salt, @Telefoon, @Autokenteken, @Taal)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Naam", gebruiker.Naam);
            cmd.Parameters.AddWithValue("@Emailadres", gebruiker.Emailadres);
            cmd.Parameters.AddWithValue("@HashedWachtwoord", gebruiker.HashedWachtwoord);
            cmd.Parameters.AddWithValue("@Salt", gebruiker.Salt);
            cmd.Parameters.AddWithValue("@Telefoon", gebruiker.Telefoon);
            cmd.Parameters.AddWithValue("@Autokenteken", gebruiker.Autokenteken);
            cmd.Parameters.AddWithValue("@Taal", gebruiker.Taal);

            return cmd.ExecuteNonQuery() > 0;
        }
        public bool Update(Gebruiker gebruiker)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                UPDATE Gebruiker
                SET 
                    Naam = @Naam,
                    Emailadres = @Emailadres,
                    HashedWachtwoord = @HashedWachtwoord,
                    Salt = @Salt,
                    Telefoon = @Telefoon,
                    Autokenteken = @Autokenteken,
                    Taal = @Taal
                WHERE GebruikerID = @id
                ";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", gebruiker.GebruikerID);
            cmd.Parameters.AddWithValue("@Naam", gebruiker.Naam);
            cmd.Parameters.AddWithValue("@Emailadres", gebruiker.Emailadres);
            cmd.Parameters.AddWithValue("@HashedWachtwoord", gebruiker.HashedWachtwoord);
            cmd.Parameters.AddWithValue("@Salt", gebruiker.Salt);
            cmd.Parameters.AddWithValue("@Telefoon", gebruiker.Telefoon);
            cmd.Parameters.AddWithValue("@Autokenteken", gebruiker.Autokenteken);
            cmd.Parameters.AddWithValue("@Taal", gebruiker.Taal);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
