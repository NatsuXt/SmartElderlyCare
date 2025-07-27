-- 智慧养老系统 - 第4步：创建序列和索引

-- 创建序列
CREATE SEQUENCE seq_room_id START WITH 1001 INCREMENT BY 1 NOCACHE;
CREATE SEQUENCE seq_device_id START WITH 2001 INCREMENT BY 1 NOCACHE;
CREATE SEQUENCE seq_elderly_id START WITH 3001 INCREMENT BY 1 NOCACHE;
CREATE SEQUENCE seq_health_id START WITH 4001 INCREMENT BY 1 NOCACHE;
CREATE SEQUENCE seq_fence_id START WITH 5001 INCREMENT BY 1 NOCACHE;
CREATE SEQUENCE seq_log_id START WITH 6001 INCREMENT BY 1 NOCACHE;

-- 创建索引
CREATE INDEX idx_room_status ON RoomManagement(status);
CREATE INDEX idx_room_floor ON RoomManagement(floor_num);
CREATE INDEX idx_room_type ON RoomManagement(room_type);
CREATE INDEX idx_device_status ON DeviceStatus(status);
CREATE INDEX idx_device_type ON DeviceStatus(device_type);
CREATE INDEX idx_device_location ON DeviceStatus(location);
CREATE INDEX idx_elderly_name ON ElderlyInfo(name);
CREATE INDEX idx_elderly_id_card ON ElderlyInfo(id_card_number);
CREATE INDEX idx_health_elderly ON HealthMonitoring(elderly_id);
CREATE INDEX idx_health_date ON HealthMonitoring(monitoring_date);
CREATE INDEX idx_health_status ON HealthMonitoring(status);
CREATE INDEX idx_fence_status ON ElectronicFence(status);
CREATE INDEX idx_log_elderly ON FenceLog(elderly_id);
CREATE INDEX idx_log_fence ON FenceLog(fence_id);
CREATE INDEX idx_log_entry_time ON FenceLog(entry_time);

COMMIT;
