using Microsoft.Data.SqlClient;

namespace Backend_Development_LeMarconnes_Reserveringssysteem.Repositories
{
    public class MedewerkerRepository
    {
        private readonly string _connectionString;

        public MedewerkerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // ------------------ Read ------------------
        public List<Medewerker> GetMedewerkers(int id, string naam)
        {
            var result = new List<Medewerker>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Medewerker WHERE MedewerkerID = @id " +
                "OR (@id = 0 AND (@naam = 'ALL' OR Naam = @naam))", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@naam", naam);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Medewerker
                {
                    MedewerkerID = (int)reader["MedewerkerID"],
                    Naam = (string)reader["Naam"],
                    Emailadres = (string)reader["Emailadres"],
                    HashedWachtwoord = (string)reader["HashedWachtwoord"],
                    Salt = (string)reader["Salt"],
                    Telefoon = reader["Telefoon"] as string,
                    Taal = reader["Taal"] as string,
                    Accounttype = (string)reader["Accounttype"]
                });
            }
            return result;
        }

        // ------------------ Write ------------------
        public bool Create(Medewerker medewerker)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO Medewerker (Naam, Emailadres, HashedWachtwoord, Salt, Telefoon, Taal, Accounttype)
                VALUES (@Naam, @Emailadres, @HashedWachtwoord, @Salt, @Telefoon, @Taal, @Accounttype)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Naam", medewerker.Naam);
            cmd.Parameters.AddWithValue("@Emailadres", medewerker.Emailadres);
            cmd.Parameters.AddWithValue("@HashedWachtwoord", medewerker.HashedWachtwoord);
            cmd.Parameters.AddWithValue("@Salt", medewerker.Salt);
            cmd.Parameters.AddWithValue("@Telefoon", medewerker.Telefoon);
            cmd.Parameters.AddWithValue("@Taal", medewerker.Taal);
            cmd.Parameters.AddWithValue("@Accounttype", medewerker.Accounttype);

            return cmd.ExecuteNonQuery() > 0;
        }
        public bool Update(Medewerker medewerker)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                UPDATE Medewerker
                SET 
                    Naam = @Naam,
                    Emailadres = @Emailadres,
                    HashedWachtwoord = @HashedWachtwoord,
                    Salt = @Salt,
                    Telefoon = @Telefoon
                    Taal = @Taal
                    Accounttype = @Accounttype
                WHERE GebruikerID = @id
                ";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Naam", medewerker.Naam);
            cmd.Parameters.AddWithValue("@Emailadres", medewerker.Emailadres);
            cmd.Parameters.AddWithValue("@HashedWachtwoord", medewerker.HashedWachtwoord);
            cmd.Parameters.AddWithValue("@Salt", medewerker.Salt);
            cmd.Parameters.AddWithValue("@Telefoon", medewerker.Telefoon);
            cmd.Parameters.AddWithValue("@Taal", medewerker.Taal);
            cmd.Parameters.AddWithValue("@Accounttype", medewerker.Accounttype);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
