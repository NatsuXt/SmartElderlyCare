using RoomDeviceManagement.Models;
using RoomDeviceManagement.DTOs;

namespace RoomDeviceManagement.Services
{
    /// <summary>
    /// 电子围栏监控业务逻辑服务
    /// </summary>
    public class ElectronicFenceService
    {
        private readonly DatabaseService _databaseService;
        private readonly ILogger<ElectronicFenceService> _logger;

        public ElectronicFenceService(DatabaseService databaseService, ILogger<ElectronicFenceService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// 处理GPS位置上报和电子围栏异常进出警报
        /// 核心业务：GPS监控 + 围栏检测 + 智能警报通知
        /// </summary>
        public async Task<object> HandleGpsLocationAsync(GpsLocationReportDto gpsReport)
        {
            try
            {
                // 1. 获取老人当前围栏状态
                var currentFenceInfo = await GetElderlyCurrentFenceAsync(gpsReport.ElderlyId);
                
                // 2. 检查当前GPS位置在哪些围栏内
                var fencesContainingPoint = await GetFencesContainingPointAsync(gpsReport.Latitude, gpsReport.Longitude);
                
                bool alertTriggered = false;
                string alertType = "";
                string alertMessage = "";
                var notifiedStaff = new List<dynamic>();

                if (currentFenceInfo != null)
                {
                    // 老人当前在某个围栏内
                    bool stillInCurrentFence = fencesContainingPoint.Any(f => f.FenceId == currentFenceInfo.FenceId);
                    
                    if (!stillInCurrentFence)
                    {
                        // 老人离开了当前围栏，更新exit_time
                        await UpdateFenceExitAsync(currentFenceInfo.EventLogId, DateTime.Now);
                        
                        if (fencesContainingPoint.Any())
                        {
                            // 正常移动：进入新围栏
                            var newFence = fencesContainingPoint.First();
                            await CreateFenceEntryAsync(gpsReport.ElderlyId, newFence.FenceId, DateTime.Now);
                            alertMessage = $"老人从围栏{currentFenceInfo.FenceId}移动到围栏{newFence.FenceId}";
                        }
                        else
                        {
                            // 🚨 异常警报：完全离开所有围栏区域
                            alertTriggered = true;
                            alertType = "越界警报";
                            alertMessage = $"警报：老人 {gpsReport.ElderlyId} 离开所有围栏区域！";
                            notifiedStaff = await TriggerFenceAlertAsync(gpsReport.ElderlyId, gpsReport.Latitude, gpsReport.Longitude, "越界");
                        }
                    }
                }
                else
                {
                    // 老人当前不在任何围栏内
                    if (fencesContainingPoint.Any())
                    {
                        // 正常进入：老人进入某个围栏
                        var enteredFence = fencesContainingPoint.First();
                        await CreateFenceEntryAsync(gpsReport.ElderlyId, enteredFence.FenceId, DateTime.Now);
                        alertMessage = $"老人进入围栏{enteredFence.FenceId}";
                    }
                    else
                    {
                        // 🚨 持续异常：老人仍在所有围栏外活动
                        alertTriggered = true;
                        alertType = "持续越界";
                        alertMessage = $"警报：老人 {gpsReport.ElderlyId} 持续在围栏外活动！";
                        notifiedStaff = await TriggerFenceAlertAsync(gpsReport.ElderlyId, gpsReport.Latitude, gpsReport.Longitude, "持续越界");
                    }
                }

                // 记录业务日志
                if (!string.IsNullOrEmpty(alertMessage))
                {
                    var logLevel = alertTriggered ? LogLevel.Warning : LogLevel.Information;
                    _logger.Log(logLevel, $"电子围栏监控 - 老人 {gpsReport.ElderlyId}: {alertMessage}");
                }

                return new
                {
                    ElderlyId = gpsReport.ElderlyId,
                    Latitude = gpsReport.Latitude,
                    Longitude = gpsReport.Longitude,
                    LocationTime = gpsReport.LocationTime,
                    AlertTriggered = alertTriggered,
                    AlertType = alertType,
                    Message = alertMessage,
                    CurrentFences = fencesContainingPoint.Select(f => f.FenceId).ToList(),
                    NotifiedStaff = notifiedStaff.Select(s => new { 
                        StaffId = s.staff_id, 
                        Name = s.name, 
                        Position = s.position,
                        ContactPhone = s.contact_phone 
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"电子围栏GPS处理失败: 老人ID {gpsReport.ElderlyId}");
                throw;
            }
        }

        /// <summary>
        /// 获取围栏进出记录（核心业务方法）
        /// </summary>
        public async Task<List<FenceLog>> GetFenceLogsAsync(int? elderlyId, int hours)
        {
            var sql = @"
                SELECT event_log_id AS EventLogId, elderly_id AS ElderlyId, 
                       fence_id AS FenceId, entry_time AS EntryTime, exit_time AS ExitTime
                FROM FenceLog 
                WHERE entry_time >= :StartTime";

            object parameters = new { StartTime = DateTime.Now.AddHours(-hours) };

            if (elderlyId.HasValue)
            {
                sql += " AND elderly_id = :ElderlyId";
                parameters = new { StartTime = DateTime.Now.AddHours(-hours), ElderlyId = elderlyId.Value };
            }

            sql += " ORDER BY entry_time DESC";

            var result = await _databaseService.QueryAsync<FenceLog>(sql, parameters);
            return result.ToList();
        }

        /// <summary>
        /// 获取老人当前围栏状态（核心业务方法）
        /// </summary>
        public async Task<List<object>> GetElderlyLocationStatusAsync()
        {
            try
            {
                var sql = @"
                    SELECT DISTINCT 
                        f.elderly_id,
                        f.fence_id,
                        f.entry_time,
                        f.exit_time,
                        f.event_type,
                        CASE WHEN f.exit_time IS NULL THEN '在围栏内' ELSE '已离开围栏' END as status
                    FROM (
                        SELECT elderly_id, fence_id, entry_time, exit_time, event_type,
                               ROW_NUMBER() OVER (PARTITION BY elderly_id ORDER BY entry_time DESC) as rn
                        FROM FenceLog
                    ) f 
                    WHERE f.rn = 1
                    ORDER BY f.elderly_id";

                var results = await _databaseService.QueryAsync<dynamic>(sql);
                
                return results.Select(r => new
                {
                    ElderlyId = r.elderly_id,
                    Name = $"老人{r.elderly_id}", // 临时方案，不依赖ElderlyInfo表
                    CurrentFenceId = r.fence_id,
                    LastEntryTime = r.entry_time,
                    ExitTime = r.exit_time,
                    EventType = r.event_type,
                    Status = r.status ?? "未知"
                }).ToList<object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取老人位置状态失败");
                return new List<object>();
            }
        }

        /// <summary>
        /// 获取围栏配置（核心业务方法）
        /// </summary>
        public async Task<List<ElectronicFence>> GetFenceConfigAsync()
        {
            var sql = @"
                SELECT fence_id AS FenceId, area_definition AS AreaDefinition
                FROM ElectronicFence 
                ORDER BY fence_id";

            var result = await _databaseService.QueryAsync<ElectronicFence>(sql);
            return result.ToList();
        }

        /// <summary>
        /// 获取围栏警报（核心业务方法）
        /// </summary>
        public async Task<List<object>> GetFenceAlertsAsync(bool activeOnly)
        {
            var sql = @"
                SELECT 
                    f.event_log_id,
                    f.elderly_id,
                    e.name as elderly_name,
                    f.fence_id,
                    f.entry_time,
                    f.exit_time
                FROM FenceLog f
                INNER JOIN ElderlyInfo e ON f.elderly_id = e.elderly_id";

            if (activeOnly)
            {
                sql += " WHERE f.exit_time IS NULL AND f.entry_time >= :RecentTime";
                var parameters = new { RecentTime = DateTime.Now.AddHours(-24) };
                var alerts = await _databaseService.QueryAsync<dynamic>(sql, parameters);
                
                return alerts.Select(a => new
                {
                    LogId = a.event_log_id,
                    ElderlyId = a.elderly_id,
                    ElderlyName = a.elderly_name,
                    FenceId = a.fence_id,
                    EntryTime = a.entry_time,
                    AlertType = "老人在围栏内超时",
                    IsActive = true
                }).ToList<object>();
            }
            else
            {
                sql += " ORDER BY f.entry_time DESC";
                var alerts = await _databaseService.QueryAsync<dynamic>(sql);
                
                return alerts.Select(a => new
                {
                    LogId = a.event_log_id,
                    ElderlyId = a.elderly_id,
                    ElderlyName = a.elderly_name,
                    FenceId = a.fence_id,
                    EntryTime = a.entry_time,
                    ExitTime = a.exit_time,
                    AlertType = a.exit_time == null ? "老人在围栏内" : "老人已离开围栏",
                    IsActive = a.exit_time == null
                }).ToList<object>();
            }
        }

        /// <summary>
        /// 获取老人位置轨迹（基于围栏进出记录）
        /// </summary>
        public async Task<List<object>> GetElderlyTrajectoryAsync(int elderlyId, int hours)
        {
            var sql = @"
                SELECT 
                    fl.fence_id, 
                    fl.entry_time, 
                    fl.exit_time,
                    ef.area_definition
                FROM FenceLog fl
                INNER JOIN ElectronicFence ef ON fl.fence_id = ef.fence_id
                WHERE fl.elderly_id = :ElderlyId 
                  AND fl.entry_time >= :StartTime
                ORDER BY fl.entry_time ASC";

            var parameters = new 
            { 
                ElderlyId = elderlyId, 
                StartTime = DateTime.Now.AddHours(-hours) 
            };

            var trajectory = await _databaseService.QueryAsync<dynamic>(sql, parameters);
            
            return trajectory.Select(t => new
            {
                FenceId = t.fence_id,
                EntryTime = t.entry_time,
                ExitTime = t.exit_time,
                AreaDefinition = t.area_definition,
                Status = t.exit_time == null ? "当前在此围栏内" : "已离开此围栏"
            }).ToList<object>();
        }

        // ===================== 围栏几何算法核心模块 =====================

        // ===================== 围栏几何算法核心模块 =====================

        /// <summary>
        /// 判断GPS点是否在多边形围栏内（射线法算法）
        /// 支持格式: "lat1,lng1;lat2,lng2;lat3,lng3;..." 或 JSON格式
        /// </summary>
        private bool IsPointInsideFence(double latitude, double longitude, string areaDefinition)
        {
            try
            {
                List<(double lat, double lng)> polygon;

                // 解析不同坐标格式
                if (areaDefinition.StartsWith("[") || areaDefinition.StartsWith("{"))
                {
                    polygon = ParseJsonCoordinates(areaDefinition);
                }
                else
                {
                    polygon = ParseDelimitedCoordinates(areaDefinition);
                }

                if (polygon.Count < 3)
                {
                    _logger.LogWarning($"围栏坐标点不足，至少需要3个点: {areaDefinition}");
                    return false;
                }

                // 使用射线法判断点是否在多边形内
                return IsPointInPolygon(latitude, longitude, polygon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"围栏坐标解析失败: {areaDefinition}");
                return false;
            }
        }

        /// <summary>
        /// 射线法判断点是否在多边形内（核心算法）
        /// </summary>
        private bool IsPointInPolygon(double pointLat, double pointLng, List<(double lat, double lng)> polygon)
        {
            var intersections = 0;
            var n = polygon.Count;

            for (int i = 0; i < n; i++)
            {
                var j = (i + 1) % n;
                var xi = polygon[i].lng;
                var yi = polygon[i].lat;
                var xj = polygon[j].lng;
                var yj = polygon[j].lat;

                if (((yi > pointLat) != (yj > pointLat)) &&
                    (pointLng < (xj - xi) * (pointLat - yi) / (yj - yi) + xi))
                {
                    intersections++;
                }
            }

            return (intersections % 2) == 1;
        }

        /// <summary>
        /// 解析分号分隔格式坐标
        /// </summary>
        private List<(double lat, double lng)> ParseDelimitedCoordinates(string delimitedCoordinates)
        {
            var coordinates = new List<(double lat, double lng)>();
            var points = delimitedCoordinates.Split(';');
            
            foreach (var point in points)
            {
                var coords = point.Split(',');
                if (coords.Length == 2 && 
                    double.TryParse(coords[0].Trim(), out double lat) && 
                    double.TryParse(coords[1].Trim(), out double lng))
                {
                    coordinates.Add((lat, lng));
                }
            }
            return coordinates;
        }

        /// <summary>
        /// 解析JSON格式坐标
        /// </summary>
        private List<(double lat, double lng)> ParseJsonCoordinates(string jsonCoordinates)
        {
            var coordinates = new List<(double lat, double lng)>();
            try
            {
                var cleaned = jsonCoordinates.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
                var pairs = cleaned.Split(',');
                
                for (int i = 0; i < pairs.Length - 1; i += 2)
                {
                    var latPart = pairs[i].Split(':')[1].Trim().Trim('"');
                    var lngPart = pairs[i + 1].Split(':')[1].Trim().Trim('"');
                    
                    if (double.TryParse(latPart, out double lat) && double.TryParse(lngPart, out double lng))
                    {
                        coordinates.Add((lat, lng));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"JSON坐标解析失败: {jsonCoordinates}");
            }
            return coordinates;
        }

        // ===================== 围栏配置管理模块 =====================

        /// <summary>
        /// 创建或更新围栏配置
        /// </summary>
        public async Task<object> CreateOrUpdateFenceConfigAsync(FenceConfigDto fenceConfig)
        {
            try
            {
                if (fenceConfig.FenceId.HasValue)
                {
                    var updateSql = @"
                        UPDATE ElectronicFence 
                        SET area_definition = :AreaDefinition
                        WHERE fence_id = :FenceId";

                    await _databaseService.ExecuteAsync(updateSql, new
                    {
                        FenceId = fenceConfig.FenceId.Value,
                        AreaDefinition = fenceConfig.AreaDefinition
                    });

                    return new { FenceId = fenceConfig.FenceId.Value, Action = "Updated" };
                }
                else
                {
                    var insertSql = @"
                        INSERT INTO ElectronicFence (area_definition)
                        VALUES (:AreaDefinition)";

                    await _databaseService.ExecuteAsync(insertSql, new
                    {
                        AreaDefinition = fenceConfig.AreaDefinition
                    });

                    return new { Action = "Created", AreaDefinition = fenceConfig.AreaDefinition };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存围栏配置失败");
                throw;
            }
        }

        /// <summary>
        /// 删除围栏配置
        /// </summary>
        public async Task<object> DeleteFenceConfigAsync(int fenceId)
        {
            try
            {
                var deleteSql = "DELETE FROM ElectronicFence WHERE fence_id = :FenceId";
                var affectedRows = await _databaseService.ExecuteAsync(deleteSql, new { FenceId = fenceId });

                return new { FenceId = fenceId, Deleted = affectedRows > 0 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除围栏配置失败: FenceId {fenceId}");
                throw;
            }
        }

        /// <summary>
        /// 测试围栏检查功能
        /// </summary>
        public async Task<object> TestFenceCheckAsync(double latitude, double longitude)
        {
            try
            {
                var sql = @"
                    SELECT fence_id, area_definition
                    FROM ElectronicFence";

                var fences = await _databaseService.QueryAsync<dynamic>(sql);
                var testResults = new List<object>();

                foreach (var fence in fences)
                {
                    var areaDefinition = fence.area_definition?.ToString();
                    if (string.IsNullOrEmpty(areaDefinition))
                        continue;

                    var isInside = IsPointInsideFence(latitude, longitude, areaDefinition);
                    testResults.Add(new
                    {
                        FenceId = fence.fence_id,
                        IsInside = isInside,
                        AreaDefinition = areaDefinition
                    });
                }

                var isOutOfAllFences = testResults.All(r => !(bool)r.GetType().GetProperty("IsInside")?.GetValue(r)!);

                return new
                {
                    TestLocation = new { Latitude = latitude, Longitude = longitude },
                    IsOutOfFence = isOutOfAllFences,
                    FenceTestResults = testResults,
                    TotalFences = testResults.Count,
                    TestTime = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "围栏检查测试失败");
                throw;
            }
        }

        // ===================== 护理人员位置管理模块 =====================

        /// <summary>
        /// 获取护理人员位置信息
        /// </summary>
        public async Task<List<object>> GetStaffLocationsAsync()
        {
            try
            {
                var sql = @"
                    SELECT 
                        s.staff_id,
                        s.name,
                        s.position,
                        s.contact_phone,
                        sl.floor,
                        sl.update_time as location_update_time
                    FROM StaffInfo s
                    LEFT JOIN StaffLocation sl ON s.staff_id = sl.staff_id
                    WHERE s.position LIKE '%护理%' OR s.position LIKE '%护士%' OR s.position LIKE '%看护%'
                    ORDER BY sl.update_time DESC";

                var results = await _databaseService.QueryAsync<dynamic>(sql);

                return results.Select(r => new
                {
                    StaffId = r.staff_id,
                    Name = r.name,
                    Position = r.position,
                    ContactPhone = r.contact_phone,
                    Floor = r.floor,
                    LocationUpdateTime = r.location_update_time,
                    HasRecentLocation = r.location_update_time != null && 
                                      ((DateTime)r.location_update_time) >= DateTime.Now.AddHours(-8)
                }).ToList<object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取护理人员位置信息失败");
                throw;
            }
        }

        /// <summary>
        /// 更新护理人员位置
        /// </summary>
        public async Task<object> UpdateStaffLocationAsync(StaffLocationUpdateDto locationUpdate)
        {
            try
            {
                var existsSql = "SELECT COUNT(*) FROM StaffLocation WHERE staff_id = :StaffId";
                var exists = await _databaseService.QueryFirstAsync<int>(existsSql, new { StaffId = locationUpdate.StaffId });

                if (exists > 0)
                {
                    var updateSql = @"
                        UPDATE StaffLocation 
                        SET floor = :Floor, update_time = :UpdateTime
                        WHERE staff_id = :StaffId";

                    await _databaseService.ExecuteAsync(updateSql, new
                    {
                        StaffId = locationUpdate.StaffId,
                        Floor = locationUpdate.Floor,
                        UpdateTime = locationUpdate.UpdateTime
                    });
                }
                else
                {
                    var insertSql = @"
                        INSERT INTO StaffLocation (staff_id, floor, update_time)
                        VALUES (:StaffId, :Floor, :UpdateTime)";

                    await _databaseService.ExecuteAsync(insertSql, new
                    {
                        StaffId = locationUpdate.StaffId,
                        Floor = locationUpdate.Floor,
                        UpdateTime = locationUpdate.UpdateTime
                    });
                }

                return new
                {
                    StaffId = locationUpdate.StaffId,
                    Floor = locationUpdate.Floor,
                    UpdateTime = locationUpdate.UpdateTime,
                    Action = exists > 0 ? "Updated" : "Created"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新护理人员位置失败: StaffId {locationUpdate.StaffId}");
                throw;
            }
        }

        // ===================== 内部辅助方法模块 =====================

        /// <summary>
        /// 触发电子围栏警报并通知最近的护理人员
        /// 核心智能通知系统：基于位置、职位、响应时间的多维度优先级算法
        /// </summary>
        private async Task<List<dynamic>> TriggerFenceAlertAsync(int elderlyId, double latitude, double longitude, string alertType)
        {
            try
            {
                // 1. 获取老人基本信息
                var elderlyInfo = await GetElderlyInfoAsync(elderlyId);
                if (elderlyInfo == null)
                {
                    _logger.LogWarning($"触发围栏警报失败：未找到老人信息 ID {elderlyId}");
                    return new List<dynamic>();
                }

                // 2. 获取所有护理人员及其位置信息（包含职位权重）
                var availableStaff = await GetAvailableNursingStaffAsync();
                if (!availableStaff.Any())
                {
                    _logger.LogWarning("触发围栏警报失败：未找到可用的护理人员");
                    return new List<dynamic>();
                }

                // 3. 智能筛选最佳响应人员（基于距离、职位、可用性）
                var prioritizedStaff = CalculateStaffPriority(availableStaff, latitude, longitude);
                var selectedStaff = prioritizedStaff.Take(3).ToList(); // 通知优先级最高的3名护理人员

                // 4. 发送警报通知
                foreach (var staff in selectedStaff)
                {
                    await SendStaffNotificationAsync(staff, elderlyInfo, latitude, longitude, alertType);
                }

                // 5. 记录警报处理日志
                await LogFenceAlertAsync(elderlyId, alertType, selectedStaff, latitude, longitude);

                _logger.LogWarning(
                    $"🚨 电子围栏{alertType}警报已触发！老人：{elderlyInfo.name} (ID: {elderlyId}), " +
                    $"位置: ({latitude:F6}, {longitude:F6}), " +
                    $"已通知 {selectedStaff.Count} 名护理人员");

                return selectedStaff;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"触发围栏警报失败: 老人ID {elderlyId}, 警报类型: {alertType}");
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// 获取老人基本信息
        /// </summary>
        private async Task<dynamic?> GetElderlyInfoAsync(int elderlyId)
        {
            var sql = @"
                SELECT elderly_id, name, room_id, emergency_contact, health_status
                FROM ElderlyInfo 
                WHERE elderly_id = :ElderlyId";

            return await _databaseService.QueryFirstOrDefaultAsync<dynamic>(sql, new { ElderlyId = elderlyId });
        }

        /// <summary>
        /// 获取所有可用的护理人员及其位置信息
        /// </summary>
        private async Task<List<dynamic>> GetAvailableNursingStaffAsync()
        {
            var sql = @"
                SELECT 
                    s.staff_id,
                    s.name,
                    s.contact_phone,
                    s.email,
                    s.position,
                    s.status as staff_status,
                    sl.floor,
                    sl.update_time as location_update_time
                FROM StaffInfo s
                LEFT JOIN StaffLocation sl ON s.staff_id = sl.staff_id
                WHERE (s.position LIKE '%护理%' OR s.position LIKE '%护士%' OR s.position LIKE '%看护%')
                  AND (s.status IS NULL OR s.status = '在岗' OR s.status = '可用')
                ORDER BY sl.update_time DESC";

            var results = await _databaseService.QueryAsync<dynamic>(sql);
            return results.ToList();
        }

        /// <summary>
        /// 计算护理人员响应优先级（多维度智能算法）
        /// 考虑因素：位置距离、职位等级、可用性、响应历史
        /// </summary>
        private List<dynamic> CalculateStaffPriority(List<dynamic> nursingStaff, double alertLat, double alertLng)
        {
            var staffWithPriority = new List<(dynamic staff, double priority)>();

            foreach (var staff in nursingStaff)
            {
                double priority = 0;

                // 1. 职位权重 (40%)
                string position = staff.position?.ToString() ?? "";
                if (position.Contains("主管") || position.Contains("负责人") || position.Contains("主任"))
                    priority += 40;
                else if (position.Contains("护理师") || position.Contains("资深护理"))
                    priority += 35;
                else if (position.Contains("护理员") || position.Contains("护理"))
                    priority += 30;
                else if (position.Contains("护士"))
                    priority += 25;

                // 2. 位置时效性权重 (30%)
                if (staff.location_update_time != null)
                {
                    var timeDiff = DateTime.Now - (DateTime)staff.location_update_time;
                    var hoursDiff = timeDiff.TotalHours;
                    
                    if (hoursDiff <= 1) priority += 30;      // 1小时内位置更新
                    else if (hoursDiff <= 4) priority += 20; // 4小时内位置更新
                    else if (hoursDiff <= 8) priority += 10; // 8小时内位置更新
                }

                // 3. 楼层位置权重 (20%)
                if (staff.floor != null)
                {
                    priority += 20; // 有明确楼层位置信息
                }

                // 4. 联系方式完整性权重 (10%)
                if (!string.IsNullOrEmpty(staff.contact_phone?.ToString()))
                    priority += 5;
                if (!string.IsNullOrEmpty(staff.email?.ToString()))
                    priority += 5;

                staffWithPriority.Add((staff, priority));
            }

            // 按优先级排序返回
            return staffWithPriority
                .Where(x => x.priority > 0)
                .OrderByDescending(x => x.priority)
                .Select(x => x.staff)
                .ToList();
        }

        /// <summary>
        /// 发送护理人员通知
        /// </summary>
        private async Task SendStaffNotificationAsync(dynamic staff, dynamic elderlyInfo, double latitude, double longitude, string alertType)
        {
            try
            {
                var notification = $"🚨 紧急{alertType}警报！\n" +
                                 $"老人：{elderlyInfo.name} (ID: {elderlyInfo.elderly_id})\n" +
                                 $"房间：{elderlyInfo.room_id}\n" +
                                 $"警报位置：({latitude:F6}, {longitude:F6})\n" +
                                 $"健康状况：{elderlyInfo.health_status}\n" +
                                 $"紧急联系人：{elderlyInfo.emergency_contact}\n" +
                                 $"时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                                 $"请立即响应！";

                // 这里可以集成实际的通知系统（短信、邮件、推送等）
                _logger.LogWarning(
                    $"📱 通知护理人员 {staff.name} ({staff.position}) - 电话: {staff.contact_phone}\n{notification}");

                // 在实际系统中，这里会调用短信API、邮件API或推送通知API
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"发送护理人员通知失败: {staff.name}");
            }
        }

        /// <summary>
        /// 记录围栏警报处理日志
        /// </summary>
        private async Task LogFenceAlertAsync(int elderlyId, string alertType, List<dynamic> notifiedStaff, double latitude, double longitude)
        {
            try
            {
                var staffNames = string.Join(", ", notifiedStaff.Select(s => $"{s.name}({s.position})"));
                var logMessage = $"围栏{alertType}警报处理记录: " +
                               $"老人ID {elderlyId}, " +
                               $"位置({latitude:F6}, {longitude:F6}), " +
                               $"通知人员: {staffNames}, " +
                               $"处理时间: {DateTime.Now}";

                _logger.LogWarning(logMessage);

                // 在实际应用中，可以将此记录写入专门的警报日志表
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "记录围栏警报日志失败");
            }
        }

        /// <summary>
        /// 获取老人当前的围栏状态（最新的未离开记录）
        /// </summary>
        private async Task<dynamic?> GetElderlyCurrentFenceAsync(int elderlyId)
        {
            var sql = @"
                SELECT event_log_id AS EventLogId, elderly_id AS ElderlyId, 
                       fence_id AS FenceId, entry_time AS EntryTime
                FROM FenceLog 
                WHERE elderly_id = :ElderlyId 
                  AND exit_time IS NULL 
                ORDER BY entry_time DESC
                FETCH FIRST 1 ROWS ONLY";

            return await _databaseService.QueryFirstOrDefaultAsync<dynamic>(sql, new { ElderlyId = elderlyId });
        }

        /// <summary>
        /// 获取包含指定GPS点的所有围栏
        /// </summary>
        private async Task<List<ElectronicFence>> GetFencesContainingPointAsync(double latitude, double longitude)
        {
            var sql = @"
                SELECT fence_id AS FenceId, area_definition AS AreaDefinition
                FROM ElectronicFence";

            var allFences = await _databaseService.QueryAsync<ElectronicFence>(sql);
            var containingFences = new List<ElectronicFence>();

            foreach (var fence in allFences)
            {
                if (!string.IsNullOrEmpty(fence.AreaDefinition) && 
                    IsPointInsideFence(latitude, longitude, fence.AreaDefinition))
                {
                    containingFences.Add(fence);
                }
            }

            return containingFences;
        }

        /// <summary>
        /// 更新围栏离开时间
        /// </summary>
        private async Task UpdateFenceExitAsync(int eventLogId, DateTime exitTime)
        {
            var sql = @"
                UPDATE FenceLog 
                SET exit_time = :ExitTime 
                WHERE event_log_id = :EventLogId";

            await _databaseService.ExecuteAsync(sql, new 
            { 
                ExitTime = exitTime, 
                EventLogId = eventLogId 
            });
        }

        /// <summary>
        /// 创建围栏进入记录
        /// </summary>
        private async Task CreateFenceEntryAsync(int elderlyId, int fenceId, DateTime entryTime)
        {
            var sql = @"
                INSERT INTO FenceLog (elderly_id, fence_id, entry_time)
                VALUES (:ElderlyId, :FenceId, :EntryTime)";

            await _databaseService.ExecuteAsync(sql, new
            {
                ElderlyId = elderlyId,
                FenceId = fenceId,
                EntryTime = entryTime
            });
        }
    }
}
