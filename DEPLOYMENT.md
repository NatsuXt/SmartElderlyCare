# 智慧养老系统部署指南

## 数据库连接配置

### 服务器信息

- **主机地址**: 47.96.238.102
- **端口**: 1521
- **服务名**: orcl
- **用户名**: FIBRE
- **密码**: FIBRE2025
- **角色**: default

### 连接字符串

```csharp
"Data Source=47.96.238.102:1521/orcl;User Id=FIBRE;Password=FIBRE2025;"
```

## 部署步骤

### 1. 环境准备

确保已安装以下软件：

- .NET 8.0 SDK
- Oracle客户端 (或 Oracle Instant Client)
- SQL Developer 或其他Oracle管理工具

### 2. 数据库初始化

#### 方式一：使用SQL Developer

1. 打开SQL Developer

2. 创建新连接：
   
   - 连接名称: 智慧养老系统
   - 用户名: FIBRE
   - 密码: FIBRE2025
   - 主机名: 47.96.238.102
   - 端口: 1521
   - 服务名: orcl

3. 测试连接成功后，执行以下脚本：
   
   ```sql
   @SQL/CreateTables.sql
   @SQL/TestData.sql
   ```

#### 方式二：使用SQLPlus

```bash
sqlplus FIBRE/FIBRE2025@47.96.238.102:1521/orcl
@SQL/CreateTables.sql
@SQL/TestData.sql
```

### 3. 应用程序部署

#### 下载项目

```bash
git clone [项目地址]
cd RoomDeviceManagement
```

#### 编译项目

```bash
dotnet restore
dotnet build
```

#### 运行程序

```bash
dotnet run
```

### 4. 验证部署

#### 4.1 数据库连接测试

1. 启动程序
2. 选择 "7. 系统设置"
3. 选择 "1. 数据库状态检查"
4. 查看连接状态和表创建情况

#### 4.2 功能测试

1. 选择 "1. 房间管理"
2. 测试基本的增删改查功能
3. 确认数据能正常保存到数据库

## 配置文件说明

### DatabaseService.cs

位置: `Services/DatabaseService.cs`

默认连接配置：

```csharp
public DatabaseService()
{
    // Oracle 18c 连接字符串配置
    _connectionString = "Data Source=47.96.238.102:1521/orcl;User Id=FIBRE;Password=FIBRE2025;";
}
```

如需修改数据库连接，请更新此文件中的连接字符串。

## 故障排除

### 常见问题

#### 1. 数据库连接失败

**错误**: `ORA-12154: TNS:could not resolve the connect identifier specified`

**解决方案**:

- 检查网络连接
- 确认服务器地址和端口正确
- 验证服务名是否为 `orcl`

#### 2. 用户认证失败

**错误**: `ORA-01017: invalid username/password; logon denied`

**解决方案**:

- 确认用户名为 `FIBRE`
- 确认密码为 `FIBRE2025`
- 检查用户是否存在且有相应权限

#### 3. 表不存在错误

**错误**: `ORA-00942: table or view does not exist`

**解决方案**:

- 执行 `SQL/CreateTables.sql` 脚本创建表
- 确认脚本执行成功
- 检查表名大小写是否正确

#### 4. 网络连接超时

**错误**: `ORA-12170: TNS:Connect timeout occurred`

**解决方案**:

- 检查防火墙设置
- 确认端口1521是否开放
- 测试网络连通性: `telnet 47.96.238.102 1521`

### 日志查看

程序运行时会在控制台输出详细的错误信息，请根据错误提示进行排查。

## 系统特性

### 已实现功能

✅ **房间管理模块**

- 房间信息的完整CRUD操作
- 多条件查询和筛选
- 实时状态管理

### 开发中功能

🔄 **设备状态管理**
🔄 **老人信息管理**
🔄 **健康监测管理**
🔄 **电子围栏管理**
🔄 **围栏日志管理***
