
TRUNCATE  TABLE  OPCParam
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('0','SizeState','MicroWin.S7-1200.NewItem112','Scan','size与PLC数据交换状态(读到0开始写，写完写1）')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('1','Diameter','MicroWin.S7-1200.NewItem10','Scan','直径')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('2','Length','MicroWin.S7-1200.NewItem11','Scan','长度')


INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('0','ScanState','MicroWin.S7-1200.NewItem1','Scan','采集处与PLC数据交换状态(读到0开始写，写完写1）')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('3','ToLocationArea','MicroWin.S7-1200.NewItem12','Scan','交地区')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('4','ToLocationNo','MicroWin.S7-1200.NewItem14','Scan','交地编号')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('5','ScanLable1','MicroWin.S7-1200.NewItem21','Scan','采集到的标签号01')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('6','ScanLable2','MicroWin.S7-1200.NewItem22','Scan','采集到的标签号01')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('9','CameraNo','MicroWin.S7-1200.NewItem15','Scan','相机编号（几号相机采集到的数据，0是手动采集）')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('11','GetWeigh','MicroWin.S7-1200.NewItem16','Scan','称重(1称重，称完置0，称重失败2）')


INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('61','Signal','MicroWin.S7-1200.NewItem111','DeleteLCode','删除布卷开关信号');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('62','LCode1','MicroWin.S7-1200.NewItem27','DeleteLCode','删除布卷标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('63','LCode2','MicroWin.S7-1200.NewItem28','DeleteLCode','删除布卷标签2');

--start2期维护
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('31','BeforCacheStatus','MicroWin.S7-1200.NewItem108','Cache','缓存前标签（读完写好结果后置空）')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('34','CacheStatus','MicroWin.S7-1200.NewItem50','Cache','缓存状态（1走；2存；3取存，同抓子；4走取；5取走；6存取，异抓子）')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('35','CachePoint','MicroWin.S7-1200.NewItem46','Cache','存入缓存区的位置')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('36','GetPoint','MicroWin.S7-1200.NewItem47','Cache','从缓存区取出的位置')


INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('31','Signal','MicroWin.S7-1200.NewItem108','LableUp','标签朝上采集（读完写好结果后置空）')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('34','Diameter','MicroWin.S7-1200.NewItem50','LableUp','直径 （单位毫米）')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('35','Goto','MicroWin.S7-1200.NewItem46','LableUp','去哪道（1-2）')


INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('41','Signal','MicroWin.S7-1200.NewItem109','RobotCarryA','开关信号（PC读到0读标签，读完写1)');


INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('51','Signal','MicroWin.S7-1200.NewItem110','RobotCarryB','开关信号（PC读到0读标签，读完写1)');
--end2期维护

--
-- 2016-11-8
-- A区-C区完成信号
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('12','Signal','MicroWin.S7-1200.NewItem3','AArea1','A1板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('13','LCode1','MicroWin.S7-1200.NewItem134','AArea1','A1板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('14','LCode2','MicroWin.S7-1200.NewItem135','AArea1','A1板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('84','Signal','MicroWin.S7-1200.NewItem4','AArea2','A1板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('85','LCode1','MicroWin.S7-1200.NewItem136','AArea2','A1板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('86','LCode2','MicroWin.S7-1200.NewItem137','AArea2','A1板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('87','Signal','MicroWin.S7-1200.NewItem5','AArea3','A1板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('88','LCode1','MicroWin.S7-1200.NewItem138','AArea3','A1板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('89','LCode2','MicroWin.S7-1200.NewItem139','AArea3','A1板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('80','Signal','MicroWin.S7-1200.NewItem6','AArea4','A1板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('81','LCode1','MicroWin.S7-1200.NewItem140','AArea4','A1板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('82','LCode2','MicroWin.S7-1200.NewItem141','AArea4','A1板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','Signal','MicroWin.S7-1200.NewItem7','AArea5','A1板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('84','LCode1','MicroWin.S7-1200.NewItem142','AArea5','A1板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('85','LCode2','MicroWin.S7-1200.NewItem143','AArea5','A1板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('86','Signal','MicroWin.S7-1200.NewItem8','AArea6','A1板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('87','LCode1','MicroWin.S7-1200.NewItem144','AArea6','A1板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('88','LCode2','MicroWin.S7-1200.NewItem145','AArea6','A1板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('89','Signal','MicroWin.S7-1200.NewItem9','AArea7','A1板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('80','LCode1','MicroWin.S7-1200.NewItem146','AArea7','A1板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('81','LCode2','MicroWin.S7-1200.NewItem147','AArea7','A1板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('82','Signal','MicroWin.S7-1200.NewItem31','CArea1','C1板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','LCode1','MicroWin.S7-1200.NewItem148','CArea1','C1板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('84','LCode2','MicroWin.S7-1200.NewItem149','CArea1','C1板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('85','Signal','MicroWin.S7-1200.NewItem32','CArea2','C2板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('86','LCode1','MicroWin.S7-1200.NewItem150','CArea2','C2板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('87','LCode2','MicroWin.S7-1200.NewItem151','CArea2','C2板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('88','Signal','MicroWin.S7-1200.NewItem33','CArea3','C3板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('89','LCode1','MicroWin.S7-1200.NewItem152','CArea3','C3板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('80','LCode2','MicroWin.S7-1200.NewItem153','CArea3','C3板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('81','Signal','MicroWin.S7-1200.NewItem34','CArea4','C4板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('82','LCode1','MicroWin.S7-1200.NewItem154','CArea4','C4板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','LCode2','MicroWin.S7-1200.NewItem155','CArea4','C4板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('84','Signal','MicroWin.S7-1200.NewItem35','CArea5','C5板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('85','LCode1','MicroWin.S7-1200.NewItem156','CArea5','C5板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('86','LCode2','MicroWin.S7-1200.NewItem157','CArea5','C5板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('87','Signal','MicroWin.S7-1200.NewItem36','CArea6','C6板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('88','LCode1','MicroWin.S7-1200.NewItem158','CArea6','C6板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('89','LCode2','MicroWin.S7-1200.NewItem159','CArea6','C6板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('80','Signal','MicroWin.S7-1200.NewItem37','CArea7','C7板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('81','LCode1','MicroWin.S7-1200.NewItem160','CArea7','C7板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('82','LCode2','MicroWin.S7-1200.NewItem161','CArea7','C7板标签2');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','Signal','MicroWin.S7-1200.NewItem38','CArea8','C8板标签信息开关量');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('84','LCode1','MicroWin.S7-1200.NewItem162','CArea8','C8板标签1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('85','LCode2','MicroWin.S7-1200.NewItem163','CArea8','C8板标签2');


--B区板完成信号

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B01','MicroWin.S7-1200.NewItem86','BAreaPanelFinish','板完成信号Ｂ区板号1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B02','MicroWin.S7-1200.NewItem87','BAreaPanelFinish','板完成信号Ｂ区板号2');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B03','MicroWin.S7-1200.NewItem88','BAreaPanelFinish','板完成信号Ｂ区板号3');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B04','MicroWin.S7-1200.NewItem89','BAreaPanelFinish','板完成信号Ｂ区板号4');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B05','MicroWin.S7-1200.NewItem90','BAreaPanelFinish','板完成信号Ｂ区板号5');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B06','MicroWin.S7-1200.NewItem91','BAreaPanelFinish','板完成信号Ｂ区板号6');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B07','MicroWin.S7-1200.NewItem92','BAreaPanelFinish','板完成信号Ｂ区板号7');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B08','MicroWin.S7-1200.NewItem93','BAreaPanelFinish','板完成信号Ｂ区板号8');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B09','MicroWin.S7-1200.NewItem94','BAreaPanelFinish','板完成信号Ｂ区板号9');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B10','MicroWin.S7-1200.NewItem95','BAreaPanelFinish','板完成信号Ｂ区板号10');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B11','MicroWin.S7-1200.NewItem96','BAreaPanelFinish','板完成信号Ｂ区板号11');

--B区半析完成信号
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B01','MicroWin.S7-1200.NewItem97','BAreaFloorFinish','半板完成信号Ｂ区板号1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B02','MicroWin.S7-1200.NewItem98','BAreaFloorFinish','半板完成信号Ｂ区板号2');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B03','MicroWin.S7-1200.NewItem99','BAreaFloorFinish','半板完成信号Ｂ区板号3');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B04','MicroWin.S7-1200.NewItem100','BAreaFloorFinish','半板完成信号Ｂ区板号4');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B05','MicroWin.S7-1200.NewItem101','BAreaFloorFinish','半板完成信号Ｂ区板号5');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B06','MicroWin.S7-1200.NewItem102','BAreaFloorFinish','半板完成信号Ｂ区板号6');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B07','MicroWin.S7-1200.NewItem103','BAreaFloorFinish','半板完成信号Ｂ区板号7');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B08','MicroWin.S7-1200.NewItem104','BAreaFloorFinish','半板完成信号Ｂ区板号8');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B09','MicroWin.S7-1200.NewItem105','BAreaFloorFinish','半板完成信号Ｂ区板号9');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B10','MicroWin.S7-1200.NewItem106','BAreaFloorFinish','半板完成信号Ｂ区板号10');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B11','MicroWin.S7-1200.NewItem107','BAreaFloorFinish','半板完成信号Ｂ区板号11');

--B区人工码满信号
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B01','MicroWin.S7-1200.NewItem29','BAreaUserFinalLayer','人工完成信号Ｂ区板号1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B02','MicroWin.S7-1200.NewItem30','BAreaUserFinalLayer','人工完成信号Ｂ区板号2');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B03','MicroWin.S7-1200.NewItem39','BAreaUserFinalLayer','人工完成信号Ｂ区板号3');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B04','MicroWin.S7-1200.NewItem40','BAreaUserFinalLayer','人工完成信号Ｂ区板号4');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B05','MicroWin.S7-1200.NewItem41','BAreaUserFinalLayer','人工完成信号Ｂ区板号5');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B06','MicroWin.S7-1200.NewItem42','BAreaUserFinalLayer','人工完成信号Ｂ区板号6');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B07','MicroWin.S7-1200.NewItem43','BAreaUserFinalLayer','人工完成信号Ｂ区板号7');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B08','MicroWin.S7-1200.NewItem66','BAreaUserFinalLayer','人工完成信号Ｂ区板号8');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B09','MicroWin.S7-1200.NewItem67','BAreaUserFinalLayer','人工完成信号Ｂ区板号9');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B10','MicroWin.S7-1200.NewItem68','BAreaUserFinalLayer','人工完成信号Ｂ区板号10');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B11','MicroWin.S7-1200.NewItem69','BAreaUserFinalLayer','人工完成信号Ｂ区板号11');

--B区板状态信号
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B01','MicroWin.S7-1200.NewItem55','BAreaPanelState','板状态Ｂ区板号1');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B02','MicroWin.S7-1200.NewItem56','BAreaPanelState','板状态Ｂ区板号2');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B03','MicroWin.S7-1200.NewItem57','BAreaPanelState','板状态Ｂ区板号3');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B04','MicroWin.S7-1200.NewItem58','BAreaPanelState','板状态Ｂ区板号4');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B05','MicroWin.S7-1200.NewItem59','BAreaPanelState','板状态Ｂ区板号5');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B06','MicroWin.S7-1200.NewItem60','BAreaPanelState','板状态Ｂ区板号6');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B07','MicroWin.S7-1200.NewItem61','BAreaPanelState','板状态Ｂ区板号7');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B08','MicroWin.S7-1200.NewItem62','BAreaPanelState','板状态Ｂ区板号8');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B09','MicroWin.S7-1200.NewItem63','BAreaPanelState','板状态Ｂ区板号9');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B10','MicroWin.S7-1200.NewItem64','BAreaPanelState','板状态Ｂ区板号10');
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','B11','MicroWin.S7-1200.NewItem65','BAreaPanelState','板状态Ｂ区板号11');

