-- yada_stacking
-- 2017-03-06

create table LableCode(
SequenceNo Integer identity PRIMARY KEY,--
LCode varchar(64) not null unique,
ToLocation varchar(64) null,
PanelNo varchar(64) null,
Status int not null default 0,
Floor int null,
FloorIndex int null,
Diameter NUMERIC default 0,
Length NUMERIC default 0,
Coordinates varchar(64) null,
Cx NUMERIC default 0,
Cy NUMERIC default 0,
Cz NUMERIC default 0,
Crz NUMERIC default 0,
GetOutLCode varchar(64) null,
CreateDate datetime not null default getdate(),
UpdateDate datetime not null default getdate(),
Remark varchar(256)
);

create table Panel(
SequenceNo Integer identity PRIMARY KEY,
PanelNo varchar(64) null unique,
Status int not null default 0,
CurrFloor int not null,
MaxFloor int not null default 7,
OddStatus bit not null default 0,
EvenStatus bit not null default 0,
CreateDate datetime not null default getdate(),
UpdateDate datetime not null default getdate(),
ToLocation varchar(64) null,
Remark varchar(256)
);

create table RobotParam(
SequenceNo Integer identity PRIMARY KEY,
PanelIndexNo int not null,
BaseIndex int not null,
Base numeric(15, 3) not null default 0,
Rx numeric(15, 4) not null default 0,
Ry numeric(15, 4) not null default 0,
Rz numeric(15, 4) not null default 0,
CreateDate datetime not null default getdate(),
UpdateDate datetime not null default getdate(),
Remark varchar(256)
);

create table OPCParam(
SequenceNo Integer identity PRIMARY KEY,
IndexNo Integer not null default 0,
Name varchar(64) not null,
Code varchar(64) not null,
Class varchar(64) not null default 'None',
Remark varchar(255)
);
