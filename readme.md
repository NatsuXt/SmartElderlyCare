# ElderlyCareSystem���������ģ��


## 1.�����������ģ���ʵ�֣������漰��������Щ��
	ElderlyInfo (���˻�����Ϣ��)
	FamilyInfo (������Ϣ��)
	HealthMonitoring (��������¼��)
	MedicalOrders (ҽ�������)
	NursingPlan (����ƻ���)
	FeeSettlement (���ý����)
	HealthAssessmentReport (�������������)
	ActivityParticipation (������)�����ڼ�¼���˲�������������
	DietRecommendation (������ʳ�Ƽ���)

## 2.ҵ���߼�:

1. ������ס�Ǽ��뽡������:

    1. �Ǽ�: ��ElderlyInfo���д���һ���¼�¼���������˵����������֤�ŵȻ�����Ϣ����������ס״̬Ϊ����ס����
    2. ����: ͬʱ�����г��ν����������������ʾ�������HealthAssessmentReport������������Ѫѹ�����ʵȻ�����������HealthMonitoring����������¼��ͨ��elderly_id�����˹�����
    3. ��������: ����ͬ��������Ϣ¼��FamilyInfo�������Ӧ��elderly_id������

2. ���˵��ӵ�������:

    ���⵵������: ���˵ĵ��ӵ�������һ���������������ݱ�����һ����̬���ɵ��ۺ���Ϣ��ͼ����������������ϵͳ�е�������������ݡ���
    ��̬���ݻ���: ����Ҫ����ĳλ���˵ĵ��ӵ���ʱ��ϵͳ��ʵʱ��ͨ��elderly_id�����¶���������в�ѯ�ͻ�����Ϣ����̬�ع�����һ�������ĵ�����ͼ��
    ElderlyInfo (��ȡ������Ϣ)FamilyInfo (��ȡ������Ϣ)
    HealthMonitoring (��ȡ���н�������¼)
    HealthAssessmentReport (��ȡ���ν�����������)
    MedicalOrders (��ȡ������ʷ�͵�ǰҽ��)
    NursingPlan (��ȡ����ƻ�)
    FeeSettlement (��ȡ���з��ú��˵���¼)
    ActivityParticipation (��ȡ�������ʷ)

    ����ʵʱ��:�������ȷ���˵��ӵ�����������Զ�����¡���׼ȷ�ġ��κζ����������Ϣ�ĸ��£���һ���µĽ������ݼ�¼��һ���µķ��ò���������������ӳ����һ�εĵ�����ѯ�У�������������ͬ������²�����

3. �ƶ��˼�����Ϣ��ѯ:

    1. ����ͨ���ƶ��˵�¼��ϵͳ��������FamilyInfo���еļ�¼��elderly_id���������֤��
    2. �������Բ�ѯ�������˵Ļ�����Ϣ(ElderlyInfo)��ʵʱ��������(HealthMonitoring)����ʷ�˵�(FeeSettlement)�Լ���������(ActivityParticipation)�����в�ѯ������Ӧ��¼��OperationLog�С�

4. ������ʳ�Ƽ�:

    1. ϵͳ����HealthMonitoring��HealthAssessmentReport�еĽ������ݣ���Ѫ�ǡ�Ѫѹ������ʷ�ȣ���
    2. ����Ԥ���Ӫ��ѧ����Ϊ�������ɸ��Ի�����ʳ���飬������DietRecommendation��
    3. ������Ա�����˿�ͨ��ϵͳ�鿴��������ʳ��execution_status��ִ��״̬����

## 3.��ǰʵ�����

1. ҵ��һ

    �ڵǼ�ʱ��Ҫ��
    
        public class ElderlyFullRegistrationDtos
        {
            public ElderlyInfoCreateDto Elderly { get; set; }
            public HealthAssessmentReportCreateDto Assessment { get; set; }
            public HealthMonitoringCreateDto Monitoring { get; set; }
            public List<FamilyInfoCreateDto> Families { get; set; }

        }
    �ж�Ӧ��ElderlyInfoCreateDto��HealthAssessmentReportCreateDto��HealthMonitoringCreateDto��List<FamilyInfoCreateDto>ȫ�������ȥ������ɡ�

2. ҵ���

    ֻ�Ǽ򵥵Ľ���������Ӧ�ļ����������ݽ��з��أ���ȥ�����ظ����ԣ�ElderlyId����һЩ��������(FamilyId)����

        public class FamilyInfoCreateDto
        {
            public string Name { get; set; }
            public string Relationship { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public string Address { get; set; }
            public string IsPrimaryContact { get; set; }
        }

3. ҵ����

    ����û���˺�����ı����ʱ�����˺ͼ�������Ϣ���е�¼��֤����ѯ����ͬʱ�Ǽ򵥵ķ��ض�Ӧ�ı����Ϣ

4. ҵ����

    ���õ��ǰٶȵ�ǧ����ģ��������diet�����ɣ����ڶԻش��Ҫ���ǣ�ֻ������򵥵�ʳ������ƣ�û����ai���ɸ����ӵ���Ϣ����������Ӧ�ó�ʲô֮���


<mark>**�ӿڵľ��崫�κ͹��ܲο�swagger**</mark>