
CREATE TABLE JobTasks
(
	Id BIGINT NOT NULL PRIMARY KEY,
	NodeName NVARCHAR(500) NOT NULL,
	Title NVARCHAR(50) NOT NULL,
	Remark NVARCHAR(500) NULL,
	CronExpression NVARCHAR(50) NULL,
	AssemblyName NVARCHAR(200) NOT NULL,
	ClassName NVARCHAR(200) NOT NULL,
	CustomParamsJson NVARCHAR(2000) NULL,
	Status INT NOT NULL,
	CreateTime DATETIME NOT NULL,
	CreateUserId INT NOT NULL,
	CreateUserName NVARCHAR(300) NULL,
	LastRunTime DATETIME NULL,
	NextRunTime DATETIME NULL,
	TotalRunCount INT NOT NULL,
	Children VARCHAR(1000) NULL
	);
CREATE TABLE JobNodes(
	NodeName NVARCHAR(400) NOT NULL PRIMARY KEY,
	NodeType NVARCHAR(500) NOT NULL,
	MachineName NVARCHAR(500) NULL,
	AccessProtocol NVARCHAR(500) NOT NULL,
	Host NVARCHAR(500) NOT NULL,
	AccessSecret NVARCHAR(500) NULL,
	LastUpdateTime DATETIME NULL,
	Status INT NOT NULL,
	Priority INT NOT NULL,
	GenerID INT NOT NULL
	);
CREATE TABLE JobTrace(
	TraceId BIGINT NOT NULL PRIMARY KEY,
	TaskId BIGINT NOT NULL,
	Node NVARCHAR(400) NULL,
	StartTime DATETIME NOT NULL,
	EndTime DATETIME  NULL,
	ElapsedTime FLOAT NOT NULL,
	Result INT NOT NULL) ;
CREATE TABLE JobDict
(
	Id INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,
	Name VARCHAR(500) NOT NULL ,
	Value VARCHAR(500)  NULL ,
	Type INT NOT NULL,
	Sort INT NOT NULL,
	Extension VARCHAR(1000) NULL,
	Remark NVARCHAR(500) NULL
);
CREATE TABLE JobUser
(
	Id BIGINT NOT NULL PRIMARY KEY,
    UserName VARCHAR(100) NOT NULL,
	Password VARCHAR(40) NOT NULL,
	RealName NVARCHAR(100)  NULL,
	Phone VARCHAR(20)  NULL ,
	Email VARCHAR(100) NULL,
	Status INT NOT NULL,
	CreateTime DATETIME NOT NULL,
	LastLoginTime DATETIME NULL
);
