﻿using System;
using System.Collections.Generic;
using System.Data;
using ProduceComm;
using System.Data.SqlClient;

namespace yidascan.DataAccess {
    public enum LableState {
        Null = 0,
        FloorLastRoll = 2,
        OnPanel = 3,
        PanelFill = 5
    }

    public enum FloorPerformance {
        None,
        OddFinish,
        EvenFinish,
        BothFinish
    }

    public enum CacheState {
        Error = 0,
        Go = 1,
        Cache = 2,
        GetThenCache = 3,
        GoThenGet = 4,
        GetThenGo = 5,
        CacheAndGet = 6
    }

    public static class CacheStateDesc {
        public static string describe(CacheState state) {
            switch (state) {
                case CacheState.Error:
                    return $"{state}: error";
                case CacheState.Go:
                    return $"{state}: go";
                case CacheState.Cache:
                    return $"{state}: cache";
                case CacheState.GetThenCache:
                    return $"{state}: get then cache";
                case CacheState.GoThenGet:
                    return $"{state}: go then get";
                case CacheState.GetThenGo:
                    return $"{state}: get then go";
                case CacheState.CacheAndGet:
                    return $"{state}: cache and get";
                default:
                    return $"{state}: unknown cache state";
            }

        }
    }

    public class LableCode {
        public int SequenceNo { get; set; }
        public string LCode { get; set; }
        public string ToLocation { get; set; }
        public string RealLocation { get; set; }
        public string PanelNo { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Remark { get; set; }
        public string Coordinates { get; set; }

        public bool CoordinatesIsEmpty() {
            return string.IsNullOrEmpty(Coordinates);
        }

        decimal cx;
        public decimal Cx {
            get {
                return cx;
            }

            set {
                cx = value;
            }
        }

        decimal cy;
        public decimal Cy {
            get {
                return cy;
            }

            set {
                cy = value;
            }
        }

        decimal cz;
        public decimal Cz {
            get {
                return cz;
            }

            set {
                cz = value;
            }
        }

        decimal crz;
        public decimal Crz {
            get {
                return crz;
            }

            set {
                crz = value;
            }
        }

        string getOutLCode;
        public string GetOutLCode {
            get { return getOutLCode; }
            set { getOutLCode = value; }
        }

        decimal diameter;
        public decimal Diameter {
            get { return diameter; }
            set { diameter = value; }
        }

        decimal length;
        public decimal Length {
            get { return length; }
            set { length = value; }
        }

        int floorIndex;
        public int FloorIndex {
            get { return floorIndex; }
            set { floorIndex = value; }
        }

        int floor;
        public int Floor {
            get { return floor; }
            set { floor = value; }
        }

        public LableCode() { }

        public LableCode(string code, string tolocation, decimal length, bool scanByMan) {
            LCode = code;
            ToLocation = tolocation;
            Length = length;
            Remark = Remark + (scanByMan ? "handwork" : "automatic");
            Coordinates = "";
        }

        /// <summary>
        /// 号码前6位。
        /// </summary>
        /// <returns></returns>
        public string CodePart1() {
            return LCode.Substring(0, 6);
        }

        /// <summary>
        /// 号码后6位。
        /// </summary>
        /// <returns></returns>
        public string CodePart2() {
            return LCode.Substring(6, 6);
        }

        public static string MakeCode(string part1, string part2) {
            const int WIDTH = 6;
            const char FILL_CHAR = '0';
            return part1.PadLeft(WIDTH, FILL_CHAR) + part2.PadLeft(WIDTH, FILL_CHAR);
        }

        public void SetSize(decimal dia, decimal len) {
            Diameter = dia;
            Remark = Remark + len;
        }

        public string Size_s() {
            return $"布卷直径: {Diameter};布卷长: {Length}";
        }

        public bool isOddSide() {
            return this.floorIndex % 2 == 1;
        }

        public bool isEvenSide() {
            return (floorIndex % 2 == 0) && (floorIndex > 0);
        }

        /// <summary>
        /// 从交地字符串中提取序号
        /// </summary>
        /// <returns></returns>
        public string ParseLocationNo() {
            return string.IsNullOrEmpty(ToLocation) || ToLocation.Length < 3
                ? string.Empty
                : ToLocation.Substring(1, 2);
        }

        public static string ParseRealLocationNo(string locno) {
            return string.IsNullOrEmpty(locno) || locno.Length < 3
                ? string.Empty
                : locno.Substring(1, 2);
        }

        /// <summary>
        /// 从交地字符串中提取区域字符。
        /// </summary>
        /// <returns></returns>
        public string ParseLocationArea() {
            return string.IsNullOrEmpty(ToLocation)
                ? string.Empty
                : ToLocation.Substring(0, 1);
        }

        public static void SetOnPanelState(string lablecode) {
            var cp = new List<CommandParameter>() { new CommandParameter(@"UPDATE LableCode SET [Status] = @Status,[UpdateDate] = @UpdateDate
                  WHERE [LCode] = @LCode",
                new SqlParameter[]{
                    new SqlParameter("@Status",LableState.OnPanel),
                    new SqlParameter("@UpdateDate",DateTime.Now),
                    new SqlParameter("@LCode",lablecode)}) };
            DataAccess.CreateDataAccess.sa.NonQueryTran(cp);
        }

        public static bool Add(LableCode c) {
            var cps = CreateLableCodeInsert(c);
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        /// <summary>
        /// 把PanelInfo的板号和层，赋予当前标签。
        /// 如果pinfo为null, 则产生新的板号。
        /// </summary>
        /// <param name="pinfo"></param>
        public void SetupPanelInfo(PanelInfo pinfo) {
            PanelNo = pinfo != null ? pinfo.PanelNo : PanelGen.NewPanelNo();
            Floor = pinfo != null ? pinfo.CurrFloor : 1;
            FloorIndex = 0;
            Coordinates = "";
        }

        public static bool Update(FloorPerformance fp, PanelInfo pInfo, LableCode c, LableCode c2 = null) {
            var cps = new List<CommandParameter>();
            cps.Add(CreateLableCodeUpdate(c));
            if (c2 != null) {
                cps.Add(new CommandParameter(@"update LableCode set FloorIndex=@FloorIndex,Coordinates=@Coordinates,
                    Cx=@Cx,Cy=@Cy,Cz=@Cz,Crz=@Crz,Status=@Status,Remark=@Remark,UpdateDate=@UpdateDate where LCode=@LCode",
                    new SqlParameter[]{
                        new SqlParameter("@LCode",c2.LCode),
                        new SqlParameter("@FloorIndex",c2.floorIndex),
                        new SqlParameter("@Coordinates",c2.Coordinates),
                        new SqlParameter("@Cx",c2.cx),
                        new SqlParameter("@Cy",c2.cy),
                        new SqlParameter("@Cz",c2.cz),
                        new SqlParameter("@Crz",c2.crz),
                        new SqlParameter("@Status",c2.Status),
                        c2.Remark==null?new SqlParameter("@Remark",DBNull.Value):new SqlParameter("@Remark",c2.Remark),
                        new SqlParameter("@UpdateDate",DateTime.Now)}));
            }
            switch (fp) {
                case FloorPerformance.OddFinish:
                case FloorPerformance.EvenFinish:
                    cps.Add(new CommandParameter("UPDATE Panel SET CurrFloor = @CurrFloor,OddStatus = @OddStatus,EvenStatus = @EvenStatus," +
                            "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo",
                        new SqlParameter[]{
                        new SqlParameter("@PanelNo",c.PanelNo),
                        new SqlParameter("@CurrFloor",c.floor),
                        new SqlParameter("@OddStatus",fp==FloorPerformance.OddFinish),
                        new SqlParameter("@EvenStatus",fp==FloorPerformance.EvenFinish),
                        new SqlParameter("@UpdateDate",DateTime.Now)}));
                    break;
                case FloorPerformance.BothFinish:
                    if (c.floor == pInfo.MaxFloor || clsSetting.SplintHeight < GetFloorMaxDiameter(c.PanelNo, c.floor + 1) + 50) {
                        cps.Add(new CommandParameter("UPDATE Panel SET Status = @Status,MaxFloor=CurrFloor," +
                            "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo",
                        new SqlParameter[]{
                            new SqlParameter("@PanelNo",c.PanelNo),
                            new SqlParameter("@Status",LableState.PanelFill),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                        if (pInfo.HasExceed) {
                            var panelno = PanelGen.NewPanelNo();
                            cps.Add(new CommandParameter("UPDATE LableCode SET Floor = 1,PanelNo = @NewPanelNo," +
                                    "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo and FloorIndex=0",
                                new SqlParameter[]{
                            new SqlParameter("@NewPanelNo",panelno),
                            new SqlParameter("@PanelNo",pInfo.PanelNo),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                            cps.Add(CreateInsertPanel(panelno, c.ToLocation));
                        }
                    } else {
                        cps.Add(new CommandParameter("UPDATE Panel SET CurrFloor = @CurrFloor,OddStatus = @OddStatus,EvenStatus = @EvenStatus," +
                                "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo",
                            new SqlParameter[]{
                            new SqlParameter("@PanelNo",c.PanelNo),
                            new SqlParameter("@CurrFloor",c.floor+1),
                            new SqlParameter("@OddStatus",false),
                            new SqlParameter("@EvenStatus",false),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                        if (pInfo.HasExceed) {
                            cps.Add(new CommandParameter("UPDATE LableCode SET Floor = @Floor," +
                                "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo and FloorIndex=0",
                            new SqlParameter[]{
                            new SqlParameter("@PanelNo",pInfo.PanelNo),
                            new SqlParameter("@Floor",c.floor+1),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                        }
                    }
                    break;
                case FloorPerformance.None:
                default:
                    break;
            }
            cps.Add(new CommandParameter("UPDATE Panel SET HasExceed=@HasExceed,UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo",
                new SqlParameter[]{
                       new SqlParameter("@PanelNo",pInfo.PanelNo),
                       new SqlParameter("@HasExceed",pInfo.HasExceed),
                       new SqlParameter("@UpdateDate",DateTime.Now)}));
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        public static bool UpdateEdgeExceed(FloorPerformance fp, PanelInfo pInfo, LableCode cur, LableCode fromcache) {
            var cps = new List<CommandParameter>();
            cps.Add(CreateLableCodeUpdate(fromcache));
            switch (fp) {
                case FloorPerformance.BothFinish:
                    if (fromcache.floor == pInfo.MaxFloor || clsSetting.SplintHeight < GetFloorMaxDiameter(cur.PanelNo, cur.floor + 1) + 50) {//板满
                        cps.Add(new CommandParameter("UPDATE Panel SET Status = @Status,MaxFloor=CurrFloor," +
                                "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo",
                            new SqlParameter[]{
                            new SqlParameter("@PanelNo",pInfo.PanelNo),
                            new SqlParameter("@Status",LableState.PanelFill),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                        cur.PanelNo = PanelGen.NewPanelNo();
                        cur.floor = 1;
                        cur.floorIndex = 0;
                        cur.Coordinates = null;
                        cur.Crz = 0;
                        cur.Cx = 0;
                        cur.Cy = 0;
                        cur.Cz = 0;
                        cps.Add(CreateLableCodeUpdate(cur));
                        cps.Add(new CommandParameter("UPDATE LableCode SET Floor = @Floor,PanelNo = @NewPanelNo," +
                                "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo and FloorIndex=0",
                            new SqlParameter[]{
                            new SqlParameter("@NewPanelNo",cur.PanelNo),
                            new SqlParameter("@PanelNo",pInfo.PanelNo),
                            new SqlParameter("@Floor",cur.floor),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                        cps.Add(CreateInsertPanel(cur.PanelNo, cur.ToLocation));
                    } else {//层满
                        cur.floor++;
                        cps.Add(CreateLableCodeUpdate(cur));
                        cps.Add(new CommandParameter("UPDATE LableCode SET Floor = @Floor," +
                                "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo and FloorIndex=0",
                            new SqlParameter[]{
                            new SqlParameter("@PanelNo",pInfo.PanelNo),
                            new SqlParameter("@Floor",cur.floor),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                        cps.Add(new CommandParameter("UPDATE Panel SET CurrFloor = @CurrFloor,OddStatus = @OddStatus,EvenStatus = @EvenStatus," +
                                "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo",
                            new SqlParameter[]{
                            new SqlParameter("@PanelNo",pInfo.PanelNo),
                            new SqlParameter("@CurrFloor",cur.floor),
                            new SqlParameter("@OddStatus",false),
                            new SqlParameter("@EvenStatus",false),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                    }
                    break;
                case FloorPerformance.None:
                default:
                    break;
            }
            cps.Add(new CommandParameter("UPDATE Panel SET HasExceed=@HasExceed,UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo",
                new SqlParameter[]{
                       new SqlParameter("@PanelNo",pInfo.PanelNo),
                       new SqlParameter("@HasExceed",pInfo.HasExceed),
                       new SqlParameter("@UpdateDate",DateTime.Now)}));
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        private static List<CommandParameter> CreateLableCodeInsert(LableCode c) {
            return new List<CommandParameter>() { new CommandParameter(
                                "insert into LableCode(LCode,ToLocation,PanelNo,Floor,FloorIndex,Diameter,Length,Coordinates,Cx,Cy,Cz,Crz,GetOutLCode,Remark) " +
                            "values(@LCode,@ToLocation,@PanelNo,@Floor,@FloorIndex,@Diameter,@Length,@Coordinates,@Cx,@Cy,@Cz,@Crz,@GetOutLCode,@Remark)",
                                new SqlParameter[]{
                        new SqlParameter("@LCode",c.LCode),
                        new SqlParameter("@ToLocation",c.ToLocation),
                        c.PanelNo==null?new SqlParameter("@PanelNo",DBNull.Value):new SqlParameter("@PanelNo",c.PanelNo),
                        new SqlParameter("@Floor",c.floor),
                        new SqlParameter("@FloorIndex",c.floorIndex),
                        new SqlParameter("@Diameter",c.diameter),
                        new SqlParameter("@Length",c.length),
                        new SqlParameter("@Coordinates",c.Coordinates),
                        new SqlParameter("@Cx",c.cx),
                        new SqlParameter("@Cy",c.cy),
                        new SqlParameter("@Cz",c.cz),
                        new SqlParameter("@Crz",c.crz),
                        c.getOutLCode==null?new SqlParameter("@GetOutLCode",DBNull.Value):new SqlParameter("@GetOutLCode",c.getOutLCode),
                        c.Remark==null?new SqlParameter("@Remark",DBNull.Value):new SqlParameter("@Remark",c.Remark)}) };
        }

        public static PanelInfo GetPanel(string panelNo) {
            var sql = "select * from Panel where PanelNo=@PanelNo";
            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }
            return Helper.DataTableToObjList<PanelInfo>(dt)[0];
        }

        public static bool SetMaxFloorAndFull(string panelno) {
            var sql = "update panel set maxfloor=currfloor,status= 5 where status!= 5 and panelno = @panelno";
            var sp = new SqlParameter[]{
                new SqlParameter("@panelno",panelno)};
            return DataAccess.CreateDataAccess.sa.NonQuery(sql, sp);
        }

        public static bool UserSetPanelLastRoll(string lcode) {
            var sql = "update LableCode set status=2,UpdateDate=getdate(),Remark=Remark+' floor last roll' where LCode=@LCode";
            var sp = new SqlParameter[]{
                new SqlParameter("@LCode",lcode)};
            return DataAccess.CreateDataAccess.sa.NonQuery(sql, sp);
        }

        public static bool SetAllPanelsFinished() {
            var cps = new List<CommandParameter>() {
                new CommandParameter("update Panel set Status=5 where Status!=5", new SqlParameter[]{}),
                new CommandParameter("update LableCode set Status=5 where Status != 5 and(ToLocation like 'A%' or ToLocation like 'C%')",
                new SqlParameter[]{}),
                new CommandParameter(
                    "insert into LableCodeHis([LCode],[ToLocation],[PanelNo],[Status],[Floor],[FloorIndex],[Diameter],[Length]," +
                    "[Coordinates],[Cx],[Cy],[Cz],[Crz],[GetOutLCode],[CreateDate],[UpdateDate],[Remark],[RealLocation]) " +
                    "select [LCode],[ToLocation],[PanelNo],[Status],[Floor],[FloorIndex],[Diameter],[Length],[Coordinates]," +
                    "[Cx],[Cy],[Cz],[Crz],[GetOutLCode],[CreateDate],[UpdateDate],[Remark],[RealLocation] from LableCode",
                new SqlParameter[]{}),
                new CommandParameter("delete from LableCode",
                new SqlParameter[]{}),
                new CommandParameter("insert into PanelHis([PanelNo],[Status],[CurrFloor],[MaxFloor],[OddStatus]," +
                "[EvenStatus],[CreateDate],[UpdateDate],[ToLocation],[Remark])" +
                "select [PanelNo],[Status],[CurrFloor],[MaxFloor],[OddStatus]," +
                "[EvenStatus],[CreateDate],[UpdateDate],[ToLocation],[Remark] from Panel",
                new SqlParameter[]{}),
                new CommandParameter("delete from Panel",
                new SqlParameter[]{}) };
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        public static decimal GetWidth(string panelNo, int floorIndex) {
            var sql = "select sum(Diameter) from LableCode where PanelNo=@PanelNo and FloorIndex%2=@FloorIndex%2 and FloorIndex<>0";
            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@FloorIndex",floorIndex)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            return (decimal)dt.Rows[0][0];
        }

        public static bool Update(string panelNo) {
            var cps = new List<CommandParameter>() {
                new CommandParameter(@"update LableCode set UpdateDate=@UpdateDate,Status=@Status where panelNo=@panelNo",
                new SqlParameter[]{
                new SqlParameter("@UpdateDate",DateTime.Now),
                new SqlParameter("@Status",LableState.PanelFill),
                new SqlParameter("@panelNo",panelNo)}),
                new CommandParameter("update Panel set UpdateDate=@UpdateDate,Status=@Status where panelNo=@panelNo",
                new SqlParameter[]{
                new SqlParameter("@UpdateDate",DateTime.Now),
                new SqlParameter("@Status",LableState.PanelFill),
                new SqlParameter("@panelNo",panelNo)})};
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        public static bool Update(LableCode obj) {
            var cps = new List<CommandParameter>();
            cps.Add(CreateLableCodeUpdate(obj));
            cps.Add(CreateInsertPanel(obj.PanelNo, obj.ToLocation));
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        public static bool UpdateRealLocation(LableCode obj) {
            var cps = new List<CommandParameter>() {
                new CommandParameter(@"update lablecode set reallocation=@reallocation where lcode=@lcode",
                new SqlParameter[] {
                new SqlParameter("@lcode", obj.LCode) ,
                new SqlParameter("@reallocation", obj.RealLocation) }) };
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        private static CommandParameter CreateInsertPanel(string panelNo, string tolocation) {
            return new CommandParameter("INSERT INTO Panel (PanelNo,ToLocation,Status,CurrFloor,MaxFloor,Remark)" +
                                "VALUES(@PanelNo,@ToLocation,@Status,1,@MaxFloor,@Remark)",
                            new SqlParameter[]{
                                 new SqlParameter("@PanelNo",panelNo),
                                 new SqlParameter("@ToLocation",tolocation),
                                 new SqlParameter("@Status",tolocation.Substring(0,1)=="B"? LableState.Null:LableState.PanelFill),
                                 new SqlParameter("@MaxFloor",clsSetting.MaxFloor),
                                 new SqlParameter("@Remark",tolocation)});
        }

        public static bool Update(string panelNo, string tolocation, List<string> lcodes) {
            var cps = new List<CommandParameter>() {
                new CommandParameter($"update LableCode set PanelNo=@PanelNo,Floor=1 where LCode in('{string.Join("','",lcodes.ToArray())}')",
                new SqlParameter[]{
                    new SqlParameter("@PanelNo",panelNo)}),
                new CommandParameter("INSERT INTO Panel (PanelNo,ToLocation,Status,CurrFloor,MaxFloor,Remark)" +
                    "VALUES(@PanelNo,@ToLocation,@Status,1,@MaxFloor,@Remark)",
                new SqlParameter[]{
                    new SqlParameter("@PanelNo",panelNo),
                    new SqlParameter("@ToLocation",tolocation),
                    new SqlParameter("@Status",tolocation.Substring(0,1)=="B"? LableState.Null:LableState.PanelFill),
                    new SqlParameter("@MaxFloor",clsSetting.MaxFloor),
                    new SqlParameter("@Remark","人工满板重新计算。")}) };
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        private static CommandParameter CreateLableCodeUpdate(LableCode obj) {
            return new CommandParameter(@"UPDATE LableCode SET [PanelNo] = @PanelNo
                  ,[Floor] = @Floor,[FloorIndex] = @FloorIndex,[Coordinates] = @Coordinates
                  ,Cx=@Cx,Cy=@Cy,Cz=@Cz,Crz=@Crz,[GetOutLCode] = @GetOutLCode,[UpdateDate] = @UpdateDate,Status=@Status,
                  [Remark] = @Remark WHERE SequenceNo =@SequenceNo and [LCode] = @LCode and [ToLocation] = @ToLocation",
                new SqlParameter[]{
                    new SqlParameter("@PanelNo",obj.PanelNo),
                    new SqlParameter("@Floor",obj.floor),
                    new SqlParameter("@FloorIndex",obj.floorIndex),
                    obj.Coordinates==null?new SqlParameter("@Coordinates",DBNull.Value):new SqlParameter("@Coordinates",obj.Coordinates),
                    new SqlParameter("@Cx",obj.cx),
                    new SqlParameter("@Cy",obj.cy),
                    new SqlParameter("@Cz",obj.cz),
                    new SqlParameter("@Crz",obj.crz),
                    obj.getOutLCode==null?new SqlParameter("@GetOutLCode",DBNull.Value):new SqlParameter("@GetOutLCode",obj.getOutLCode),
                    new SqlParameter("@UpdateDate",DateTime.Now),
                    new SqlParameter("@Status",obj.Status),
                    new SqlParameter("@Remark",obj.Remark),
                    new SqlParameter("@SequenceNo",obj.SequenceNo),
                    new SqlParameter("@LCode",obj.LCode),
                    new SqlParameter("@ToLocation",obj.ToLocation)});
        }

        public static bool Delete(string code) {
            var sql = "delete from LableCode where LCode=@LCode";
            var sp = new SqlParameter[]{
                new SqlParameter("@LCode",code)};
            return DataAccess.CreateDataAccess.sa.NonQuery(sql, sp);
        }

        public static bool DeleteByLocation(string tolocation) {
            var sql = "delete from LableCode where tolocation=@tolocation";
            var sp = new SqlParameter[]{
                new SqlParameter("@tolocation",tolocation)};
            return DataAccess.CreateDataAccess.sa.NonQuery(sql, sp);
        }

        public static DataTable QueryByLocation(string location) {
            var sql = "select * from LableCode where tolocation=@tolocation";
            var sp = new SqlParameter[]{
                new SqlParameter("@tolocation",location)};
            return DataAccess.CreateDataAccess.sa.Query(sql, sp);
        }

        public static bool PanelNoFinished(string panelNo) {
            var sql = "select * from LableCode where PanelNo=@PanelNo and Status=5";
            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            return dt != null && dt.Rows.Count > 0;
        }

        public static bool SetPanelFinished(string panelNo) {
            var cps = new List<CommandParameter>() {
                new CommandParameter("update LableCode set Status=5 where PanelNo=@PanelNo",
                new SqlParameter[]{new SqlParameter("@PanelNo",panelNo)}),
                new CommandParameter(
                    "insert into LableCodeHis([LCode],[ToLocation],[PanelNo],[Status],[Floor],[FloorIndex],[Diameter],[Length]," +
                    "[Coordinates],[Cx],[Cy],[Cz],[Crz],[GetOutLCode],[CreateDate],[UpdateDate],[Remark],[RealLocation]) " +
                    "select [LCode],[ToLocation],[PanelNo],[Status],[Floor],[FloorIndex],[Diameter],[Length]," +
                    "[Coordinates],[Cx],[Cy],[Cz],[Crz],[GetOutLCode],[CreateDate],[UpdateDate],[Remark],[RealLocation] " +
                    "from LableCode where PanelNo=@PanelNo",
                new SqlParameter[]{new SqlParameter("@PanelNo",panelNo)}),
                new CommandParameter("delete from LableCode where PanelNo=@PanelNo",
                new SqlParameter[]{new SqlParameter("@PanelNo",panelNo)}),
                new CommandParameter("insert into PanelHis([PanelNo],[Status],[CurrFloor],[MaxFloor],[OddStatus]," +
                "[EvenStatus],[CreateDate],[UpdateDate],[ToLocation],[Remark])" +
                "select [PanelNo],[Status],[CurrFloor],[MaxFloor],[OddStatus]," +
                "[EvenStatus],[CreateDate],[UpdateDate],[ToLocation],[Remark] from Panel where PanelNo=@PanelNo",
                new SqlParameter[]{new SqlParameter("@PanelNo",panelNo)}),
                new CommandParameter("delete from Panel where PanelNo=@PanelNo",
                new SqlParameter[]{new SqlParameter("@PanelNo",panelNo)}) };
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        public static bool IsPanelLastRoll(string panelNo, string lcode) {
            var sql = @"select top 1 * from LableCode
where panelNo=@PanelNo and exists (select top 1 panelNo from Panel where PanelNo=@PanelNo and LableCode.floor=panel.MaxFloor)
order by floorindex desc;";
            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt != null && dt.Rows.Count > 0) {
                return dt.Rows[0]["LCode"].ToString() == lcode;
            }
            return false;
        }

        [Obsolete("use areAllRollsOnBoard instead.")]
        public static bool IsAllRollOnPanel(string panelNo) {
            var sql = @"select * from LableCode where PanelNo=@PanelNo 
and exists (select 1 from Panel where Panel.PanelNo=LableCode.PanelNo and (LableCode.Floor=Panel.CurrFloor or LableCode.Floor=Panel.CurrFloor-1)) 
and Status<3 and FloorIndex<>0";
            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            return !(dt != null && dt.Rows.Count > 0);
        }

        /// <summary>
        /// 从数据库读取指定交地和板的当前层的所有标签。
        /// </summary>
        /// <param name="tolocation">交地</param>
        /// <param name="pinfo">板信息</param>
        /// <returns></returns>
        public static List<LableCode> GetLableCodesOfRecentFloor(string toLocation, string panelNo, int floor) {
            var sql = @"select * from LableCode where ToLocation=@toLocation and 
                                    PanelNo=@PanelNo and Floor=@Floor";
            var sp = new SqlParameter[]{
                new SqlParameter("@ToLocation",toLocation),
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",floor)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return new List<LableCode>();
            }
            return Helper.DataTableToObjList<LableCode>(dt);
        }

        public static PanelInfo GetTolactionCurrPanelNo(string tolocation, string dateShiftNo) {
            // 这个判断批次号的做法非常不可靠。
            var sql = "select * from Panel where ToLocation=@ToLocation and PanelNo like @PanelDate+'%'  and Status=0 order by PanelNo desc";
            var sp = new SqlParameter[]{
                new SqlParameter("@ToLocation",tolocation),
                new SqlParameter("@PanelDate",dateShiftNo)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }
            return Helper.DataTableToObjList<PanelInfo>(dt)[0];
        }

        public static bool SetPanelNo(string lCode) {
            var cps = new List<CommandParameter>() {
                new CommandParameter(@"update lablecode set PanelNo=tmp.PanelNo,updatedate=getdate(),Status=5 from 
                (select SequenceNo,ToLocation,PanelNo from lablecode where LCode=@lCode) tmp 
                where    tmp.ToLocation = lablecode.ToLocation and
                tmp.SequenceNo > lablecode.SequenceNo and lablecode.Status = 0",
                new SqlParameter[]{new SqlParameter("@lCode", lCode) }) };
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        public static bool DeleteFinishPanel(string lCode) {
            var cps = new List<CommandParameter>() {
            new CommandParameter(
                    @"insert into LableCodeHis([LCode],[ToLocation],[PanelNo],[Status],[Floor],[FloorIndex],[Diameter],[Length],
                [Coordinates],[Cx],[Cy],[Cz],[Crz],[GetOutLCode],[CreateDate],[UpdateDate],[Remark],[RealLocation])
                select [LCode],[ToLocation],[PanelNo],[Status],[Floor],[FloorIndex],[Diameter],[Length],
                [Coordinates],[Cx],[Cy],[Cz],[Crz],[GetOutLCode],[CreateDate],[UpdateDate],[Remark],[RealLocation]
                from LableCode where PanelNo=(select PanelNo from lablecode where LCode=@lCode)",
                new SqlParameter[]{new SqlParameter("@lCode", lCode) }),
                new CommandParameter("insert into PanelHis([PanelNo],[Status],[CurrFloor],[MaxFloor],[OddStatus]," +
                "[EvenStatus],[CreateDate],[UpdateDate],[ToLocation],[Remark])" +
                "select [PanelNo],[Status],[CurrFloor],[MaxFloor],[OddStatus]," +
                "[EvenStatus],[CreateDate],[UpdateDate],[ToLocation],[Remark] " +
                "from Panel where PanelNo=(select PanelNo from lablecode where LCode=@lCode)",
                new SqlParameter[]{new SqlParameter("@lCode", lCode) }),
                new CommandParameter("delete from Panel where PanelNo=(select PanelNo from lablecode where LCode=@lCode)",
                new SqlParameter[]{new SqlParameter("@lCode", lCode) }),
                new CommandParameter("delete from LableCode where PanelNo=(select PanelNo from lablecode where LCode=@lCode)",
                new SqlParameter[]{new SqlParameter("@lCode", lCode) }) };
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }


        public static List<LableCode> GetLastLableCode(string panelNo, int currFloor) {
            var sql = "select * from LableCode where PanelNo=@PanelNo and Floor=@Floor";
            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",currFloor)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }
            return Helper.DataTableToObjList<LableCode>(dt);
        }

        public static string GetLastPanelNo(string shiftNo) {
            var sql = "select top 1 PanelNo from LableCode where PanelNo like @shiftNo+'%' group by PanelNo order by PanelNo desc";

            var sp = new SqlParameter[]{
                new SqlParameter("@shiftNo",shiftNo)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return string.Empty;
            }
            return dt.Rows[0][0].ToString();
        }

        public static decimal GetFloorMaxDiameter(string panelNo, int currFloor) {
            var sql = "select  MAX(Diameter+Cz) " +
                "from LableCode where PanelNo = @PanelNo and Floor=@Floor";

            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",currFloor-1)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            return (decimal)dt.Rows[0][0];
        }

        public static decimal GetFloorHalfAvgLength(string panelNo, int currFloor) {
            var sql = "select  avg(Length)/2 " +
                "from LableCode where PanelNo = @PanelNo and Floor=@Floor";

            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",currFloor-1)};//此处应该传进来当前层的上一层层号，建议修改
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            var elem = dt.Rows[0][0];
            return elem != null ? (decimal)elem : 0;
        }

        /// <summary>
        /// 取一层中，倒数第二个较短布卷的长度.
        /// </summary>
        /// <param name="panelNo"></param>
        /// <param name="currFloor"></param>
        /// <param name="floor">todo: describe floor parameter on GetSecondShortestLengthHalf</param>
        /// <returns></returns>
        [Obsolete("no use any more.")]
        public static decimal GetSecondShortestLengthHalf(string panelNo, int floor) {
            var sql = "select top 2 Length from LableCode where PanelNo = @PanelNo and Floor=@Floor order by Length";

            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",floor)
            };

            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);

            if (dt == null || dt.Rows.Count < 2) {
                return 0;
            }

            return (decimal)dt.Rows[1][0] / 2;
        }

        public static int GetPanelMaxFloor(string panelNo) {
            var sql = "select max(Floor) from LableCode b where PanelNo = @PanelNo";

            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            return (int)dt.Rows[0][0];
        }

        public static decimal GetFloorMaxLength(string panelNo, int currFloor) {
            var sql = "select isnull(sum(Length),0) from (select (select max(Length) Length " +
                "from LableCode b where PanelNo=@PanelNo and b.Floor=a.Floor)Length " +
                "from LableCode a where PanelNo = @PanelNo and Floor<@Floor group by Floor)tmp";

            var sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",currFloor)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            return (decimal)dt.Rows[0][0];
        }

        public static LableCode QueryByLCode(string lcode) {
            var sql = "select * from LableCode where lcode=@lcode";
            var sp = new SqlParameter[]{
                new SqlParameter("@lcode",lcode)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }
            return Helper.DataTableToObjList<LableCode>(dt)[0];
        }

        public static IList<LableCode> QueryByLCodeFromHis(string lcode) {
            var sql = $"select top 100 * from LableCodeHis where lcode like @lcode order by createdate desc";
            var sp = new SqlParameter[]{
                new SqlParameter("@lcode",$"%{lcode}%")};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);

            if (dt == null) { return null; }
            return dt.ToList<LableCode>();
        }

        public static List<string> QueryLabelcodeByPanelNo(string panelno) {
            var sql = "select lcode from LableCode where panelno=@panelno";
            var sp = new SqlParameter[]{
                new SqlParameter("@panelno",panelno)};
            var dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }

            var r = new List<string>();
            foreach (DataRow row in dt.Rows) {
                r.Add(row["LCode"].ToString());
            }
            return r;
        }

        /// <summary>
        /// 取lablecode表中全部交地名称，不含重复项。
        /// </summary>
        /// <returns></returns>
        public static List<string> QueryAreaBLocations() {
            var sql = "select distinct tolocation from LableCode order by tolocation";
            var dt = DataAccess.CreateDataAccess.sa.Query(sql);

            var r = new List<string>();
            if (dt != null) {
                foreach (DataRow row in dt.Rows) {
                    var s = row["tolocation"].ToString();
                    if (s.StartsWith("B") || s.StartsWith("b")) {
                        r.Add(s);
                    }
                }
            }

            return r;
        }

        /// <summary>
        /// 用于界面队列显示
        /// </summary>
        /// <returns></returns>
        public string brief() {
            return $"{LCode} {ToLocation}/{RealLocation} {diameter} F{floor}/{floorIndex}";
        }

        public string detail() {
            return $"{LCode} {ToLocation}/{RealLocation} {diameter} F{Floor}/{FloorIndex} panel/{PanelNo}";
        }

        private static List<CommandParameter> CreateLableCodeInsertHistory(LableCode c) {
            return new List<CommandParameter>() { new CommandParameter(
                                "insert into LableCodeHis(LCode,ToLocation,PanelNo,Status,Floor,FloorIndex,Diameter,Length,Coordinates,Cx,Cy,Cz,Crz,GetOutLCode,Remark,[RealLocation]) " +
                            "values(@LCode,@ToLocation,@PanelNo,@Status,@Floor,@FloorIndex,@Diameter,@Length,@Coordinates,@Cx,@Cy,@Cz,@Crz,@GetOutLCode,@Remark,@RealLocation)",
                                new SqlParameter[]{
                        new SqlParameter("@LCode",c.LCode),
                        new SqlParameter("@ToLocation",c.ToLocation ?? ""),
                        new SqlParameter("@PanelNo",c.PanelNo ?? ""),
                        new SqlParameter("@Status", c.Status),
                        new SqlParameter("@Floor",c.floor),
                        new SqlParameter("@FloorIndex",c.floorIndex),
                        new SqlParameter("@Diameter",c.diameter),
                        new SqlParameter("@Length",c.length),
                        new SqlParameter("@Coordinates",c.Coordinates ?? ""),
                        new SqlParameter("@Cx",c.cx),
                        new SqlParameter("@Cy",c.cy),
                        new SqlParameter("@Cz",c.cz),
                        new SqlParameter("@Crz",c.crz),
                        new SqlParameter("@GetOutLCode",c.getOutLCode ?? ""),
                        new SqlParameter("@Remark",c.Remark ?? ""),
                        new SqlParameter("@RealLocation",c.RealLocation ?? "")}) };
        }

        public static bool SaveToHistory(LableCode c) {
            var cps = CreateLableCodeInsertHistory(c);
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }
    }
}
