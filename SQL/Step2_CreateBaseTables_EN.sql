-- 智慧养老系统 - 第2步：创建基础表 (英文版本以避免编码问题)

-- 1. 房间管理表
CREATE TABLE RoomManagement (
    room_id NUMBER PRIMARY KEY,
    room_number VARCHAR2(20) NOT NULL UNIQUE,
    room_type VARCHAR2(50) NOT NULL,
    capacity NUMBER NOT NULL,
    status VARCHAR2(20) NOT NULL DEFAULT 'Available',
    rate DECIMAL(10,2) NOT NULL,
    bed_type VARCHAR2(50) NOT NULL,
    floor_num NUMBER NOT NULL,
    created_time DATE DEFAULT SYSDATE,
    updated_time DATE DEFAULT SYSDATE,
    CONSTRAINT chk_room_status CHECK (status IN ('Available', 'Occupied', 'Maintenance', 'Cleaning')),
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
    CONSTRAINT chk_gender CHECK (gender IN ('Male', 'Female'))
);

-- 3. 设备状态表
CREATE TABLE DeviceStatus (
    device_id NUMBER PRIMARY KEY,
    device_name VARCHAR2(100) NOT NULL,
    device_type VARCHAR2(50) NOT NULL,
    installation_date DATE NOT NULL,
    status VARCHAR2(20) NOT NULL DEFAULT 'Normal',
    last_maintenance_date DATE,
    maintenance_status VARCHAR2(20),
    location VARCHAR2(100) NOT NULL,
    created_time DATE DEFAULT SYSDATE,
    updated_time DATE DEFAULT SYSDATE,
    CONSTRAINT chk_device_status CHECK (status IN ('Normal', 'Fault', 'Maintenance', 'Deactivated')),
    CONSTRAINT chk_maintenance_status CHECK (maintenance_status IN ('Normal', 'Need_Maintenance', 'In_Maintenance', 'Maintained'))
);

-- 4. 健康监测记录表 (HealthMonitoring)
CREATE TABLE HealthMonitoring (
    monitoring_id NUMBER PRIMARY KEY,
    elderly_id NUMBER NOT NULL,
    monitoring_date DATE NOT NULL,
    heart_rate NUMBER,
    blood_pressure VARCHAR2(20),
    oxygen_level NUMBER(5,2),
    temperature NUMBER(4,1),
    status VARCHAR2(20) NOT NULL DEFAULT 'Normal',
    created_time DATE DEFAULT SYSDATE,
    CONSTRAINT fk_health_elderly FOREIGN KEY (elderly_id) REFERENCES ElderlyInfo(elderly_id) ON DELETE CASCADE,
    CONSTRAINT chk_monitoring_status CHECK (status IN ('Normal', 'Abnormal', 'Critical')),
    CONSTRAINT chk_heart_rate CHECK (heart_rate IS NULL OR heart_rate BETWEEN 40 AND 200),
    CONSTRAINT chk_oxygen_level CHECK (oxygen_level IS NULL OR oxygen_level BETWEEN 0 AND 100),
    CONSTRAINT chk_temperature CHECK (temperature IS NULL OR temperature BETWEEN 30 AND 45)
);

-- 5. 电子围栏信息表 (ElectronicFence)
CREATE TABLE ElectronicFence (
    fence_id NUMBER PRIMARY KEY,
    area_definition CLOB NOT NULL,
    fence_name VARCHAR2(100),
    fence_type VARCHAR2(50),
    status VARCHAR2(20) DEFAULT 'Enabled',
    created_time DATE DEFAULT SYSDATE,
    updated_time DATE DEFAULT SYSDATE,
    CONSTRAINT chk_fence_status CHECK (status IN ('Enabled', 'Disabled', 'Maintenance'))
);

-- 6. 电子围栏出入记录表 (FenceLog)
CREATE TABLE FenceLog (
    event_log_id NUMBER PRIMARY KEY,
    elderly_id NUMBER NOT NULL,
    fence_id NUMBER NOT NULL,
    entry_time DATE NOT NULL,
    exit_time DATE,
    created_time DATE DEFAULT SYSDATE,
    CONSTRAINT fk_log_elderly FOREIGN KEY (elderly_id) REFERENCES ElderlyInfo(elderly_id) ON DELETE CASCADE,
    CONSTRAINT fk_log_fence FOREIGN KEY (fence_id) REFERENCES ElectronicFence(fence_id) ON DELETE CASCADE
);

COMMIT;
