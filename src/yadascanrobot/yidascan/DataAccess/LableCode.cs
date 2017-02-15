using System;
using System.Collections.Generic;
using System.Data;
using ProduceComm;
using System.Data.SqlClient;

namespace yidascan.DataAccess {
    public enum LableState {
        Null = 0,
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
        Error,
        Go,
        Cache,
        GetThenCache,
        GoThenGet,
        GetThenGo
    }

    public class LableCode {
        int sequenceNo;

        public int SequenceNo {
            get { return sequenceNo; }
            set { sequenceNo = value; }
        }

        string lCode;

        public string LCode {
            get { return lCode; }
            set { lCode = value; }
        }

        string toLocation;

        public string ToLocation {
            get { return toLocation; }
            set { toLocation = value; }
        }

        string panelNo;

        public string PanelNo {
            get { return panelNo; }
            set { panelNo = value; }
        }

        int status;

        public int Status {
            get { return status; }
            set { status = value; }
        }

        DateTime createDate;

        public DateTime CreateDate {
            get { return createDate; }
            set { createDate = value; }
        }

        DateTime updateDate;

        public DateTime UpdateDate {
            get { return updateDate; }
            set { updateDate = value; }
        }

        string remark;

        public string Remark {
            get { return remark; }
            set { remark = value; }
        }

