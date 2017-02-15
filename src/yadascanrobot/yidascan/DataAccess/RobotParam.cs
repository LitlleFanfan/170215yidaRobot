using System.Data;

namespace yidascan.DataAccess {
    public class RobotParam {
        public int PanelIndexNo;
        public int BaseIndex;
        public decimal Base;
        public decimal Rx;
        public decimal Ry;
        public decimal Rz;

        private static void MapPoint(DataRow row, RobotParam param) {
            param.Rx = decimal.Parse(row["RX"].ToString());
            param.Ry = decimal.Parse(row["RY"].ToString());
            param.Rz = decimal.Parse(row["RZ"].ToString());
            param.Base = decimal.Parse(row["Base"].ToString());
            param.BaseIndex = int.Parse(row["BaseIndex"].ToString());
        }

        public static RobotParam GetOrigin(int panelPoint) {
            const string SQL_1 = "select * from RobotParam where PanelIndexNo=@PanelIndexNo and Remark='Base'";
            var dtOrigin = new DataAccess().Query(SQL_1,
                new System.Data.SqlClient.SqlParameter("@PanelIndexNo", panelPoint));

            if (dtOrigin != null && dtOrigin.Rows.Count > 0) {
                var foo = new RobotParam();
                var row = dtOrigin.Rows[0];
                MapPoint(row, foo);
                return foo;
            } else { return null; }
        }

        public static RobotParam GetPoint(int panelPoint, int baseIndex) {
            const string SQL_2 = "select * from RobotParam where PanelIndexNo=@PanelIndexNo and BaseIndex=@BaseIndex";
            var dtPoint = DataAccess.CreateDataAccess.sa.Query(SQL_2,
                new System.Data.SqlClient.SqlParameter("@PanelIndexNo", panelPoint),
                new System.Data.SqlClient.SqlParameter("@BaseIndex", baseIndex));

            if (dtPoint != null && dtPoint.Rows.Count > 0) {
                var row = dtPoint.Rows[0];
                var foo = new RobotParam();
                MapPoint(row, foo);
                return foo;
            } else { return null; }
        }
    }
}
