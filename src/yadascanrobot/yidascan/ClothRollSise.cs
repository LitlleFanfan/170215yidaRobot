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
    public void getFromOPC(IOpcClient client, OPCParam param)
    {
        diameter = client.ReadDecimal(param.ScanParam.Diameter);
        length = client.ReadDecimal(param.ScanParam.Length);
    }
}
