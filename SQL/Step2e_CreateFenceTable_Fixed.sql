-- 简化版本 - 创建 ElectronicFence 表 (兼容版本)

-- 第一步：创建基本表结构（不使用DEFAULT值）
CREATE TABLE ElectronicFence (
    fence_id NUMBER PRIMARY KEY,
    area_definition CLOB NOT NULL,
    fence_name VARCHAR2(100),
    fence_type VARCHAR2(50),
    status VARCHAR2(20),
    created_time DATE,
    updated_time DATE
);

-- 第二步：添加约束
ALTER TABLE ElectronicFence ADD CONSTRAINT chk_fence_status CHECK (status IN ('Enabled', 'Disabled', 'Maintenance'));

-- 第三步：修改列添加默认值
ALTER TABLE ElectronicFence MODIFY status DEFAULT 'Enabled';
ALTER TABLE ElectronicFence MODIFY created_time DEFAULT SYSDATE;
ALTER TABLE ElectronicFence MODIFY updated_time DEFAULT SYSDATE;

-- 验证表创建成功
SELECT table_name FROM user_tables WHERE table_name = 'ELECTRONICFENCE';

COMMIT;
