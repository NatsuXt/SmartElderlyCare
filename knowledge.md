models�ļ����д�����ݿ���ÿһ�ű�����ݽṹ
dto������ǰ�˺ͺ��֮�䴫����Ϣ


��ɫ	����
Controller	�������󲢷��ؽ��
DTO			���ջ򷵻��������õ����ݽṹ
Service		ִ��ҵ���߼�
Model		�������ݽṹ���������ݿ�洢
DbContext	�������ݿ⣬����ʵ�ʲ���

public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
��ʾ�̳У���DbContextOptions<AppDbContext> options�е�options���ݸ�����base��options

public DbSet<ElderlyInfo> ElderlyInfo { get; set; }
DbSet<ElderlyInfo> ��һ�����⼯�ϣ���ʾ���ݿ��� ElderlyInfo ���������
�����൱�ڶ������ElderlyInfoΪһ������ElderlyInfo��Ľӿ�
{get;set;}��������������Խ��ж�д
ͬʱ���ԶԶ�д����һЩ���ƣ�����
int age{get;set;}�����൱��

private int _age;

public int Age
{
    get { return _age; }
    set
    {
        if (value >= 0)  // ������֤��ֵ������
            _age = value;
    }
}


