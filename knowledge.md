models文件夹中存放数据库中每一张表的数据结构
dto用于在前端和后端之间传输信息


角色	功能
Controller	接收请求并返回结果
DTO			接收或返回请求中用的数据结构
Service		执行业务逻辑
Model		定义数据结构，用于数据库存储
DbContext	连接数据库，进行实际操作

public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
表示继承，将DbContextOptions<AppDbContext> options中的options传递给父类base的options

public DbSet<ElderlyInfo> ElderlyInfo { get; set; }
DbSet<ElderlyInfo> 是一个特殊集合，表示数据库中 ElderlyInfo 表的所有行
所以相当于定义变量ElderlyInfo为一个访问ElderlyInfo表的接口
{get;set;}声明这个变量可以进行读写
同时可以对读写进行一些限制，比如
int age{get;set;}可以相当于

private int _age;

public int Age
{
    get { return _age; }
    set
    {
        if (value >= 0)  // 可以验证赋值合理性
            _age = value;
    }
}


