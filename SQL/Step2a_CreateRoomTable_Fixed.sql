-- 简化版本 - 创建 RoomManagement 表 (兼容版本)

-- 第一步：创建基本表结构（不使用DEFAULT值）
CREATE TABLE RoomManagement (
    room_id NUMBER PRIMARY KEY,
    room_number VARCHAR2(20) NOT NULL UNIQUE,
    room_type VARCHAR2(50) NOT NULL,
    capacity NUMBER NOT NULL,
    status VARCHAR2(20) NOT NULL,
    rate DECIMAL(10,2) NOT NULL,
    bed_type VARCHAR2(50) NOT NULL,
    floor_num NUMBER NOT NULL,
    created_time DATE,
    updated_time DATE
);

-- 第二步：添加约束
ALTER TABLE RoomManagement ADD CONSTRAINT chk_room_status CHECK (status IN ('Available', 'Occupied', 'Maintenance', 'Cleaning'));
ALTER TABLE RoomManagement ADD CONSTRAINT chk_capacity CHECK (capacity > 0);
ALTER TABLE RoomManagement ADD CONSTRAINT chk_rate CHECK (rate > 0);
ALTER TABLE RoomManagement ADD CONSTRAINT chk_floor CHECK (floor_num > 0);

-- 第三步：修改列添加默认值
ALTER TABLE RoomManagement MODIFY status DEFAULT 'Available';
ALTER TABLE RoomManagement MODIFY created_time DEFAULT SYSDATE;
ALTER TABLE RoomManagement MODIFY updated_time DEFAULT SYSDATE;

-- 验证表创建成功
SELECT table_name FROM user_tables WHERE table_name = 'ROOMMANAGEMENT';

COMMIT;
