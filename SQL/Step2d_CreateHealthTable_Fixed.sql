-- 简化版本 - 创建 HealthMonitoring 表 (兼容版本)

-- 第一步：创建基本表结构（不使用DEFAULT值）
CREATE TABLE HealthMonitoring (
    monitoring_id NUMBER PRIMARY KEY,
    elderly_id NUMBER NOT NULL,
    monitoring_date DATE NOT NULL,
    heart_rate NUMBER,
    blood_pressure VARCHAR2(20),
    oxygen_level NUMBER(5,2),
    temperature NUMBER(4,1),
    status VARCHAR2(20) NOT NULL,
    created_time DATE
);

-- 第二步：添加外键约束
ALTER TABLE HealthMonitoring ADD CONSTRAINT fk_health_elderly FOREIGN KEY (elderly_id) REFERENCES ElderlyInfo(elderly_id) ON DELETE CASCADE;

-- 第三步：添加检查约束
ALTER TABLE HealthMonitoring ADD CONSTRAINT chk_monitoring_status CHECK (status IN ('Normal', 'Abnormal', 'Critical'));
ALTER TABLE HealthMonitoring ADD CONSTRAINT chk_heart_rate CHECK (heart_rate IS NULL OR heart_rate BETWEEN 40 AND 200);
ALTER TABLE HealthMonitoring ADD CONSTRAINT chk_oxygen_level CHECK (oxygen_level IS NULL OR oxygen_level BETWEEN 0 AND 100);
ALTER TABLE HealthMonitoring ADD CONSTRAINT chk_temperature CHECK (temperature IS NULL OR temperature BETWEEN 30 AND 45);

-- 第四步：修改列添加默认值
ALTER TABLE HealthMonitoring MODIFY status DEFAULT 'Normal';
ALTER TABLE HealthMonitoring MODIFY created_time DEFAULT SYSDATE;

-- 验证表创建成功
SELECT table_name FROM user_tables WHERE table_name = 'HEALTHMONITORING';

COMMIT;
