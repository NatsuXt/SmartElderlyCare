// 员工信息表 (STAFFINFO)
public DbSet<STAFFINFO> StaffInfos { get; set; }

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

//系统公告表(SystemAnnouncements)
public DbSet<SystemAnnouncements> SystemAnnouncements { get; set; }

//操作日志表(OperationLog)
public DbSet<OperationLog> OperationLogs { get; set; }

//访客登记表(VisitorRegistration)   
public DbSet<VisitorRegistration> VisitorRegistrations { get; set; }

//SOS通知表（SOSNOTIFICATION）
public DbSet<SOSNOTIFICATION> SOSNotifications { get; set; }

//员工计划表（STAFFSCHEDULE）
public DbSet<STAFFSCHEDULE> StaffSchedules { get; set; }

//员工位置表（STAFFLOCATIION）
public DbSet<STAFFLOCATION> StaffLocations { get; set; }

// 异常健康预警表 (HealthAlert)
public DbSet<HealthAlert> HealthAlerts { get; set; }

// 健康数据阈值表 (HealthThreshold)
public DbSet<HealthThreshold> HealthThresholds { get; set; }

// 药品库存信息表 (MedicineInventory)
public DbSet<MedicineInventory> MedicineInventories { get; set; }

// 药品采购管理表 (MedicineProcurement)
public DbSet<MedicineProcurement> MedicineProcurements { get; set; }

// 语音助手提示表 (VoiceAssistantReminder)
public DbSet<VoiceAssistantReminder> VoiceAssistantReminders { get; set; }

//SOS（EMERGENCYSOS）
   public DbSet<EMERGENCYSOS> EmergencySOS { get; set; }

//消毒表（DISINFECTIONRECORD）
public DbSet<DISINFECTIONRECORD> DisinfectionRecords { get; set; }


public DbSet<FeeDetail> FeeDetails { get; set; }

// 健康评估报告表(HealthAssessmentReport)
public DbSet<HealthAssessmentReport> HealthAssessmentReports { get; set; }
