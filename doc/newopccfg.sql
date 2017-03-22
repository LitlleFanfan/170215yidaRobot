
create table NewOPCParam(
SequenceNo Integer identity PRIMARY KEY,
Name varchar(64) not null,
Code varchar(64) not null,
Class varchar(64) not null default 'None',
Remark varchar(255)
);



TRUNCATE  TABLE  NewOPCParam
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('SizeState','MicroWin.S7-1200.NewItem112','Scan','size与PLC数据交换状态(读到0开始写，写完写1）')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Diameter','MicroWin.S7-1200.NewItem10','Scan','直径')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Length','MicroWin.S7-1200.NewItem11','Scan','长度')

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('ScanState','MicroWin.S7-1200.NewItem1','Scan','采集处与PLC数据交换状态(读到0开始写，写完写1）')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('ToLocationArea','MicroWin.S7-1200.NewItem12','Scan','交地区')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('ToLocationNo','MicroWin.S7-1200.NewItem14','Scan','交地编号')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('ScanLable1','MicroWin.S7-1200.NewItem21','Scan','采集到的标签号01')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('ScanLable2','MicroWin.S7-1200.NewItem22','Scan','采集到的标签号01')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('PushAside','MicroWin.S7-1200.NewItem15','Scan','通知PLC勾料信号')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('PlcPushAside','MicroWin.S7-1200.NewItem2','Scan','PLC勾料信号（扫描超时）')


INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('GetWeigh','MicroWin.S7-1200.NewItem16','Weigh','称重(1称重，称完置0，称重失败2）')


INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem111','DeleteLCode','删除布卷开关信号');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem27','DeleteLCode','删除布卷标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem28','DeleteLCode','删除布卷标签2');

--start2期维护
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('BeforCacheStatus','MicroWin.S7-1200-3.NewItem108','Cache','缓存前标签（读完写好结果后置空）')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('CacheStatus','MicroWin.S7-1200-3.NewItem50','Cache','缓存状态（1走；2存；3取存，同抓子；4走取；5取走；6存取，异抓子）')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('CachePoint','MicroWin.S7-1200-3.NewItem46','Cache','存入缓存区的位置')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('GetPoint','MicroWin.S7-1200-3.NewItem47','Cache','从缓存区取出的位置')


INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200-3.NewItem113','LableUp','标签朝上采集（读完写好结果后置空）')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Diameter','MicroWin.S7-1200-3.NewItem51','LableUp','直径 （单位毫米）')
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Goto','MicroWin.S7-1200-3.NewItem52','LableUp','去哪道（1-2）')


INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('RobotCarryA','MicroWin.S7-1200-3.NewItem109','RobotCarry','开关信号（PC读到0读标签，读完写1)');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('RobotCarryB','MicroWin.S7-1200-3.NewItem110','RobotCarry','开关信号（PC读到0读标签，读完写1)');

--end2期维护

--
-- 2016-11-8
-- A区-C区完成信号
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem3','AArea1','A1板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem134','AArea1','A1板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem135','AArea1','A1板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem4','AArea2','A1板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem136','AArea2','A1板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem137','AArea2','A1板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem5','AArea3','A1板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem138','AArea3','A1板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem139','AArea3','A1板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem6','AArea4','A1板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem140','AArea4','A1板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem141','AArea4','A1板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem7','AArea5','A1板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem142','AArea5','A1板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem143','AArea5','A1板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem8','AArea6','A1板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem144','AArea6','A1板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem145','AArea6','A1板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem9','AArea7','A1板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem146','AArea7','A1板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem147','AArea7','A1板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem31','CArea1','C1板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem148','CArea1','C1板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem149','CArea1','C1板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem32','CArea2','C2板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem150','CArea2','C2板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem151','CArea2','C2板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem33','CArea3','C3板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem152','CArea3','C3板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem153','CArea3','C3板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem34','CArea4','C4板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem154','CArea4','C4板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem155','CArea4','C4板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem35','CArea5','C5板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem156','CArea5','C5板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem157','CArea5','C5板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem36','CArea6','C6板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem158','CArea6','C6板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem159','CArea6','C6板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem37','CArea7','C7板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem160','CArea7','C7板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem161','CArea7','C7板标签2');

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('Signal','MicroWin.S7-1200.NewItem38','CArea8','C8板标签信息开关量');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode1','MicroWin.S7-1200.NewItem162','CArea8','C8板标签1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('LCode2','MicroWin.S7-1200.NewItem163','CArea8','C8板标签2');


