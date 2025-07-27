-- 智慧养老系统 - 第5步：创建触发器

-- 创建触发器
CREATE OR REPLACE TRIGGER trg_room_update_time
    BEFORE UPDATE ON RoomManagement
    FOR EACH ROW
BEGIN
    :NEW.updated_time := SYSDATE;
END;
/

CREATE OR REPLACE TRIGGER trg_device_update_time
    BEFORE UPDATE ON DeviceStatus
    FOR EACH ROW
BEGIN
    :NEW.updated_time := SYSDATE;
END;
/

CREATE OR REPLACE TRIGGER trg_elderly_update_time
    BEFORE UPDATE ON ElderlyInfo
    FOR EACH ROW
BEGIN
    :NEW.updated_time := SYSDATE;
END;
/

CREATE OR REPLACE TRIGGER trg_fence_update_time
    BEFORE UPDATE ON ElectronicFence
    FOR EACH ROW
BEGIN
    :NEW.updated_time := SYSDATE;
END;
/

COMMIT;
