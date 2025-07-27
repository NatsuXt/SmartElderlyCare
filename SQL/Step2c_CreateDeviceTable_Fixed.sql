-- 简化版本 - 创建 DeviceStatus 表 (兼容版本)

-- 第一步：创建基本表结构（不使用DEFAULT值）
CREATE TABLE DeviceStatus (
    device_id NUMBER PRIMARY KEY,
    device_name VARCHAR2(100) NOT NULL,
    device_type VARCHAR2(50) NOT NULL,
    installation_date DATE NOT NULL,
    status VARCHAR2(20) NOT NULL,
    last_maintenance_date DATE,
    maintenance_status VARCHAR2(20),
    location VARCHAR2(100) NOT NULL,
    created_time DATE,
    updated_time DATE
);

-- 第二步：添加约束
ALTER TABLE DeviceStatus ADD CONSTRAINT chk_device_status CHECK (status IN ('Normal', 'Fault', 'Maintenance', 'Deactivated'));
ALTER TABLE DeviceStatus ADD CONSTRAINT chk_maintenance_status CHECK (maintenance_status IN ('Normal', 'Need_Maintenance', 'In_Maintenance', 'Maintained'));

-- 第三步：修改列添加默认值
ALTER TABLE DeviceStatus MODIFY status DEFAULT 'Normal';
ALTER TABLE DeviceStatus MODIFY created_time DEFAULT SYSDATE;
ALTER TABLE DeviceStatus MODIFY updated_time DEFAULT SYSDATE;

-- 验证表创建成功
SELECT table_name FROM user_tables WHERE table_name = 'DEVICESTATUS';

COMMIT;
