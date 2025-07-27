-- 智慧养老系统测试数据插入脚本 (英文版本)
-- 
-- 注意：请确保所有表都已创建成功后再执行此脚本
-- 执行顺序：先插入基础表数据，再插入带外键的表数据

-- 插入房间管理测试数据
INSERT INTO RoomManagement (room_id, room_number, room_type, capacity, status, rate, bed_type, floor_num, created_time, updated_time) VALUES
(1001, '101', 'Single Room', 1, 'Available', 3000.00, 'Single Bed', 1, SYSDATE, SYSDATE);

INSERT INTO RoomManagement (room_id, room_number, room_type, capacity, status, rate, bed_type, floor_num, created_time, updated_time) VALUES
(1002, '102', 'Double Room', 2, 'Occupied', 5000.00, 'Double Bed', 1, SYSDATE, SYSDATE);

INSERT INTO RoomManagement (room_id, room_number, room_type, capacity, status, rate, bed_type, floor_num, created_time, updated_time) VALUES
(1003, '201', 'Suite', 3, 'Available', 8000.00, 'Double Bed', 2, SYSDATE, SYSDATE);

INSERT INTO RoomManagement (room_id, room_number, room_type, capacity, status, rate, bed_type, floor_num, created_time, updated_time) VALUES
(1004, '202', 'Care Room', 1, 'Maintenance', 4000.00, 'Medical Bed', 2, SYSDATE, SYSDATE);

-- 插入老人信息测试数据
INSERT INTO ElderlyInfo (elderly_id, name, gender, birth_date, id_card_number, contact_phone, address, emergency_contact, created_time, updated_time) VALUES
(3001, '张三', 'Male', DATE '1945-03-15', '110101194503151234', '13800138001', '北京市朝阳区', '张小明(儿子):13900139001', SYSDATE, SYSDATE);

INSERT INTO ElderlyInfo (elderly_id, name, gender, birth_date, id_card_number, contact_phone, address, emergency_contact, created_time, updated_time) VALUES
(3002, '李四', 'Female', DATE '1950-08-20', '110101195008201234', '13800138002', '北京市海淀区', '李小红(女儿):13900139002', SYSDATE, SYSDATE);

INSERT INTO ElderlyInfo (elderly_id, name, gender, birth_date, id_card_number, contact_phone, address, emergency_contact, created_time, updated_time) VALUES
(3003, '王五', 'Male', DATE '1948-12-10', '110101194812101234', '13800138003', '北京市西城区', '王小军(儿子):13900139003', SYSDATE, SYSDATE);

INSERT INTO ElderlyInfo (elderly_id, name, gender, birth_date, id_card_number, contact_phone, address, emergency_contact, created_time, updated_time) VALUES
(3004, '赵六', 'Female', DATE '1952-05-25', '110101195205251234', '13800138004', '北京市东城区', '赵小丽(女儿):13900139004', SYSDATE, SYSDATE);

-- 插入设备状态测试数据
INSERT INTO DeviceStatus (device_id, device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location, created_time, updated_time) VALUES
(2001, '智能床垫001', 'Smart Mattress', DATE '2024-01-15', 'Normal', DATE '2024-06-15', 'Normal', '101房间', SYSDATE, SYSDATE);

INSERT INTO DeviceStatus (device_id, device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location, created_time, updated_time) VALUES
(2002, '心率监测仪001', 'Heart Rate Monitor', DATE '2024-02-20', 'Normal', DATE '2024-07-20', 'Normal', '102房间', SYSDATE, SYSDATE);

INSERT INTO DeviceStatus (device_id, device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location, created_time, updated_time) VALUES
(2003, '紧急呼叫器001', 'Emergency Call Button', DATE '2024-03-10', 'Fault', DATE '2024-06-10', 'Need_Maintenance', '201房间', SYSDATE, SYSDATE);

INSERT INTO DeviceStatus (device_id, device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location, created_time, updated_time) VALUES
(2004, '环境监测器001', 'Environment Monitor', DATE '2024-04-05', 'Maintenance', DATE '2024-07-25', 'In_Maintenance', '202房间', SYSDATE, SYSDATE);

