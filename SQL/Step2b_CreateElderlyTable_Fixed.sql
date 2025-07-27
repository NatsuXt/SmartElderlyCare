-- 简化版本 - 创建 ElderlyInfo 表 (兼容版本)

-- 第一步：创建基本表结构（不使用DEFAULT值）
CREATE TABLE ElderlyInfo (
    elderly_id NUMBER PRIMARY KEY,
    name VARCHAR2(100) NOT NULL,
    gender VARCHAR2(10) NOT NULL,
    birth_date DATE NOT NULL,
    id_card_number VARCHAR2(18) NOT NULL UNIQUE,
    contact_phone VARCHAR2(20),
    address VARCHAR2(200),
    emergency_contact VARCHAR2(200),
    created_time DATE,
    updated_time DATE
);

-- 第二步：添加约束
ALTER TABLE ElderlyInfo ADD CONSTRAINT chk_gender CHECK (gender IN ('Male', 'Female'));

-- 第三步：修改列添加默认值
ALTER TABLE ElderlyInfo MODIFY created_time DEFAULT SYSDATE;
ALTER TABLE ElderlyInfo MODIFY updated_time DEFAULT SYSDATE;

-- 验证表创建成功
SELECT table_name FROM user_tables WHERE table_name = 'ELDERLYINFO';

COMMIT;
