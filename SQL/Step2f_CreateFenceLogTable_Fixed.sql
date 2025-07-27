-- 简化版本 - 创建 FenceLog 表 (兼容版本)

-- 第一步：创建基本表结构（不使用DEFAULT值）
CREATE TABLE FenceLog (
    event_log_id NUMBER PRIMARY KEY,
    elderly_id NUMBER NOT NULL,
    fence_id NUMBER NOT NULL,
    entry_time DATE NOT NULL,
    exit_time DATE,
    created_time DATE
);

-- 第二步：添加外键约束
ALTER TABLE FenceLog ADD CONSTRAINT fk_log_elderly FOREIGN KEY (elderly_id) REFERENCES ElderlyInfo(elderly_id) ON DELETE CASCADE;
ALTER TABLE FenceLog ADD CONSTRAINT fk_log_fence FOREIGN KEY (fence_id) REFERENCES ElectronicFence(fence_id) ON DELETE CASCADE;

-- 第三步：修改列添加默认值
ALTER TABLE FenceLog MODIFY created_time DEFAULT SYSDATE;

-- 验证表创建成功
SELECT table_name FROM user_tables WHERE table_name = 'FENCELOG';

COMMIT;
