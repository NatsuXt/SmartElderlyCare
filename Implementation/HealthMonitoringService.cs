using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using RoomDeviceManagement.Interfaces;
using RoomDeviceManagement.Models;
using RoomDeviceManagement.Services;

namespace RoomDeviceManagement.Implementation
{
    /// <summary>
    /// 健康监测服务实现类
    /// </summary>
    public class HealthMonitoringService : IHealthMonitoringService
    {
        private readonly DatabaseService _databaseService;

        public HealthMonitoringService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<HealthMonitoring> GetAllHealthRecords()
        {
            var records = new List<HealthMonitoring>();
            var sql = @"SELECT monitoring_id, elderly_id, monitoring_date, heart_rate, 
                              blood_pressure, oxygen_level, temperature, status, created_time 
                       FROM HealthMonitoring ORDER BY monitoring_date DESC";

            try
            {
                var dataTable = _databaseService.ExecuteQuery(sql);
                foreach (DataRow row in dataTable.Rows)
                {
                    records.Add(MapDataRowToHealthRecord(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有健康监测记录失败：{ex.Message}");
            }

            return records;
        }

        public List<HealthMonitoring> GetHealthRecordsByElderlyId(string elderlyId)
        {
            var records = new List<HealthMonitoring>();
            var sql = @"SELECT monitoring_id, elderly_id, monitoring_date, heart_rate, 
                              blood_pressure, oxygen_level, temperature, status, created_time 
                       FROM HealthMonitoring WHERE elderly_id = :elderlyId
                       ORDER BY monitoring_date DESC";

            try
            {
                var parameters = new[] { new OracleParameter("elderlyId", elderlyId) };

                var dataTable = _databaseService.ExecuteQuery(sql, parameters);
                foreach (DataRow row in dataTable.Rows)
                {
                    records.Add(MapDataRowToHealthRecord(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据老人ID获取健康监测记录失败：{ex.Message}");
            }

            return records;
        }

        public List<HealthMonitoring> GetHealthRecordsByType(string monitoringType)
        {
            var records = new List<HealthMonitoring>();
            // 根据监测类型筛选，这里通过检查相应字段是否为空来判断
            string sql = "";
            
            switch (monitoringType.ToLower())
            {
                case "heart_rate":
                case "heartrate":
                    sql = @"SELECT monitoring_id, elderly_id, monitoring_date, heart_rate, 
                                  blood_pressure, oxygen_level, temperature, status, created_time 
                           FROM HealthMonitoring WHERE heart_rate IS NOT NULL
                           ORDER BY monitoring_date DESC";
                    break;
                case "blood_pressure":
                case "bloodpressure":
                    sql = @"SELECT monitoring_id, elderly_id, monitoring_date, heart_rate, 
                                  blood_pressure, oxygen_level, temperature, status, created_time 
                           FROM HealthMonitoring WHERE blood_pressure IS NOT NULL
                           ORDER BY monitoring_date DESC";
                    break;
                case "oxygen_level":
                case "oxygenlevel":
                    sql = @"SELECT monitoring_id, elderly_id, monitoring_date, heart_rate, 
                                  blood_pressure, oxygen_level, temperature, status, created_time 
                           FROM HealthMonitoring WHERE oxygen_level IS NOT NULL
                           ORDER BY monitoring_date DESC";
                    break;
                case "temperature":
                    sql = @"SELECT monitoring_id, elderly_id, monitoring_date, heart_rate, 
                                  blood_pressure, oxygen_level, temperature, status, created_time 
                           FROM HealthMonitoring WHERE temperature IS NOT NULL
                           ORDER BY monitoring_date DESC";
                    break;
                default:
                    return records; // 返回空列表
            }

            try
            {
                var dataTable = _databaseService.ExecuteQuery(sql);
                foreach (DataRow row in dataTable.Rows)
                {
                    records.Add(MapDataRowToHealthRecord(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"根据监测类型获取记录失败：{ex.Message}");
            }

            return records;
        }

        public List<HealthMonitoring> GetAbnormalHealthRecords()
        {
            var records = new List<HealthMonitoring>();
            var sql = @"SELECT monitoring_id, elderly_id, monitoring_date, heart_rate, 
                              blood_pressure, oxygen_level, temperature, status, created_time 
                       FROM HealthMonitoring WHERE status IN ('Abnormal', 'Critical')
                       ORDER BY monitoring_date DESC";

            try
            {
                var dataTable = _databaseService.ExecuteQuery(sql);
                foreach (DataRow row in dataTable.Rows)
                {
                    records.Add(MapDataRowToHealthRecord(row));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取异常健康记录失败：{ex.Message}");
            }

            return records;
        }

        public List<HealthMonitoring> GetUnprocessedAbnormalRecords()
        {
            // 假设未处理的记录状态为"Critical"或"Abnormal"
            return GetAbnormalHealthRecords();
        }

        public bool AddHealthRecord(HealthMonitoring record)
        {
            var sql = @"INSERT INTO HealthMonitoring (elderly_id, monitoring_date, heart_rate, 
                              blood_pressure, oxygen_level, temperature, status, created_time)
                       VALUES (:elderlyId, :monitoringDate, :heartRate, 
                              :bloodPressure, :oxygenLevel, :temperature, :status, SYSDATE)";

            try
            {
                var parameters = new[]
                {
                    new OracleParameter("elderlyId", record.ElderlyId),
                    new OracleParameter("monitoringDate", record.MonitoringDate),
                    new OracleParameter("heartRate", (object?)record.HeartRate ?? DBNull.Value),
                    new OracleParameter("bloodPressure", record.BloodPressure ?? string.Empty),
                    new OracleParameter("oxygenLevel", (object?)record.OxygenLevel ?? DBNull.Value),
                    new OracleParameter("temperature", (object?)record.Temperature ?? DBNull.Value),
                    new OracleParameter("status", record.Status ?? string.Empty)
                };

                int result = _databaseService.ExecuteNonQuery(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加健康监测记录失败：{ex.Message}");
                return false;
            }
        }

        public bool AddHealthRecords(List<HealthMonitoring> records)
        {
            try
            {
                bool allSuccess = true;
                foreach (var record in records)
                {
                    if (!AddHealthRecord(record))
                    {
                        allSuccess = false;
                    }
                }
                return allSuccess;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"批量添加健康监测记录失败：{ex.Message}");
                return false;
            }
        }

        public bool UpdateHealthRecord(HealthMonitoring record)
        {
            var sql = @"UPDATE HealthMonitoring SET 
                              elderly_id = :elderlyId,
                              monitoring_date = :monitoringDate,
                              heart_rate = :heartRate,
                              blood_pressure = :bloodPressure,
                              oxygen_level = :oxygenLevel,
                              temperature = :temperature,
                              status = :status
                       WHERE monitoring_id = :monitoringId";

            try
            {
                var parameters = new[]
                {
                    new OracleParameter("elderlyId", record.ElderlyId),
                    new OracleParameter("monitoringDate", record.MonitoringDate),
                    new OracleParameter("heartRate", (object?)record.HeartRate ?? DBNull.Value),
                    new OracleParameter("bloodPressure", record.BloodPressure ?? string.Empty),
                    new OracleParameter("oxygenLevel", (object?)record.OxygenLevel ?? DBNull.Value),
                    new OracleParameter("temperature", (object?)record.Temperature ?? DBNull.Value),
                    new OracleParameter("status", record.Status ?? string.Empty),
                    new OracleParameter("monitoringId", record.MonitoringId)
                };

                int result = _databaseService.ExecuteNonQuery(sql, parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新健康监测记录失败：{ex.Message}");
                return false;
            }
        }

        public bool ProcessAbnormalRecord(string recordId, string processor, string measures)
        {
            // 将异常记录标记为已处理，这里简单地将状态改为"Processed"
            var sql = @"UPDATE HealthMonitoring SET 
                              status = 'Processed'
                       WHERE monitoring_id = :recordId";

            try
            {
                var parameters = new[] { new OracleParameter("recordId", recordId) };

                int result = _databaseService.ExecuteNonQuery(sql, parameters);
                
                // 可以在这里记录处理日志到另一个表
                Console.WriteLine($"异常记录 {recordId} 已由 {processor} 处理，处理措施：{measures}");
                
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理异常健康记录失败：{ex.Message}");
                return false;
            }
        }

        public Dictionary<string, HealthMonitoring> GetLatestHealthStatus(string elderlyId)
        {
            var latestRecords = new Dictionary<string, HealthMonitoring>();
            
            // 获取每种监测类型的最新记录
            var monitoringTypes = new[] { "heart_rate", "blood_pressure", "oxygen_level", "temperature" };
            
            foreach (var type in monitoringTypes)
            {
                var records = GetHealthRecordsByType(type)
                    .Where(r => r.ElderlyId.ToString() == elderlyId)
                    .OrderByDescending(r => r.MonitoringDate)
                    .ToList();
                
                if (records.Any())
                {
                    latestRecords[type] = records.First();
                }
            }

            return latestRecords;
        }

        public bool IsHealthDataAbnormal(decimal value, decimal minNormal, decimal maxNormal)
        {
            return value < minNormal || value > maxNormal;
        }

        public Dictionary<string, object> GetHealthDataStatistics(string elderlyId, string monitoringType, int days = 7)
        {
            var statistics = new Dictionary<string, object>();
            var startDate = DateTime.Now.AddDays(-days);
            
            var sql = @"SELECT heart_rate, blood_pressure, oxygen_level, temperature 
                       FROM HealthMonitoring 
                       WHERE elderly_id = :elderlyId AND monitoring_date >= :startDate
                       ORDER BY monitoring_date DESC";

            try
            {
                var parameters = new[]
                {
                    new OracleParameter("elderlyId", elderlyId),
                    new OracleParameter("startDate", startDate)
                };

                var dataTable = _databaseService.ExecuteQuery(sql, parameters);
                var values = new List<decimal>();

                foreach (DataRow row in dataTable.Rows)
                {
                    decimal? value = null;
                    switch (monitoringType.ToLower())
                    {
                        case "heart_rate":
                            if (row["heart_rate"] != DBNull.Value)
                                value = Convert.ToDecimal(row["heart_rate"]);
                            break;
                        case "oxygen_level":
                            if (row["oxygen_level"] != DBNull.Value)
                                value = Convert.ToDecimal(row["oxygen_level"]);
                            break;
                        case "temperature":
                            if (row["temperature"] != DBNull.Value)
                                value = Convert.ToDecimal(row["temperature"]);
                            break;
                    }
                    
                    if (value.HasValue)
                        values.Add(value.Value);
                }

                if (values.Any())
                {
                    statistics["count"] = values.Count;
                    statistics["average"] = values.Average();
                    statistics["min"] = values.Min();
                    statistics["max"] = values.Max();
                    statistics["latest"] = values.First();
                }
                else
                {
                    statistics["count"] = 0;
                    statistics["message"] = "在指定时间范围内没有找到相关数据";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取健康数据统计失败：{ex.Message}");
                statistics["error"] = ex.Message;
            }

            return statistics;
        }

        /// <summary>
        /// 将数据行映射到健康监测记录对象
        /// </summary>
        private HealthMonitoring MapDataRowToHealthRecord(DataRow row)
        {
            return new HealthMonitoring
            {
                MonitoringId = Convert.ToInt32(row["monitoring_id"]),
                ElderlyId = Convert.ToInt32(row["elderly_id"]),
                MonitoringDate = Convert.ToDateTime(row["monitoring_date"]),
                HeartRate = row["heart_rate"] != DBNull.Value ? Convert.ToInt32(row["heart_rate"]) : null,
                BloodPressure = row["blood_pressure"].ToString(),
                OxygenLevel = row["oxygen_level"] != DBNull.Value ? Convert.ToDecimal(row["oxygen_level"]) : null,
                Temperature = row["temperature"] != DBNull.Value ? Convert.ToDecimal(row["temperature"]) : null,
                Status = row["status"].ToString(),
                CreatedTime = Convert.ToDateTime(row["created_time"])
            };
        }
    }
}
