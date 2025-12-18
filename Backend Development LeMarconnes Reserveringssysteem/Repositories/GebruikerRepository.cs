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
        public List<Gebruiker> GetGebruikers(int id, string naam, string telefoon,int BoekingID, bool IncludeBoekingen)
        {
            var result = new List<Gebruiker>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Gebruiker WHERE GebruikerID = @id " +
                "OR (@id = 0 AND (@naam = 'ALL' OR Naam = @naam) " +
                "AND (@telefoon = 'ALL' OR Telefoon = @telefoon)) " + 
                "SELECHT * FROM Boeking WHERE BoekingID = @boekingID OR @boekingID = 0", connection);
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
                    Taal = reader["Taal"] as string
                };
                if (IncludeBoekingen)
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
                INSERT INTO Gebruiker (Naam, Emailadres, HashedWachtwoord, Salt, Telefoon, Taal)
                VALUES (@Naam, @Emailadres, @HashedWachtwoord, @Salt, @Telefoon, @Taal)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Naam", gebruiker.Naam);
            cmd.Parameters.AddWithValue("@Emailadres", gebruiker.Emailadres);
            cmd.Parameters.AddWithValue("@HashedWachtwoord", gebruiker.HashedWachtwoord);
            cmd.Parameters.AddWithValue("@Salt", gebruiker.Salt);
            cmd.Parameters.AddWithValue("@Telefoon", gebruiker.Telefoon);
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
                    Telefoon = @Telefoon
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
            cmd.Parameters.AddWithValue("@Taal", gebruiker.Taal);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
