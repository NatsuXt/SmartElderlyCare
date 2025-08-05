// 员工信息表 (StaffInfo)
public DbSet<StaffInfo> StaffInfos { get; set; }

// 老人信息表 (ElderInfo)
public DbSet<ElderlyInfo> ElderlyInfos { get; set; }

// 家属信息表 (FamilyInfo)
public DbSet<FamilyInfo> FamilyInfos { get; set; }

// 护理计划表 (NursingPlan)
public DbSet<NursingPlan> NursingPlans { get; set; }

// 费用结算表 (FeeSettlement)
public DbSet<FeeSettlement> FeeSettlements { get; set; }

// 健康监测记录表 (HealthMonitoring)
public DbSet<HealthMonitoring> HealthMonitorings { get; set; }

// 房间管理表 (RoomManagement)
public DbSet<RoomManagement> RoomManagements { get; set; }

// 设备状态表 (DeviceStatus)
public DbSet<DeviceStatus> DeviceStatuses { get; set; }

// 电子围栏表 (ElectronicFence)
public DbSet<ElectronicFence> ElectronicFences { get; set; }

// 围栏出入记录表 (FenceLog)
public DbSet<FenceLog> FenceLogs { get; set; }

// 医嘱管理表 (MedicalOrder)
public DbSet<MedicalOrder> MedicalOrders { get; set; }

// 活动安排表 (ActivitySchedule)
public DbSet<ActivitySchedule> ActivitySchedules { get; set; }

// 活动参与表 (ActivityParticipation)
public DbSet<ActivityParticipation> ActivityParticipations { get; set; }

// 饮食推荐表(DietRecommendation)
public DbSet<DietRecommendation> DietRecommendations { get; set; }
