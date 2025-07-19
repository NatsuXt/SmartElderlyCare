using Oracle.ManagedDataAccess.Client;
using System.Data;

public class ElderlyRegistrationService
{
    private readonly string connectionString;

    public ElderlyRegistrationService(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("OracleDb");
    }

    public int RegisterElderly(ElderlyDto dto)
    {
        using var conn = new OracleConnection(connectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction();

        try
        {
            // 插入 ElderlyInfo + 返回 elderly_id
            var cmd = conn.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = @"
                INSERT INTO ElderlyInfo (...)
                RETURNING elderly_id INTO :elderlyId";

            // 添加参数（略）...

            var outParam = new OracleParameter(":elderlyId", OracleDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outParam);
            cmd.ExecuteNonQuery();

            int newElderlyId = Convert.ToInt32(outParam.Value.ToString());

            // 插入 HealthAssessment、HealthMonitoring、FamilyInfo（略）

            transaction.Commit();
            return newElderlyId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
