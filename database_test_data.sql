-- 智慧养老系统测试数据 SQL INSERT 语句
-- 生成日期: 2025年9月12日
-- 包含表: DeviceStatus, RoomManagement, RoomOccupancy

-- ========================================
-- 1. RoomManagement 表数据 (房间管理)
-- ========================================
-- 字段: room_id(自增), room_number, room_type, capacity, status, rate, bed_type, floor

INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('101', 'Single Room', 1, 'Occupied', 1500.00, 'Single Bed', 1);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('102', 'Single Room', 1, 'Occupied', 1500.00, 'Single Bed', 1);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('103', 'Double Room', 2, 'Occupied', 2200.00, 'Twin Beds', 1);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('104', 'Single Room', 1, 'Available', 1500.00, 'Single Bed', 1);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('105', 'Double Room', 2, 'Occupied', 2200.00, 'Twin Beds', 1);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('201', 'Single Room', 1, 'Occupied', 1600.00, 'Single Bed', 2);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('202', 'Triple Room', 3, 'Occupied', 3000.00, 'Triple Beds', 2);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('203', 'Single Room', 1, 'Occupied', 1600.00, 'Single Bed', 2);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('204', 'Double Room', 2, 'Available', 2300.00, 'Twin Beds', 2);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('205', 'Single Room', 1, 'Occupied', 1600.00, 'Single Bed', 2);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('301', 'VIP Suite', 1, 'Occupied', 2500.00, 'Queen Bed', 3);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('302', 'Double Room', 2, 'Occupied', 2400.00, 'Twin Beds', 3);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('303', 'Single Room', 1, 'Available', 1700.00, 'Single Bed', 3);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('304', 'VIP Suite', 1, 'Occupied', 2500.00, 'Queen Bed', 3);
INSERT INTO RoomManagement (room_number, room_type, capacity, status, rate, bed_type, floor) VALUES ('305', 'Double Room', 2, 'Occupied', 2400.00, 'Twin Beds', 3);

-- ========================================
-- 2. DeviceStatus 表数据 (设备状态)
-- ========================================
-- 字段: device_id(自增), device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location

INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 101', 'Smart Mattress', TO_DATE('2024-01-15', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-15', 'YYYY-MM-DD'), 'Good', 'Room 101');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Heart Rate Monitor 101', 'Heart Rate Monitor', TO_DATE('2024-01-15', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-20', 'YYYY-MM-DD'), 'Good', 'Room 101');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 102', 'Smart Mattress', TO_DATE('2024-01-20', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-18', 'YYYY-MM-DD'), 'Good', 'Room 102');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Blood Pressure Monitor 102', 'Blood Pressure Monitor', TO_DATE('2024-01-20', 'YYYY-MM-DD'), 'Faulty', TO_DATE('2024-07-10', 'YYYY-MM-DD'), 'Needs Repair', 'Room 102');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 103A', 'Smart Mattress', TO_DATE('2024-02-01', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-25', 'YYYY-MM-DD'), 'Good', 'Room 103 Bed A');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 103B', 'Smart Mattress', TO_DATE('2024-02-01', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-25', 'YYYY-MM-DD'), 'Good', 'Room 103 Bed B');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Emergency Button 103', 'Emergency Alert', TO_DATE('2024-02-01', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-30', 'YYYY-MM-DD'), 'Good', 'Room 103');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 105A', 'Smart Mattress', TO_DATE('2024-02-10', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-28', 'YYYY-MM-DD'), 'Good', 'Room 105 Bed A');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Temperature Sensor 201', 'Temperature Monitor', TO_DATE('2024-02-15', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-12', 'YYYY-MM-DD'), 'Good', 'Room 201');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Motion Detector 201', 'Motion Sensor', TO_DATE('2024-02-15', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-12', 'YYYY-MM-DD'), 'Good', 'Room 201');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 202A', 'Smart Mattress', TO_DATE('2024-02-20', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-22', 'YYYY-MM-DD'), 'Good', 'Room 202 Bed A');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 202B', 'Smart Mattress', TO_DATE('2024-02-20', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-22', 'YYYY-MM-DD'), 'Good', 'Room 202 Bed B');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 202C', 'Smart Mattress', TO_DATE('2024-02-20', 'YYYY-MM-DD'), 'Maintenance', TO_DATE('2024-09-01', 'YYYY-MM-DD'), 'Under Maintenance', 'Room 202 Bed C');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Air Quality Monitor 203', 'Air Quality Sensor', TO_DATE('2024-03-01', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-15', 'YYYY-MM-DD'), 'Good', 'Room 203');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 205', 'Smart Mattress', TO_DATE('2024-03-05', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-18', 'YYYY-MM-DD'), 'Good', 'Room 205');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Fall Detection Sensor 301', 'Fall Detection', TO_DATE('2024-03-10', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-25', 'YYYY-MM-DD'), 'Good', 'Room 301');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 301', 'Smart Mattress', TO_DATE('2024-03-10', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-25', 'YYYY-MM-DD'), 'Good', 'Room 301');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 302A', 'Smart Mattress', TO_DATE('2024-03-15', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-20', 'YYYY-MM-DD'), 'Good', 'Room 302 Bed A');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 302B', 'Smart Mattress', TO_DATE('2024-03-15', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-20', 'YYYY-MM-DD'), 'Good', 'Room 302 Bed B');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Sleep Monitor 304', 'Sleep Quality Monitor', TO_DATE('2024-03-20', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-28', 'YYYY-MM-DD'), 'Good', 'Room 304');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 304', 'Smart Mattress', TO_DATE('2024-03-20', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-08-28', 'YYYY-MM-DD'), 'Good', 'Room 304');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 305A', 'Smart Mattress', TO_DATE('2024-03-25', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-09-05', 'YYYY-MM-DD'), 'Good', 'Room 305 Bed A');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Smart Bed Sensor 305B', 'Smart Mattress', TO_DATE('2024-03-25', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-09-05', 'YYYY-MM-DD'), 'Good', 'Room 305 Bed B');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Central Monitoring Hub', 'Monitoring System', TO_DATE('2024-01-01', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-09-01', 'YYYY-MM-DD'), 'Good', 'Nursing Station');
INSERT INTO DeviceStatus (device_name, device_type, installation_date, status, last_maintenance_date, maintenance_status, location) VALUES ('Backup Power System', 'Power Supply', TO_DATE('2024-01-01', 'YYYY-MM-DD'), 'Normal', TO_DATE('2024-09-01', 'YYYY-MM-DD'), 'Good', 'Equipment Room');

-- ========================================
-- 3. RoomOccupancy 表数据 (房间入住信息)
-- ========================================
-- 字段: occupancy_id(自增), room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date
-- 注意: elderly_id 对应已有的老人数据 (101-115)，room_id 对应已有的房间数据 (217-231)

INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (217, 101, TO_DATE('2024-06-01', 'YYYY-MM-DD'), NULL, 'Checked In', '1', 'Patricia Davis - Stable condition', TO_DATE('2024-06-01', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (218, 102, TO_DATE('2024-06-15', 'YYYY-MM-DD'), NULL, 'Checked In', '1', 'John Smith - Requires daily medication', TO_DATE('2024-06-15', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (219, 103, TO_DATE('2024-07-01', 'YYYY-MM-DD'), NULL, 'Checked In', 'A', 'Mary Johnson - Mobility assistance needed', TO_DATE('2024-07-01', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (219, 104, TO_DATE('2024-07-01', 'YYYY-MM-DD'), NULL, 'Checked In', 'B', 'Robert Brown - Diet restrictions', TO_DATE('2024-07-01', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (221, 105, TO_DATE('2024-07-15', 'YYYY-MM-DD'), NULL, 'Checked In', 'A', 'Jennifer Wilson - Regular checkups', TO_DATE('2024-07-15', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (222, 106, TO_DATE('2024-08-01', 'YYYY-MM-DD'), NULL, 'Checked In', '1', 'Michael Jones - Physical therapy', TO_DATE('2024-08-01', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (223, 107, TO_DATE('2024-08-05', 'YYYY-MM-DD'), NULL, 'Checked In', 'A', 'Susan Miller - Memory care', TO_DATE('2024-08-05', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (223, 108, TO_DATE('2024-08-05', 'YYYY-MM-DD'), NULL, 'Checked In', 'B', 'David Anderson - Cardiac monitoring', TO_DATE('2024-08-05', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (223, 109, TO_DATE('2024-08-05', 'YYYY-MM-DD'), NULL, 'Checked In', 'C', 'Lisa Taylor - Diabetes management', TO_DATE('2024-08-05', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (224, 110, TO_DATE('2024-08-10', 'YYYY-MM-DD'), NULL, 'Checked In', '1', 'William Thomas - Blood pressure monitoring', TO_DATE('2024-08-10', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (226, 111, TO_DATE('2024-08-20', 'YYYY-MM-DD'), NULL, 'Checked In', '1', 'Karen Martinez - Joint pain management', TO_DATE('2024-08-20', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (227, 112, TO_DATE('2024-08-25', 'YYYY-MM-DD'), NULL, 'Checked In', '1', 'Richard Garcia - Respiratory therapy', TO_DATE('2024-08-25', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (228, 113, TO_DATE('2024-09-01', 'YYYY-MM-DD'), NULL, 'Checked In', 'A', 'Betty Robinson - Fall prevention', TO_DATE('2024-09-01', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (228, 114, TO_DATE('2024-09-01', 'YYYY-MM-DD'), NULL, 'Checked In', 'B', 'James Lee - Medication supervision', TO_DATE('2024-09-01', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (230, 115, TO_DATE('2024-09-05', 'YYYY-MM-DD'), NULL, 'Checked In', '1', 'Nancy Lewis - Routine care', TO_DATE('2024-09-05', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (231, 0, TO_DATE('2024-09-10', 'YYYY-MM-DD'), NULL, 'Checked In', 'A', 'Test elderly - System testing purposes', TO_DATE('2024-09-10', 'YYYY-MM-DD'), TO_DATE('2024-09-12', 'YYYY-MM-DD'));

-- ========================================
-- 历史入住记录 (已退房的记录)
-- ========================================
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (220, 101, TO_DATE('2024-03-01', 'YYYY-MM-DD'), TO_DATE('2024-05-30', 'YYYY-MM-DD'), 'Checked Out', '1', 'Patricia Davis - Temporary stay completed', TO_DATE('2024-03-01', 'YYYY-MM-DD'), TO_DATE('2024-05-30', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (225, 102, TO_DATE('2024-04-01', 'YYYY-MM-DD'), TO_DATE('2024-06-10', 'YYYY-MM-DD'), 'Checked Out', 'A', 'John Smith - Transferred to current room', TO_DATE('2024-04-01', 'YYYY-MM-DD'), TO_DATE('2024-06-10', 'YYYY-MM-DD'));
INSERT INTO RoomOccupancy (room_id, elderly_id, check_in_date, check_out_date, status, bed_number, remarks, created_date, updated_date) VALUES (229, 103, TO_DATE('2024-05-01', 'YYYY-MM-DD'), TO_DATE('2024-06-25', 'YYYY-MM-DD'), 'Checked Out', '1', 'Mary Johnson - Room upgrade', TO_DATE('2024-05-01', 'YYYY-MM-DD'), TO_DATE('2024-06-25', 'YYYY-MM-DD'));

-- ========================================
-- 数据验证查询 (可选执行)
-- ========================================
/*
-- 验证房间数据
SELECT room_id, room_number, room_type, capacity, status, rate FROM RoomManagement ORDER BY room_id;

-- 验证设备数据
SELECT device_id, device_name, device_type, status, location FROM DeviceStatus ORDER BY device_id;

-- 验证入住数据（当前入住）
SELECT ro.occupancy_id, rm.room_number, ro.elderly_id, ro.check_in_date, ro.status, ro.bed_number 
FROM RoomOccupancy ro
JOIN RoomManagement rm ON ro.room_id = rm.room_id
WHERE ro.status = 'Checked In'
ORDER BY ro.occupancy_id;

-- 统计信息
SELECT 
    (SELECT COUNT(*) FROM RoomManagement) as total_rooms,
    (SELECT COUNT(*) FROM RoomManagement WHERE status = 'Occupied') as occupied_rooms,
    (SELECT COUNT(*) FROM DeviceStatus) as total_devices,
    (SELECT COUNT(*) FROM DeviceStatus WHERE status = 'Normal') as normal_devices,
    (SELECT COUNT(*) FROM RoomOccupancy WHERE status = 'Checked In') as current_residents
FROM DUAL;
*/

-- 执行完成提示
SELECT 'Test data insertion completed successfully!' as MESSAGE FROM DUAL;