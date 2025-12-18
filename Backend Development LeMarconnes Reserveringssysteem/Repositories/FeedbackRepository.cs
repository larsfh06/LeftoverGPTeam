using Microsoft.Data.SqlClient;
using Microsoft.OpenApi;

namespace Backend_Development_LeMarconnes_Reserveringssysteem.Repositories
{
    public class FeedbackRepository
    {
        private readonly string _connectionString;

        public FeedbackRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // ------------------ Read ------------------
        public List<Feedback> GetFeedback(int id, int gebruikerID, bool IncludeGebruiker, bool IncludeBoeking)
        {
            var result = new List<Feedback>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM Feedback WHERE FeedbackID = @id " +
                "OR (@id = 0 AND (@gebruikerID = 0 OR GebruikerID = @gebruikerID))", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@gebruikerID", gebruikerID);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Feedback
                {
                    FeedbackID = (int)reader["FeedbackID"],
                    GebruikerID = (int)reader["GebruikerID"],
                    BoekingID = (int)reader["BoekingID"],
                    FeedbackScore = (int)reader["FeedbackScore"],
                    FeedbackTekst = reader["FeedbackTekst"] as string,
                    FeedbackDatum = (DateTime)reader["FeedbackDatum"]
                });
            }
            return result;
        }

        // ------------------ Write ------------------
        public bool Create(Feedback feedback)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO Feedback (GebruikerID, BoekingID, FeedbackScore, FeedbackTekst, FeedbackDatum)
                VALUES (@GebruikerID, @BoekingID, @FeedbackScore, @stroom, @FeedbackDatum)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@GebruikerID", feedback.GebruikerID);
            cmd.Parameters.AddWithValue("@BoekingID", feedback.BoekingID);
            cmd.Parameters.AddWithValue("@FeedbackScore", feedback.FeedbackScore);
            cmd.Parameters.AddWithValue("@FeedbackTekst", feedback.FeedbackTekst);
            cmd.Parameters.AddWithValue("@FeedbackDatum", feedback.FeedbackDatum);

            return cmd.ExecuteNonQuery() > 0;
        }
        public bool Update(Feedback feedback)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                UPDATE Feedback
                SET 
                    GebruikerID = @GebruikerID,
                    BoekingID = @BoekingID,
                    FeedbackScore = @FeedbackScore,
                    FeedbackTekst = @FeedbackTekst,
                    FeedbackDatum = @FeedbackDatum
                WHERE FeedbackID = @id
                ";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", feedback.FeedbackID);
            cmd.Parameters.AddWithValue("@GebruikerID", feedback.GebruikerID);
            cmd.Parameters.AddWithValue("@BoekingID", feedback.BoekingID);
            cmd.Parameters.AddWithValue("@FeedbackScore", feedback.FeedbackScore);
            cmd.Parameters.AddWithValue("@FeedbackTekst", feedback.FeedbackTekst);
            cmd.Parameters.AddWithValue("@FeedbackDatum", feedback.FeedbackDatum);

            return cmd.ExecuteNonQuery() > 0;
        }
        // ------------------ Delete ------------------
        public bool Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand("DELETE FROM Feedback WHERE FeedbackID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
