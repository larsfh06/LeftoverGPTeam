using Microsoft.Data.SqlClient;

namespace Backend_Development_LeMarconnes_Reserveringssysteem.Repositories
{
    public class FaciliteitRepository
    {
        private readonly string _connectionString;

        public FaciliteitRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // ------------------ Read ------------------
        public List<Faciliteit> GetFaciliteiten(int id)
        {
            var result = new List<Faciliteit>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Faciliteit WHERE @id = '0' OR FaciliteitID = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Faciliteit
                {
                    FaciliteitID = (int)reader["FaciliteitID"],
                    FaciliteitNaam = (string)reader["Faciliteit"],
                    Omschrijving = reader["Omschrijving"] as string,
                    Capaciteit = reader["Capaciteit"] as int?,
                    Openingstijd = reader["Openingstijd"] as DateTime?,
                    Sluitingstijd = reader["Sluitingstijd"] as DateTime?
                });
            }
            return result;
        }

        // ------------------ Write ------------------
        public bool Create(Faciliteit faciliteit)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO Faciliteit (Faciliteit, Omschrijving, Capaciteit, Openingstijd, Sluitingstijd)
                VALUES (@FaciliteitNaam, @Omschrijving, @Capaciteit, @Openingstijd, @Sluitingstijd)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@FaciliteitNaam", faciliteit.FaciliteitNaam);
            cmd.Parameters.AddWithValue("@Omschrijving", faciliteit.Omschrijving);
            cmd.Parameters.AddWithValue("@Capaciteit", faciliteit.Capaciteit);
            cmd.Parameters.AddWithValue("@Openingstijd", faciliteit.Openingstijd);
            cmd.Parameters.AddWithValue("@Sluitingstijd", faciliteit.Sluitingstijd);

            return cmd.ExecuteNonQuery() > 0;
        }
        public bool Update(Faciliteit faciliteit)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                UPDATE Faciliteit
                SET 
                    FaciliteitNaam = @FaciliteitNaam,
                    Omschrijving = @Omschrijving,
                    Capaciteit = @Capaciteit,
                    Openingstijd = @Openingstijd,
                    Sluitingstijd = @Sluitingstijd
                WHERE FaciliteitID = @id
                ";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", faciliteit.FaciliteitID);
            cmd.Parameters.AddWithValue("@FaciliteitNaam", faciliteit.FaciliteitNaam);
            cmd.Parameters.AddWithValue("@Omschrijving", faciliteit.Omschrijving);
            cmd.Parameters.AddWithValue("@Capaciteit", faciliteit.Capaciteit);
            cmd.Parameters.AddWithValue("@Openingstijd", faciliteit.Openingstijd);
            cmd.Parameters.AddWithValue("@Sluitingstijd", faciliteit.Sluitingstijd);

            return cmd.ExecuteNonQuery() > 0;
        }
        // ------------------ Delete ------------------
        public bool Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("DELETE FROM Faciliteit WHERE FaciliteitID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