--故障和报警
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('83','ERPAlarm','MicroWin.S7-1200.NewItem165','None','ERP:1=ERP通讯失败，2=ERP没有交地标签错误？');

INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('72','RobotWorkState','MicroWin.S7-1200.NewItem70','None','工作状态（运行：１；空闲：０；）')
INSERT OPCParam(IndexNo,Name,Code,Class,Remark) VALUES('73','RobotRunState','MicroWin.S7-1200.NewItem72','None','运行状态是否在安全位置（安全：１；危险：０；）')



TRUNCATE  TABLE  robotparam
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('1','0','-2480.105','180.0000','0.0000','-90.0538',convert(datetime,'2016-09-12 15:37:32.843',121),convert(datetime,'2016-09-12 15:37:32.843',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('1','1','-3520.166','180.0000','0.0000','90.2548',convert(datetime,'2016-09-12 15:37:35.600',121),convert(datetime,'2016-09-12 15:37:35.600',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('1','2','-3520.166','180.0000','0.0000','0.5577',convert(datetime,'2016-09-12 15:37:37.800',121),convert(datetime,'2016-09-12 15:37:37.800',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('1','3','-3520.166','180.0000','0.0000','-179.4834',convert(datetime,'2016-09-12 15:37:40.030',121),convert(datetime,'2016-09-12 15:37:40.030',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('2','0','-4693.363','180.0000','0.0000','-89.2469',convert(datetime,'2016-09-12 15:37:43.570',121),convert(datetime,'2016-09-12 15:37:43.570',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('2','1','-5583.320','180.0000','0.0000','91.4408',convert(datetime,'2016-09-12 15:37:46.010',121),convert(datetime,'2016-09-12 15:37:46.010',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('2','2','-5583.320','180.0000','0.0000','0.7965',convert(datetime,'2016-09-12 15:37:48.147',121),convert(datetime,'2016-09-12 15:37:48.147',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('2','3','-5583.320','180.0000','0.0000','-179.7696',convert(datetime,'2016-09-12 15:37:51.290',121),convert(datetime,'2016-09-12 15:37:51.290',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('3','0','-4633.681','180.0000','0.0000','90.2939',convert(datetime,'2016-09-12 15:20:43.883',121),convert(datetime,'2016-09-12 15:20:43.883',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('3','1','-5121.404','180.0000','0.0000','-89.9200',convert(datetime,'2016-09-12 15:23:14.297',121),convert(datetime,'2016-09-12 15:23:14.297',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('3','2','-5121.404','180.0000','0.0000','-0.3127',convert(datetime,'2016-09-12 15:24:08.067',121),convert(datetime,'2016-09-12 15:24:08.067',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('3','3','-5121.404','180.0000','0.0000','179.6229',convert(datetime,'2016-09-12 15:24:36.817',121),convert(datetime,'2016-09-12 15:24:36.817',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('4','0','-2420.064','180.0000','0.0000','89.8852',convert(datetime,'2016-09-12 15:25:38.033',121),convert(datetime,'2016-09-12 15:25:38.033',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('4','1','-2933.439','180.0000','0.0000','-89.9507',convert(datetime,'2016-09-12 15:27:50.117',121),convert(datetime,'2016-09-12 15:27:50.117',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('4','2','-2933.439','180.0000','0.0000','0.0100',convert(datetime,'2016-09-12 15:28:53.187',121),convert(datetime,'2016-09-12 15:28:53.187',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('4','3','-2933.439','180.0000','0.0000','-179.9863',convert(datetime,'2016-09-12 15:29:16.810',121),convert(datetime,'2016-09-12 15:29:16.810',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('5','0','-214.370','180.0000','0.0000','90.1456',convert(datetime,'2016-09-12 15:32:56.643',121),convert(datetime,'2016-09-12 15:32:56.643',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('5','1','-753.481','180.0000','0.0000','-89.9396',convert(datetime,'2016-09-12 15:33:55.913',121),convert(datetime,'2016-09-12 15:33:55.913',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('5','2','-753.481','180.0000','0.0000','-0.5638',convert(datetime,'2016-09-12 15:34:34.300',121),convert(datetime,'2016-09-12 15:34:34.300',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('5','3','-753.481','180.0000','0.0000','179.1053',convert(datetime,'2016-09-12 15:35:02.233',121),convert(datetime,'2016-09-12 15:35:02.233',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('6','0','1686.696','180.0000','0.0000','89.8069',convert(datetime,'2016-09-12 15:35:41.067',121),convert(datetime,'2016-09-12 15:35:41.067',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('6','1','1313.363','180.0000','0.0000','-89.9057',convert(datetime,'2016-09-12 15:35:46.553',121),convert(datetime,'2016-09-12 15:35:46.553',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('6','2','1313.363','180.0000','0.0000','-0.9775',convert(datetime,'2016-09-12 15:35:50.203',121),convert(datetime,'2016-09-12 15:35:50.203',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('6','3','1313.363','180.0000','0.0000','-179.7868',convert(datetime,'2016-09-12 15:35:57.177',121),convert(datetime,'2016-09-12 15:35:57.177',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('7','0','1084.368','180.0000','0.0000','90.0128',convert(datetime,'2016-09-12 15:36:02.307',121),convert(datetime,'2016-09-12 15:36:02.307',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('7','1','1645.746','180.0000','0.0000','-89.2546',convert(datetime,'2016-09-12 15:36:04.970',121),convert(datetime,'2016-09-12 15:36:04.970',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('7','2','1645.746','180.0000','0.0000','-0.0100',convert(datetime,'2016-09-12 15:36:08.643',121),convert(datetime,'2016-09-12 15:36:08.643',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('7','3','1645.746','180.0000','0.0000','-179.3391',convert(datetime,'2016-09-12 15:36:12.107',121),convert(datetime,'2016-09-12 15:36:12.107',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('8','0','2919.667','180.0000','0.0000','89.9644',convert(datetime,'2016-09-12 15:36:15.540',121),convert(datetime,'2016-09-12 15:36:15.540',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('8','1','3957.118','180.0000','0.0000','-89.2170',convert(datetime,'2016-09-12 15:36:19.423',121),convert(datetime,'2016-09-12 15:36:19.423',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('8','2','3957.118','180.0000','0.0000','-0.0100',convert(datetime,'2016-09-12 15:36:23.987',121),convert(datetime,'2016-09-12 15:36:23.987',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('8','3','3957.118','180.0000','0.0000','-179.6536',convert(datetime,'2016-09-12 15:36:30.593',121),convert(datetime,'2016-09-12 15:36:30.593',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('9','0','-164.924','180.0000','0.0000','-89.3140',convert(datetime,'2016-09-12 15:37:13.087',121),convert(datetime,'2016-09-12 15:37:13.087',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('9','1','190.454','180.0000','0.0000','90.0990',convert(datetime,'2016-09-12 15:37:16.280',121),convert(datetime,'2016-09-12 15:37:16.280',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('9','2','190.454','180.0000','0.0000','-0.0059',convert(datetime,'2016-09-12 15:37:18.870',121),convert(datetime,'2016-09-12 15:37:18.870',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('9','3','190.454','180.0000','0.0000','-179.5960',convert(datetime,'2016-09-12 15:37:21.457',121),convert(datetime,'2016-09-12 15:37:21.457',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('10','0','1580.583','180.0000','0.0000','-89.4172',convert(datetime,'2016-09-12 15:36:56.187',121),convert(datetime,'2016-09-12 15:36:56.187',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('10','1','2351.904','180.0000','0.0000','90.0967',convert(datetime,'2016-09-12 15:37:00.200',121),convert(datetime,'2016-09-12 15:37:00.200',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('10','2','2351.471','180.0000','0.0000','0.8315',convert(datetime,'2016-09-12 15:37:05.233',121),convert(datetime,'2016-09-12 15:37:05.233',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('10','3','2351.471','180.0000','0.0000','-179.7092',convert(datetime,'2016-09-12 15:37:08.593',121),convert(datetime,'2016-09-12 15:37:08.593',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('11','0','3613.065','180.0000','0.0000','-89.7639',convert(datetime,'2016-09-12 15:36:38.770',121),convert(datetime,'2016-09-12 15:36:38.770',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('11','1','4334.850','180.0000','0.0000','90.0806',convert(datetime,'2016-09-12 15:36:42.060',121),convert(datetime,'2016-09-12 15:36:42.060',121),'Base')
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('11','2','4334.850','180.0000','0.0000','1.5283',convert(datetime,'2016-09-12 15:36:44.943',121),convert(datetime,'2016-09-12 15:36:44.943',121),NULL)
INSERT robotparam(PanelIndexNo,BaseIndex,Base,Rx,Ry,Rz,CreateDate,UpdateDate,Remark) VALUES('11','3','4334.850','180.0000','0.0000','178.4580',convert(datetime,'2016-09-12 15:36:49.270',121),convert(datetime,'2016-09-12 15:36:49.270',121),NULL)