-- 插入电子围栏测试数据
INSERT INTO ElectronicFence (fence_id, area_definition, fence_name, fence_type, status, created_time, updated_time) VALUES
(5001, '{"coordinates": [[116.3974, 39.9093], [116.3984, 39.9103], [116.3994, 39.9093], [116.3984, 39.9083]]}', '一楼活动区域', 'Activity Area', 'Enabled', SYSDATE, SYSDATE);

INSERT INTO ElectronicFence (fence_id, area_definition, fence_name, fence_type, status, created_time, updated_time) VALUES
(5002, '{"coordinates": [[116.3964, 39.9083], [116.3974, 39.9093], [116.3984, 39.9083], [116.3974, 39.9073]]}', '花园区域', 'Garden Area', 'Enabled', SYSDATE, SYSDATE);

-- 插入健康监测测试数据 (依赖ElderlyInfo表)
INSERT INTO HealthMonitoring (monitoring_id, elderly_id, monitoring_date, heart_rate, blood_pressure, oxygen_level, temperature, status, created_time) VALUES
(4001, 3001, SYSDATE - 0.5, 75, '120/80', 98.5, 36.5, 'Normal', SYSDATE);

INSERT INTO HealthMonitoring (monitoring_id, elderly_id, monitoring_date, heart_rate, blood_pressure, oxygen_level, temperature, status, created_time) VALUES
(4002, 3002, SYSDATE - 0.3, 82, '130/85', 97.8, 36.8, 'Normal', SYSDATE);

INSERT INTO HealthMonitoring (monitoring_id, elderly_id, monitoring_date, heart_rate, blood_pressure, oxygen_level, temperature, status, created_time) VALUES
(4003, 3003, SYSDATE - 0.2, 95, '140/90', 96.5, 37.2, 'Abnormal', SYSDATE);

INSERT INTO HealthMonitoring (monitoring_id, elderly_id, monitoring_date, heart_rate, blood_pressure, oxygen_level, temperature, status, created_time) VALUES
(4004, 3004, SYSDATE - 0.1, 68, '110/70', 99.2, 36.3, 'Normal', SYSDATE);

INSERT INTO HealthMonitoring (monitoring_id, elderly_id, monitoring_date, heart_rate, blood_pressure, oxygen_level, temperature, status, created_time) VALUES
(4005, 3001, SYSDATE, 78, '125/82', 98.8, 36.6, 'Normal', SYSDATE);

-- 插入围栏出入记录测试数据 (依赖ElderlyInfo和ElectronicFence表)
INSERT INTO FenceLog (event_log_id, elderly_id, fence_id, entry_time, exit_time, created_time) VALUES
(6001, 3001, 5001, SYSDATE - 2/24, SYSDATE - 1.5/24, SYSDATE);

INSERT INTO FenceLog (event_log_id, elderly_id, fence_id, entry_time, exit_time, created_time) VALUES
(6002, 3002, 5002, SYSDATE - 1/24, NULL, SYSDATE);

-- 提交所有更改
COMMIT;

-- 验证插入结果
SELECT 'Data Insertion Results:' as info FROM dual;

SELECT 'RoomManagement' as table_name, COUNT(*) as record_count FROM RoomManagement
UNION ALL
SELECT 'ElderlyInfo' as table_name, COUNT(*) as record_count FROM ElderlyInfo
UNION ALL
SELECT 'DeviceStatus' as table_name, COUNT(*) as record_count FROM DeviceStatus
UNION ALL
SELECT 'HealthMonitoring' as table_name, COUNT(*) as record_count FROM HealthMonitoring
UNION ALL
SELECT 'ElectronicFence' as table_name, COUNT(*) as record_count FROM ElectronicFence
UNION ALL
SELECT 'FenceLog' as table_name, COUNT(*) as record_count FROM FenceLog
ORDER BY table_name;

SELECT 'Test data insertion completed successfully!' as result FROM dual;
