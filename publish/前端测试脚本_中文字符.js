// 智慧养老系统第三模块 - 前端测试脚本
// 确保中文字符正确传输到数据库

const API_BASE_URL = 'http://47.96.238.102:3003';

// 🔧 关键配置：确保中文字符正确传输
const createApiHeaders = () => ({
    'Content-Type': 'application/json; charset=utf-8',
    'Accept': 'application/json'
});

// 测试房间管理API - 中文字符支持
async function testRoomManagementWithChinese() {
    console.log('🏠 测试房间管理API中文字符支持');
    
    const roomData = {
        roomNumber: `豪华套房-${Date.now()}`,
        roomType: "豪华套房",
        capacity: 2,
        status: "空闲",
        rate: 288.50,
        bedType: "双人大床",
        floor: 3
    };

    try {
        // 使用正确的方法创建房间
        const response = await fetch(`${API_BASE_URL}/api/RoomManagement/rooms`, {
            method: 'POST',
            headers: createApiHeaders(),
            body: JSON.stringify(roomData)
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const result = await response.json();
        console.log('✅ 房间创建成功:', result);

        // 立即读取验证中文字符
        if (result.success && result.data) {
            const roomId = result.data.roomId;
            const getResponse = await fetch(`${API_BASE_URL}/api/RoomManagement/rooms/${roomId}`, {
                headers: { 'Accept': 'application/json' }
            });

            if (getResponse.ok) {
                const getRoomResult = await getResponse.json();
                console.log('📖 读取房间数据:', getRoomResult.data);
                
                // 验证中文字符
                if (getRoomResult.data.roomType === "豪华套房" && 
                    getRoomResult.data.bedType === "双人大床" && 
                    getRoomResult.data.status === "空闲") {
                    console.log('🎉 中文字符完美支持！');
                } else {
                    console.log('❌ 中文字符可能有问题');
                    console.log('期望: roomType="豪华套房", bedType="双人大床", status="空闲"');
                    console.log('实际:', {
                        roomType: getRoomResult.data.roomType,
                        bedType: getRoomResult.data.bedType,
                        status: getRoomResult.data.status
                    });
                }
            }
        }

        return result;
    } catch (error) {
        console.error('❌ 房间创建失败:', error);
        throw error;
    }
}

// 测试设备管理API - 中文字符支持
async function testDeviceManagementWithChinese() {
    console.log('📱 测试设备管理API中文字符支持');
    
    const deviceData = {
        deviceName: `智能血压监测仪-${Date.now()}`,
        deviceType: "医疗监测设备",
        location: "二楼护士站",
        status: "正常运行",
        description: "专业医疗级血压监测设备，支持中文显示",
        installationDate: new Date().toISOString()
    };

    try {
        const response = await fetch(`${API_BASE_URL}/api/DeviceManagement/devices`, {
            method: 'POST',
            headers: createApiHeaders(),
            body: JSON.stringify(deviceData)
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const result = await response.json();
        console.log('✅ 设备创建成功:', result);

        // 验证中文字符
        if (result.success && result.data) {
            const deviceId = result.data.deviceId;
            const getResponse = await fetch(`${API_BASE_URL}/api/DeviceManagement/devices/${deviceId}`, {
                headers: { 'Accept': 'application/json' }
            });

            if (getResponse.ok) {
                const getDeviceResult = await getResponse.json();
                console.log('📖 读取设备数据:', getDeviceResult.data);
                
                // 验证中文字符
                if (getDeviceResult.data.deviceType === "医疗监测设备" && 
                    getDeviceResult.data.location === "二楼护士站" && 
                    getDeviceResult.data.status === "正常运行") {
                    console.log('🎉 设备中文字符完美支持！');
                } else {
                    console.log('❌ 设备中文字符可能有问题');
                }
            }
        }

        return result;
    } catch (error) {
        console.error('❌ 设备创建失败:', error);
        throw error;
    }
}

// 测试健康监测API - 中文字符支持
async function testHealthMonitoringWithChinese() {
    console.log('💓 测试健康监测API中文字符支持');
    
    const healthData = {
        elderlyName: "张三",
        checkupType: "常规体检",
        healthStatus: "良好",
        notes: "血压正常，心率稳定，建议定期复查",
        checkupDate: new Date().toISOString()
    };

    try {
        const response = await fetch(`${API_BASE_URL}/api/HealthMonitoring/records`, {
            method: 'POST',
            headers: createApiHeaders(),
            body: JSON.stringify(healthData)
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const result = await response.json();
        console.log('✅ 健康记录创建成功:', result);
        return result;
    } catch (error) {
        console.error('❌ 健康记录创建失败:', error);
        throw error;
    }
}

// 测试电子围栏API - 中文字符支持
async function testElectronicFenceWithChinese() {
    console.log('🔒 测试电子围栏API中文字符支持');
    
    const fenceData = {
        fenceName: `医院安全区域-${Date.now()}`,
        areaDefinition: "医院大楼及周边50米范围，包含门诊部、住院部、花园区域",
        isActive: true,
        status: "正常运行"
    };

    try {
        const response = await fetch(`${API_BASE_URL}/api/ElectronicFence/fences`, {
            method: 'POST',
            headers: createApiHeaders(),
            body: JSON.stringify(fenceData)
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const result = await response.json();
        console.log('✅ 电子围栏创建成功:', result);
        return result;
    } catch (error) {
        console.error('❌ 电子围栏创建失败:', error);
        throw error;
    }
}

// 完整测试套件
async function runCompleteChineseCharacterTest() {
    console.log('🧪 智慧养老系统第三模块 - 中文字符完整测试');
    console.log('===============================================');
    
    try {
        // 测试所有模块
        await testRoomManagementWithChinese();
        await new Promise(resolve => setTimeout(resolve, 1000)); // 等待1秒
        
        await testDeviceManagementWithChinese();
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        await testHealthMonitoringWithChinese();
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        await testElectronicFenceWithChinese();
        
        console.log('🎉 所有中文字符测试完成！');
    } catch (error) {
        console.error('❌ 测试过程中出现错误:', error);
    }
}

// Axios 版本的测试方法（如果项目使用Axios）
async function testWithAxios() {
    // 确保安装了axios: npm install axios
    
    const axios = require('axios'); // 或者 import axios from 'axios';
    
    const apiClient = axios.create({
        baseURL: API_BASE_URL,
        timeout: 10000,
        headers: createApiHeaders()
    });

    const roomData = {
        roomNumber: `Axios豪华套房-${Date.now()}`,
        roomType: "豪华套房",
        capacity: 2,
        status: "空闲",
        rate: 388.80,
        bedType: "双人大床",
        floor: 5
    };

    try {
        const response = await apiClient.post('/api/RoomManagement/rooms', roomData);
        console.log('✅ Axios房间创建成功:', response.data);
        
        // 验证读取
        if (response.data.success && response.data.data) {
            const roomId = response.data.data.roomId;
            const getResponse = await apiClient.get(`/api/RoomManagement/rooms/${roomId}`);
            console.log('📖 Axios读取房间:', getResponse.data);
        }
        
        return response.data;
    } catch (error) {
        console.error('❌ Axios测试失败:', error.response?.data || error.message);
        throw error;
    }
}

// 🚀 使用方法
console.log('使用方法:');
console.log('1. 直接调用: runCompleteChineseCharacterTest()');
console.log('2. 单独测试: testRoomManagementWithChinese()');
console.log('3. Axios测试: testWithAxios()');

// 如果在浏览器中运行，可以直接调用
// runCompleteChineseCharacterTest();

// 如果在Node.js中运行，需要导出
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        runCompleteChineseCharacterTest,
        testRoomManagementWithChinese,
        testDeviceManagementWithChinese,
        testHealthMonitoringWithChinese,
        testElectronicFenceWithChinese,
        testWithAxios
    };
}