--B区板完成信号

INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B01','MicroWin.S7-1200.NewItem86','BAreaPanelFinish','板完成信号Ｂ区板号1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B02','MicroWin.S7-1200.NewItem87','BAreaPanelFinish','板完成信号Ｂ区板号2');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B03','MicroWin.S7-1200.NewItem88','BAreaPanelFinish','板完成信号Ｂ区板号3');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B04','MicroWin.S7-1200.NewItem89','BAreaPanelFinish','板完成信号Ｂ区板号4');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B05','MicroWin.S7-1200.NewItem90','BAreaPanelFinish','板完成信号Ｂ区板号5');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B06','MicroWin.S7-1200.NewItem91','BAreaPanelFinish','板完成信号Ｂ区板号6');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B07','MicroWin.S7-1200.NewItem92','BAreaPanelFinish','板完成信号Ｂ区板号7');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B08','MicroWin.S7-1200.NewItem93','BAreaPanelFinish','板完成信号Ｂ区板号8');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B09','MicroWin.S7-1200.NewItem94','BAreaPanelFinish','板完成信号Ｂ区板号9');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B10','MicroWin.S7-1200.NewItem95','BAreaPanelFinish','板完成信号Ｂ区板号10');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B11','MicroWin.S7-1200.NewItem96','BAreaPanelFinish','板完成信号Ｂ区板号11');

--B区半析完成信号
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B01','MicroWin.S7-1200.NewItem97','BAreaFloorFinish','半板完成信号Ｂ区板号1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B02','MicroWin.S7-1200.NewItem98','BAreaFloorFinish','半板完成信号Ｂ区板号2');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B03','MicroWin.S7-1200.NewItem99','BAreaFloorFinish','半板完成信号Ｂ区板号3');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B04','MicroWin.S7-1200.NewItem100','BAreaFloorFinish','半板完成信号Ｂ区板号4');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B05','MicroWin.S7-1200.NewItem101','BAreaFloorFinish','半板完成信号Ｂ区板号5');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B06','MicroWin.S7-1200.NewItem102','BAreaFloorFinish','半板完成信号Ｂ区板号6');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B07','MicroWin.S7-1200.NewItem103','BAreaFloorFinish','半板完成信号Ｂ区板号7');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B08','MicroWin.S7-1200.NewItem104','BAreaFloorFinish','半板完成信号Ｂ区板号8');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B09','MicroWin.S7-1200.NewItem105','BAreaFloorFinish','半板完成信号Ｂ区板号9');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B10','MicroWin.S7-1200.NewItem106','BAreaFloorFinish','半板完成信号Ｂ区板号10');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B11','MicroWin.S7-1200.NewItem107','BAreaFloorFinish','半板完成信号Ｂ区板号11');

