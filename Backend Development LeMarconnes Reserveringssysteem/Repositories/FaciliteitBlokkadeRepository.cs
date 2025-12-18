using Microsoft.Data.SqlClient;

namespace Backend_Development_LeMarconnes_Reserveringssysteem.Repositories
{
    public class FaciliteitBlokkadeRepository
    {
        private readonly string _connectionString;

        public FaciliteitBlokkadeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        // ------------------ Read ------------------
        public List<FaciliteitBlokkade> GetFaciliteitBlokkades(int id, string Status, string reden)
        {
            var result = new List<FaciliteitBlokkade>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                "SELECT * FROM FaciliteitBlokkade WHERE BlokkadeID = @id " +
                "OR (@id = 0 AND (@status = 'ALL' OR Status = @status) " +
                "AND (@BlokkadeReden = 'ALL' OR BlokkadeReden = @BlokkadeReden))", connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@status", Status);
            command.Parameters.AddWithValue("@BlokkadeReden", reden);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new FaciliteitBlokkade
                {
                    BlokkadeID = (int)reader["BlokkadeID"],
                    FaciliteitID = (int)reader["FaciliteitID"],
                    BlokkadeType = reader["BlokkadeType"] as string,
                    BeginDatum = (DateTime)reader["BeginDatum"],
                    EindDatum = (DateTime)reader["EindDatum"],
                    BlokkadeReden = reader["BlokkadeReden"] as string,
                    Status = reader["Status"] as string
                });
            }
            return result;
        }

        // ------------------ Write ------------------
        public bool Create(FaciliteitBlokkade faciliteitBlokkade)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO FaciliteitBlokkade (FaciliteitID, BlokkadeType, BeginDatum, EindDatum, BlokkadeReden, Status)
                VALUES (@FaciliteitID, @BlokkadeType, @BeginDatum, @EindDatum, @BlokkadeReden, @Status)";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@FaciliteitID",faciliteitBlokkade.FaciliteitID);
            cmd.Parameters.AddWithValue("@BlokkadeType", faciliteitBlokkade.BlokkadeType);
            cmd.Parameters.AddWithValue("@BeginDatum", faciliteitBlokkade.BeginDatum);
            cmd.Parameters.AddWithValue("@EindDatum", faciliteitBlokkade.EindDatum);
            cmd.Parameters.AddWithValue("@BlokkadeReden", faciliteitBlokkade.BlokkadeReden);
            cmd.Parameters.AddWithValue("@Status", faciliteitBlokkade.Status);

            return cmd.ExecuteNonQuery() > 0;
        }
        public bool Update(FaciliteitBlokkade faciliteitBlokkade)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string sql = @"
                UPDATE FaciliteitBlokkade
                SET 
                    FaciliteitID = @FaciliteitID,
                    BlokkadeType = @BlokkadeType,
                    BeginDatum = @BeginDatum,
                    EindDatum = @EindDatum,
                    BlokkadeReden = @BlokkadeReden
                    Status = @Status
                WHERE FaciliteitBlokkadeID = @id
                ";

            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", faciliteitBlokkade.BlokkadeID);
            cmd.Parameters.AddWithValue("@FaciliteitID", faciliteitBlokkade.FaciliteitID);
            cmd.Parameters.AddWithValue("@BlokkadeType", faciliteitBlokkade.BlokkadeType);
            cmd.Parameters.AddWithValue("@BeginDatum", faciliteitBlokkade.BeginDatum);
            cmd.Parameters.AddWithValue("@EindDatum", faciliteitBlokkade.EindDatum);
            cmd.Parameters.AddWithValue("@BlokkadeReden", faciliteitBlokkade.BlokkadeReden);
            cmd.Parameters.AddWithValue("@Status", faciliteitBlokkade.Status);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