        string coordinates;
        public string Coordinates {
            set { coordinates = value; }
        }
        public bool CoordinatesIsEmpty() {
            return string.IsNullOrEmpty(coordinates);
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

        public LableCode(string code, string tolocation, bool scanByMan) {
            LCode = code;
            ToLocation = tolocation;
            Remark = (scanByMan ? "handwork" : "automatic");
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
            Length = len;
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
            List<CommandParameter> cp = new List<CommandParameter>() { new CommandParameter(@"UPDATE LableCode SET [Status] = @Status,[UpdateDate] = @UpdateDate
                  WHERE [LCode] = @LCode",
                new SqlParameter[]{
                    new SqlParameter("@Status",LableState.OnPanel),
                    new SqlParameter("@UpdateDate",DateTime.Now),
                    new SqlParameter("@LCode",lablecode)}) };
            DataAccess.CreateDataAccess.sa.NonQueryTran(cp);
        }

        public static bool Add(LableCode c) {
            List<CommandParameter> cps = CreateLableCodeInsert(c);
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        /// <summary>
        /// 把PanelInfo的板号和层，赋予当前标签。
        /// </summary>
        /// <param name="pinfo"></param>
        public void SetupPanelInfo(PanelInfo pinfo) {
            PanelNo = pinfo.PanelNo;
            Floor = pinfo.CurrFloor;
            FloorIndex = 0;
            Coordinates = "";
        }

        public static bool Update(FloorPerformance fp, PanelInfo pInfo, LableCode c, LableCode c2 = null) {
            List<CommandParameter> cps = CreateLableCodeUpdate(c);
            if (c2 != null) {
                cps.Add(new CommandParameter(@"update LableCode set FloorIndex=@FloorIndex,Coordinates=@Coordinates,
                    Cx=@Cx,Cy=@Cy,Cz=@Cz,Crz=@Crz,UpdateDate=@UpdateDate where LCode=@LCode",
                    new SqlParameter[]{
                        new SqlParameter("@LCode",c2.lCode),
                        new SqlParameter("@FloorIndex",c2.floorIndex),
                        new SqlParameter("@Coordinates",c2.coordinates),
                        new SqlParameter("@Cx",c2.cx),
                        new SqlParameter("@Cy",c2.cy),
                        new SqlParameter("@Cz",c2.cz),
                        new SqlParameter("@Crz",c2.crz),
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
                    if (c.floor == pInfo.MaxFloor) {
                        cps.Add(new CommandParameter("UPDATE Panel SET Status = @Status," +
                                "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo",
                            new SqlParameter[]{
                            new SqlParameter("@PanelNo",c.PanelNo),
                            new SqlParameter("@Status",LableState.PanelFill),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                    } else {
                        cps.Add(new CommandParameter("UPDATE Panel SET CurrFloor = @CurrFloor,OddStatus = @OddStatus,EvenStatus = @EvenStatus," +
                                "UpdateDate = @UpdateDate WHERE PanelNo = @PanelNo",
                            new SqlParameter[]{
                            new SqlParameter("@PanelNo",c.PanelNo),
                            new SqlParameter("@CurrFloor",c.floor+1),
                            new SqlParameter("@OddStatus",false),
                            new SqlParameter("@EvenStatus",false),
                            new SqlParameter("@UpdateDate",DateTime.Now)}));
                    }
                    break;
                case FloorPerformance.None:
                default:
                    break;
            }
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        private static List<CommandParameter> CreateLableCodeInsert(LableCode c) {
            return new List<CommandParameter>() { new CommandParameter(
                                "insert into LableCode(LCode,ToLocation,PanelNo,Floor,FloorIndex,Diameter,Length,Coordinates,Cx,Cy,Cz,Crz,GetOutLCode,Remark) " +
                            "values(@LCode,@ToLocation,@PanelNo,@Floor,@FloorIndex,@Diameter,@Length,@Coordinates,@Cx,@Cy,@Cz,@Crz,@GetOutLCode,@Remark)",
                                new SqlParameter[]{
                        new SqlParameter("@LCode",c.lCode),
                        new SqlParameter("@ToLocation",c.toLocation),
                        c.panelNo==null?new SqlParameter("@PanelNo",DBNull.Value):new SqlParameter("@PanelNo",c.panelNo),
                        new SqlParameter("@Floor",c.floor),
                        new SqlParameter("@FloorIndex",c.floorIndex),
                        new SqlParameter("@Diameter",c.diameter),
                        new SqlParameter("@Length",c.length),
                        new SqlParameter("@Coordinates",c.coordinates),
                        new SqlParameter("@Cx",c.cx),
                        new SqlParameter("@Cy",c.cy),
                        new SqlParameter("@Cz",c.cz),
                        new SqlParameter("@Crz",c.crz),
                        c.getOutLCode==null?new SqlParameter("@GetOutLCode",DBNull.Value):new SqlParameter("@GetOutLCode",c.getOutLCode),
                        c.remark==null?new SqlParameter("@Remark",DBNull.Value):new SqlParameter("@Remark",c.remark)}) };
        }

        public static PanelInfo GetPanel(string panelNo) {
            string sql = "select * from Panel where PanelNo=@PanelNo";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }
            return Helper.DataTableToObjList<PanelInfo>(dt)[0];
        }

        public static bool SetMaxFloor(string tolocation) {
            string sql = "update panel set maxfloor=currfloor where status!= 5 and tolocation = @tolocation";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@tolocation",tolocation)};
            return DataAccess.CreateDataAccess.sa.NonQuery(sql, sp);
        }

        public static decimal GetWidth(string panelNo, int floorIndex) {
            string sql = "select sum(Diameter) from LableCode where PanelNo=@PanelNo and FloorIndex%2=@FloorIndex%2 and FloorIndex<>0";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@FloorIndex",floorIndex)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            return (decimal)dt.Rows[0][0];
        }

        public static bool Update(string panelNo) {
            List<CommandParameter> cps = new List<CommandParameter>() {
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
            List<CommandParameter> cps = CreateLableCodeUpdate(obj);
            cps.Add(new CommandParameter("INSERT INTO Panel (PanelNo,ToLocation,Status,CurrFloor,MaxFloor,Remark)" +
                    "VALUES(@PanelNo,@ToLocation,@Status,1,@MaxFloor,@Remark)",
                new SqlParameter[]{
                    new SqlParameter("@PanelNo",obj.panelNo),
                    new SqlParameter("@ToLocation",obj.toLocation),
                    new SqlParameter("@Status",obj.ToLocation.Substring(0,1)=="B"? LableState.Null:LableState.PanelFill),
                    new SqlParameter("@MaxFloor",clsSetting.MaxFloor),
                    new SqlParameter("@Remark",obj.toLocation)}));
            return DataAccess.CreateDataAccess.sa.NonQueryTran(cps);
        }

        private static List<CommandParameter> CreateLableCodeUpdate(LableCode obj) {
            return new List<CommandParameter>() {
                new CommandParameter(@"UPDATE LableCode SET [PanelNo] = @PanelNo
                  ,[Floor] = @Floor,[FloorIndex] = @FloorIndex,[Coordinates] = @Coordinates
                  ,Cx=@Cx,Cy=@Cy,Cz=@Cz,Crz=@Crz,[GetOutLCode] = @GetOutLCode,[UpdateDate] = @UpdateDate,
                  [Remark] = @Remark WHERE SequenceNo =@SequenceNo and [LCode] = @LCode and [ToLocation] = @ToLocation",
                new SqlParameter[]{
                    new SqlParameter("@PanelNo",obj.panelNo),
                    new SqlParameter("@Floor",obj.floor),
                    new SqlParameter("@FloorIndex",obj.floorIndex),
                    obj.coordinates==null?new SqlParameter("@Coordinates",DBNull.Value):new SqlParameter("@Coordinates",obj.coordinates),
                        new SqlParameter("@Cx",obj.cx),
                        new SqlParameter("@Cy",obj.cy),
                        new SqlParameter("@Cz",obj.cz),
                        new SqlParameter("@Crz",obj.crz),
                    obj.getOutLCode==null?new SqlParameter("@GetOutLCode",DBNull.Value):new SqlParameter("@GetOutLCode",obj.getOutLCode),
                    new SqlParameter("@UpdateDate",DateTime.Now),
                    new SqlParameter("@Remark",obj.remark),
                    new SqlParameter("@SequenceNo",obj.sequenceNo),
                    new SqlParameter("@LCode",obj.lCode),
                    new SqlParameter("@ToLocation",obj.toLocation)})};
        }

        public static bool DeleteAllFinished() {
            string sql = "delete from LableCode where Status=5";
            return DataAccess.CreateDataAccess.sa.NonQuery(sql);
        }

        public static bool Delete(string code) {
            string sql = "delete from LableCode where LCode=@LCode";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@LCode",code)};
            return DataAccess.CreateDataAccess.sa.NonQuery(sql, sp);
        }

        public static bool DeleteByLocation(string tolocation) {
            string sql = "delete from LableCode where tolocation=@tolocation";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@tolocation",tolocation)};
            return DataAccess.CreateDataAccess.sa.NonQuery(sql, sp);
        }

        public static DataTable QueryByLocation(string location) {
            string sql = "select * from LableCode where tolocation=@tolocation";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@tolocation",location)};
            return DataAccess.CreateDataAccess.sa.Query(sql, sp);
        }

        public static bool PanelNoFinished(string panelNo) {
            string sql = "select * from LableCode where PanelNo=@PanelNo and Status=5";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            return dt != null && dt.Rows.Count > 0;
        }

        public static bool IsPanelLastRoll(string panelNo, string lcode) {
            string sql = @"select top 1 * from LableCode
where panelNo=@PanelNo and exists (select top 1 panelNo from Panel where PanelNo=@PanelNo and LableCode.floor=panel.MaxFloor)
order by floorindex desc;";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt != null && dt.Rows.Count > 0) {
                return dt.Rows[0]["LCode"].ToString() == lcode;
            }
            return false;
        }

        public static bool PanelNoHas(string panelNo) {
            string sql = "select * from LableCode where PanelNo=@PanelNo";
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql,
                new System.Data.SqlClient.SqlParameter[] {
                    new System.Data.SqlClient.SqlParameter("@PanelNo",panelNo)
            });
            return dt != null && dt.Rows.Count > 0;
        }

        /// <summary>
        /// 从数据库读取指定交地和板的当前层的所有标签。
        /// </summary>
        /// <param name="tolocation">交地</param>
        /// <param name="panelDate">板号字头</param>
        /// <returns></returns>
        public static List<LableCode> GetLableCodesOfRecentFloor(string tolocation, PanelInfo pinfo) {
            string sql = @"select * from LableCode where ToLocation=@ToLocation and 
                                    PanelNo=@PanelNo and Floor=@Floor";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@ToLocation",tolocation),
                new SqlParameter("@PanelNo",pinfo.PanelNo),
                new SqlParameter("@Floor",pinfo.CurrFloor)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }
            return Helper.DataTableToObjList<LableCode>(dt);
        }

        public static PanelInfo GetTolactionCurrPanelNo(string tolocation, string dateShiftNo) {
            string sql = "select * from Panel where ToLocation=@ToLocation and PanelNo like @PanelDate+'%'  and Status=0 order by PanelNo desc";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@ToLocation",tolocation),
                new SqlParameter("@PanelDate",dateShiftNo)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }
            return Helper.DataTableToObjList<PanelInfo>(dt)[0];
        }

        public static bool SetPanelNo(string lCode) {
            string sql = @"update lablecode set PanelNo=tmp.PanelNo,updatedate=getdate(),Status=5 from 
                (select SequenceNo,ToLocation,PanelNo from lablecode where LCode=@lCode) tmp 
                where
                tmp.ToLocation = lablecode.ToLocation and
                tmp.SequenceNo > lablecode.SequenceNo and lablecode.Status = 0";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@lCode",lCode)};
            return DataAccess.CreateDataAccess.sa.NonQuery(sql, sp);
        }

        public static List<LableCode> GetLastLableCode(string panelNo, int currFloor) {
            string sql = "select * from LableCode where PanelNo=@PanelNo and Floor=@Floor";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",currFloor)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }
            return Helper.DataTableToObjList<LableCode>(dt);
        }

        public static string GetLastPanelNo(string shiftNo) {
            string sql = "select top 1 PanelNo from LableCode where PanelNo like @shiftNo+'%' group by PanelNo order by PanelNo desc";

            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@shiftNo",shiftNo)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return string.Empty;
            }
            return dt.Rows[0][0].ToString();
        }

        public static decimal GetFloorMaxDiameter(string panelNo, int currFloor) {
            string sql = "select  MAX(Diameter+Cz) " +
                "from LableCode where PanelNo = @PanelNo and Floor=@Floor";

            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",currFloor-1)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            return (decimal)dt.Rows[0][0];
        }

        public static decimal GetFloorHalfAvgLength(string panelNo, int currFloor) {
            string sql = "select  avg(Length)/2 " +
                "from LableCode where PanelNo = @PanelNo and Floor=@Floor";

            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",currFloor-1)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            return (decimal)dt.Rows[0][0];
        }

        public static int GetPanelMaxFloor(string panelNo) {
            string sql = "select max(Floor) from LableCode b where PanelNo = @PanelNo";

            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            return (int)dt.Rows[0][0];
        }

        public static decimal GetFloorMaxLength(string panelNo, int currFloor) {
            string sql = "select isnull(sum(Length),0) from (select (select max(Length) Length " +
                "from LableCode b where PanelNo=@PanelNo and b.Floor=a.Floor)Length " +
                "from LableCode a where PanelNo = @PanelNo and Floor<@Floor group by Floor)tmp";

            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",currFloor)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return 0;
            }
            return (decimal)dt.Rows[0][0];
        }

