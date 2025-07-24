# ElderlyCareSystem的老人相关模块


## 1.这是老人相关模块的实现，具体涉及到以下这些表
	ElderlyInfo (老人基本信息表)
	FamilyInfo (家属信息表)
	HealthMonitoring (健康监测记录表)
	MedicalOrders (医嘱管理表)
	NursingPlan (护理计划表)
	FeeSettlement (费用结算表)
	HealthAssessmentReport (健康评估报告表)
	ActivityParticipation (活动参与表)：用于记录老人参与具体活动的情况。
	DietRecommendation (智能饮食推荐表)

## 2.业务逻辑:

1. 老人入住登记与健康评估:

    1. 登记: 在ElderlyInfo表中创建一条新记录，包含老人的姓名、身份证号等基本信息，并设置入住状态为“入住”。
    2. 评估: 同时，进行初次健康评估，将评估问卷结果存入HealthAssessmentReport表，并将测量的血压、心率等基本体征存入HealthMonitoring表。这两条记录都通过elderly_id与老人关联。
    3. 家属关联: 将陪同家属的信息录入FamilyInfo表，并与对应的elderly_id关联。

2. 老人电子档案管理:

    虚拟档案概念: 老人的电子档案并非一个独立的物理数据表，而是一个动态生成的综合信息视图。它代表了老人在系统中的完整“数字身份”。
    动态数据汇总: 当需要查阅某位老人的电子档案时，系统会实时地通过elderly_id从以下多个关联表中查询和汇总信息，动态地构建出一个完整的档案视图：
    ElderlyInfo (获取基本信息)FamilyInfo (获取家属信息)
    HealthMonitoring (获取所有健康监测记录)
    HealthAssessmentReport (获取历次健康评估报告)
    MedicalOrders (获取所有历史和当前医嘱)
    NursingPlan (获取护理计划)
    FeeSettlement (获取所有费用和账单记录)
    ActivityParticipation (获取活动参与历史)

    数据实时性:这种设计确保了电子档案的数据永远是最新、最准确的。任何对老人相关信息的更新（如一次新的健康数据记录、一笔新的费用产生）都会立即反映在下一次的档案查询中，无需额外的数据同步或更新操作。

3. 移动端家属信息查询:

    1. 家属通过移动端登录，系统根据其在FamilyInfo表中的记录和elderly_id进行身份验证。
    2. 家属可以查询关联老人的基本信息(ElderlyInfo)、实时健康数据(HealthMonitoring)、历史账单(FeeSettlement)以及活动参与情况(ActivityParticipation)。所有查询操作都应记录在OperationLog中。

4. 智能饮食推荐:

    1. 系统分析HealthMonitoring和HealthAssessmentReport中的健康数据（如血糖、血压、过敏史等）。
    2. 根据预设的营养学规则，为老人生成个性化的饮食建议，并存入DietRecommendation表。
    3. 护理人员或老人可通过系统查看并更新饮食的execution_status（执行状态）。

## 3.当前实现情况

1. 业务一

    在登记时需要将
    
        public class ElderlyFullRegistrationDtos
        {
            public ElderlyInfoCreateDto Elderly { get; set; }
            public HealthAssessmentReportCreateDto Assessment { get; set; }
            public HealthMonitoringCreateDto Monitoring { get; set; }
            public List<FamilyInfoCreateDto> Families { get; set; }

        }
    中对应的ElderlyInfoCreateDto、HealthAssessmentReportCreateDto、HealthMonitoringCreateDto、List<FamilyInfoCreateDto>全部输入进去才算完成。

2. 业务二

    只是简单的将老人所对应的几个表格的内容进行返回，并去除了重复属性（ElderlyId）和一些无用属性(FamilyId)，如

        public class FamilyInfoCreateDto
        {
            public string Name { get; set; }
            public string Relationship { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public string Address { get; set; }
            public string IsPrimaryContact { get; set; }
        }

3. 业务三

    由于没有账号密码的表格，暂时用老人和家属的信息进行登录验证，查询操作同时是简单的返回对应的表格信息

4. 业务四

    调用的是百度的千帆大模型来进行diet的生成，现在对回答的要求是：只生成最简单的食物的名称，没有让ai生成更复杂的信息，比如早上应该吃什么之类的


<mark>**接口的具体传参和功能参考swagger**</mark>