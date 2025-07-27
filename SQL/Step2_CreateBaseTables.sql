-- 智慧养老系统 - 第2步：创建基础表

-- 1. 房间管理表
CREATE TABLE RoomManagement (
    room_id NUMBER PRIMARY KEY,
    room_number VARCHAR2(20) NOT NULL UNIQUE,
    room_type VARCHAR2(50) NOT NULL,
    capacity NUMBER NOT NULL,
    status VARCHAR2(20) NOT NULL DEFAULT '空闲',
    rate DECIMAL(10,2) NOT NULL,
    bed_type VARCHAR2(50) NOT NULL,
    floor_num NUMBER NOT NULL,
    created_time DATE DEFAULT SYSDATE,
    updated_time DATE DEFAULT SYSDATE,
    CONSTRAINT chk_room_status CHECK (status IN ('空闲', '已入住', '维护中', '清洁中')),
    CONSTRAINT chk_capacity CHECK (capacity > 0),
    CONSTRAINT chk_rate CHECK (rate > 0),
    CONSTRAINT chk_floor CHECK (floor_num > 0)
);

-- 2. 老人基本信息表  
CREATE TABLE ElderlyInfo (
    elderly_id NUMBER PRIMARY KEY,
    name VARCHAR2(100) NOT NULL,
    gender VARCHAR2(10) NOT NULL,
    birth_date DATE NOT NULL,
    id_card_number VARCHAR2(18) NOT NULL UNIQUE,
    contact_phone VARCHAR2(20),
    address VARCHAR2(200),
    emergency_contact VARCHAR2(200),
    created_time DATE DEFAULT SYSDATE,
    updated_time DATE DEFAULT SYSDATE,
    CONSTRAINT chk_gender CHECK (gender IN ('男', '女'))
);

-- 3. 设备状态表
CREATE TABLE DeviceStatus (
    device_id NUMBER PRIMARY KEY,
    device_name VARCHAR2(100) NOT NULL,
    device_type VARCHAR2(50) NOT NULL,
    installation_date DATE NOT NULL,
    status VARCHAR2(20) NOT NULL DEFAULT '正常',
    last_maintenance_date DATE,
    maintenance_status VARCHAR2(20),
    location VARCHAR2(100) NOT NULL,
    created_time DATE DEFAULT SYSDATE,
    updated_time DATE DEFAULT SYSDATE,
    CONSTRAINT chk_device_status CHECK (status IN ('正常', '故障', '维护中', '停用')),
    CONSTRAINT chk_maintenance_status CHECK (maintenance_status IN ('正常', '需要维护', '维护中', '已维护'))
);

COMMIT;
