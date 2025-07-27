-- 智慧养老系统 - 第6步：验证创建结果

-- 检查表是否创建成功
SELECT 'Tables Created:' as info FROM dual;
SELECT table_name FROM user_tables 
WHERE table_name IN ('ROOMMANAGEMENT', 'DEVICESTATUS', 'ELDERLYINFO', 'HEALTHMONITORING', 'ELECTRONICFENCE', 'FENCELOG')
ORDER BY table_name;

-- 检查序列是否创建成功
SELECT 'Sequences Created:' as info FROM dual;
SELECT sequence_name, last_number FROM user_sequences 
WHERE sequence_name IN ('SEQ_ROOM_ID', 'SEQ_DEVICE_ID', 'SEQ_ELDERLY_ID', 'SEQ_HEALTH_ID', 'SEQ_FENCE_ID', 'SEQ_LOG_ID')
ORDER BY sequence_name;

-- 检查索引是否创建成功
SELECT 'Indexes Created:' as info FROM dual;
SELECT index_name, table_name FROM user_indexes 
WHERE table_name IN ('ROOMMANAGEMENT', 'DEVICESTATUS', 'ELDERLYINFO', 'HEALTHMONITORING', 'ELECTRONICFENCE', 'FENCELOG')
AND index_name LIKE 'IDX_%'
ORDER BY table_name, index_name;

-- 检查触发器是否创建成功
SELECT 'Triggers Created:' as info FROM dual;
SELECT trigger_name, table_name, status FROM user_triggers 
WHERE trigger_name LIKE 'TRG_%'
ORDER BY trigger_name;

-- 检查外键约束
SELECT 'Foreign Key Constraints:' as info FROM dual;
SELECT constraint_name, table_name, r_constraint_name 
FROM user_constraints 
WHERE constraint_type = 'R' 
AND table_name IN ('HEALTHMONITORING', 'FENCELOG')
ORDER BY table_name;

SELECT 'Database setup completed successfully!' as result FROM dual;
