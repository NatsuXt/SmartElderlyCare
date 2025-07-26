# 智慧养老系统 - 访客预约与数据分析模块

一个基于 .NET 8 和 Oracle 数据库的现代化养老院管理系统，提供访客预约管理和多维度数据可视化功能。

## 功能特性

### 核心功能
- **远程视频探视预约**：家属在线提交探视申请，工作人员审核管理
- **健康趋势分析**：老人健康数据的时间序列分析和可视化
- **费用构成分析**：多维度费用统计和图表展示
- **操作日志记录**：完整的审计追溯和系统监控

### 系统特性
- **高可用性**：完善的错误处理和边界情况处理
- **数据安全**：完整的操作日志和数据验证
- **用户友好**：统一的API响应格式和清晰的错误提示
- **易于扩展**：模块化设计，支持功能扩展

## 技术架构

### 后端技术栈
- **.NET 8**：现代化的后端框架
- **ASP.NET Core Web API**：RESTful API设计
- **Entity Framework Core**：ORM数据访问层
- **Oracle Database**：企业级数据库支持

### 核心组件
- **Models**：数据模型和实体定义
- **Services**：业务逻辑服务层
- **Controllers**：API控制器层
- **Migrations**：数据库版本管理

## 快速开始

### 环境要求
- .NET 8.0 SDK
- Oracle Database 12c+
- Visual Studio 2022 或 VS Code

### 安装步骤

1. **克隆项目**
```bash
git clone https://github.com/yourusername/nursing-home-system.git
cd nursing-home-system/NursingHome/NursingHome
```

2. **安装依赖**
```bash
dotnet restore
```

3. **配置数据库**
```bash
# 修改 appsettings.json 中的连接字符串
# 创建数据库表（使用提供的SQL脚本）
```

4. **运行项目**
```bash
dotnet run
```

5. **访问API文档**
```
https://localhost:5155/swagger
```

## API接口说明

### 访客预约管理
- `POST /api/VisitorRegistration/submit-video-visit` - 提交预约申请
- `PUT /api/VisitorRegistration/{id}/approve` - 审核预约
- `GET /api/VisitorRegistration` - 查询预约列表
- `GET /api/VisitorRegistration/pending-approval` - 获取待审核列表

### 数据分析仪表板
- `POST /api/Dashboard/health-trend-data` - 获取健康趋势数据
- `POST /api/Dashboard/fee-composition-data` - 获取费用构成数据
- `GET /api/Dashboard/operation-logs` - 查询操作日志

## 数据库设计

### 核心数据表
- **VisitorRegistrations**：访客预约记录
- **HealthMonitorings**：健康监测数据  
- **FeeSettlements**：费用结算记录
- **OperationLogs**：系统操作日志
- **SystemAnnouncements**：系统公告


## 测试说明

### API测试
系统提供完整的Swagger UI界面，支持：
- 功能测试：核心业务流程验证
- 边界测试：异常数据和边界条件处理
- 集成测试：多模块协同工作验证

### 测试数据
项目包含完整的测试数据集：
- 健康监测样本数据
- 费用结算示例记录
- 多种预约状态场景

## 系统监控

### 操作日志
- **完整追溯**：记录所有用户操作和系统行为
- **性能监控**：API调用时间和响应状态
- **安全审计**：操作人员、时间、IP地址记录

### 数据统计
- **实时查询**：支持按时间范围和条件筛选
- **图表支持**：数据格式适配常见图表库
- **导出功能**：支持数据导出和报表生成

## 配置说明

### 数据库配置
```json
{
  "ConnectionStrings": {
    "OracleDb": "Data Source=your-server:1521/your-service;User Id=username;Password=password;"
  }
}
```

### 日志配置
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