--B区人工码满信号
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B01','MicroWin.S7-1200.NewItem29','BAreaUserFinalLayer','人工完成信号Ｂ区板号1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B02','MicroWin.S7-1200.NewItem30','BAreaUserFinalLayer','人工完成信号Ｂ区板号2');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B03','MicroWin.S7-1200.NewItem39','BAreaUserFinalLayer','人工完成信号Ｂ区板号3');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B04','MicroWin.S7-1200.NewItem40','BAreaUserFinalLayer','人工完成信号Ｂ区板号4');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B05','MicroWin.S7-1200.NewItem41','BAreaUserFinalLayer','人工完成信号Ｂ区板号5');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B06','MicroWin.S7-1200.NewItem42','BAreaUserFinalLayer','人工完成信号Ｂ区板号6');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B07','MicroWin.S7-1200.NewItem43','BAreaUserFinalLayer','人工完成信号Ｂ区板号7');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B08','MicroWin.S7-1200.NewItem66','BAreaUserFinalLayer','人工完成信号Ｂ区板号8');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B09','MicroWin.S7-1200.NewItem67','BAreaUserFinalLayer','人工完成信号Ｂ区板号9');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B10','MicroWin.S7-1200.NewItem68','BAreaUserFinalLayer','人工完成信号Ｂ区板号10');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B11','MicroWin.S7-1200.NewItem69','BAreaUserFinalLayer','人工完成信号Ｂ区板号11');

--B区板状态信号
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B01','MicroWin.S7-1200.NewItem55','BAreaPanelState','板状态Ｂ区板号1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B02','MicroWin.S7-1200.NewItem56','BAreaPanelState','板状态Ｂ区板号2');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B03','MicroWin.S7-1200.NewItem57','BAreaPanelState','板状态Ｂ区板号3');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B04','MicroWin.S7-1200.NewItem58','BAreaPanelState','板状态Ｂ区板号4');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B05','MicroWin.S7-1200.NewItem59','BAreaPanelState','板状态Ｂ区板号5');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B06','MicroWin.S7-1200.NewItem60','BAreaPanelState','板状态Ｂ区板号6');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B07','MicroWin.S7-1200.NewItem61','BAreaPanelState','板状态Ｂ区板号7');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B08','MicroWin.S7-1200.NewItem62','BAreaPanelState','板状态Ｂ区板号8');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B09','MicroWin.S7-1200.NewItem63','BAreaPanelState','板状态Ｂ区板号9');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B10','MicroWin.S7-1200.NewItem64','BAreaPanelState','板状态Ｂ区板号10');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B11','MicroWin.S7-1200.NewItem65','BAreaPanelState','板状态Ｂ区板号11');

--B区层不规则形状报警信号
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B01','MicroWin.S7-1200.NewItem113','BadShapeLocations','B区层不规则形状报警信号板号1');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B02','MicroWin.S7-1200.NewItem114','BadShapeLocations','B区层不规则形状报警信号板号2');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B03','MicroWin.S7-1200.NewItem115','BadShapeLocations','B区层不规则形状报警信号板号3');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B04','MicroWin.S7-1200.NewItem116','BadShapeLocations','B区层不规则形状报警信号板号4');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B05','MicroWin.S7-1200.NewItem117','BadShapeLocations','B区层不规则形状报警信号板号5');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B06','MicroWin.S7-1200.NewItem118','BadShapeLocations','B区层不规则形状报警信号板号6');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B07','MicroWin.S7-1200.NewItem119','BadShapeLocations','B区层不规则形状报警信号板号7');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B08','MicroWin.S7-1200.NewItem120','BadShapeLocations','B区层不规则形状报警信号板号8');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B09','MicroWin.S7-1200.NewItem121','BadShapeLocations','B区层不规则形状报警信号板号9');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B10','MicroWin.S7-1200.NewItem122','BadShapeLocations','B区层不规则形状报警信号板号10');
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('B11','MicroWin.S7-1200.NewItem123','BadShapeLocations','B区层不规则形状报警信号板号11');

--故障和报警
INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('ERPAlarm','MicroWin.S7-1200.NewItem165','None','ERP:1=ERP通讯失败，2=ERP没有交地标签错误？');

---机器人状态可以直接从机器人电柜里接信号
--INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('RobotWorkState','MicroWin.S7-1200.NewItem70','None','工作状态（运行：１；空闲：０；）');
--INSERT NewOPCParam(Name,Code,Class,Remark) VALUES('RobotRunState','MicroWin.S7-1200.NewItem72','None','运行状态是否在安全位置（安全：１；危险：０；）');