        public static bool SetPanelFloorFill(string panelNo, int floor) {
            string sql = "update LableCode set Status=5 where PanelNo='@PanelNo' and Floor=@Floor";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelNo",panelNo),
                new SqlParameter("@Floor",floor)};
            return DataAccess.CreateDataAccess.sa.NonQuery(sql, sp);
        }

        public static DataTable Query(string panelDate) {
            string sql = "select LCode Code,ToLocation,PanelNo,Floor,FloorIndex,Diameter,Coordinates,case status when 5 then '完成' else '未完成' end Finished from LableCode where PanelNo like @PanelDate+'%'  order by SequenceNo desc";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@PanelDate",panelDate)};
            return DataAccess.CreateDataAccess.sa.Query(sql, sp);
        }

        public static LableCode QueryByLCode(string lcode) {
            string sql = "select * from LableCode where lcode=@lcode";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@lcode",lcode)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
            if (dt == null || dt.Rows.Count < 1) {
                return null;
            }
            return Helper.DataTableToObjList<LableCode>(dt)[0];
        }

        public static List<string> QueryLabelcodeByPanelNo(string panelno) {
            string sql = "select lcode from LableCode where panelno=@panelno";
            SqlParameter[] sp = new SqlParameter[]{
                new SqlParameter("@panelno",panelno)};
            DataTable dt = DataAccess.CreateDataAccess.sa.Query(sql, sp);
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
    }
}
