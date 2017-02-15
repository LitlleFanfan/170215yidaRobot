/// laozhang_gz@139.com
/// created at 2016-11-14
/// update at: 2016-11-14

using System;
using ProduceComm.OPC;
using yidascan.DataAccess;

/// <summary>
/// 布卷尺寸类
/// </summary>
public class ClothRollSize
{
    public decimal diameter { get; set; }
    public decimal length { get; set; }

    public ClothRollSize()
    {
        diameter = 0;
        length = 0;
    }

    /// <summary>
    /// 取OPC的diameter和length两个量。
    /// 和static getFromOPC函数等价。
    /// </summary>
    public void getFromOPC(OPCClient client, OPCParam param)
    {
        var d = client.Read(param.ScanParam.Diameter);
        var l = client.Read(param.ScanParam.Length);

        if (d == null) { throw new Exception("读OPC的Diameter参数，返回nul值。"); }
        if (l == null) { throw new Exception("读OPC的Length参数，返回nul值。"); }

        diameter = decimal.Parse(d.ToString());
        length = decimal.Parse(l.ToString());
    }
}
