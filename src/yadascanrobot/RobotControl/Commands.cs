using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotControl {
    public class Commands {
        //Testing commands
        public const int CMD_SerialTest = -8;
        public const int CMD_FileTest = -7;
        public const int CMD_ClkAnnounceTest = -6;
        public const int CMD_DelayTest = -5;
        public const int CMD_SemTest = -4;
        public const int CMD_CrashTest = -3;
        public const int CMD_TaskCtrlTest = -2;
        public const int CMD_TimeTest = -1;

        public const int CMD_STOP = 0;

        //API Commands
        public const int CMD_MpGetVarData = 1;
        public const int CMD_MpReadIO = 2;
        public const int CMD_MpMonitor = 3;
        public const int CMD_MpGetPosVarData = 4;
        public const int CMD_MpGetAlarmStatus = 5;
        public const int CMD_MpGetAlarmCode = 6;
        public const int CMD_MpGetMode = 7;
        public const int CMD_MpGetCycle = 8;
        public const int CMD_MpGetServoPower = 9;
        public const int CMD_MpGetPlayStatus = 10;
        public const int CMD_MpGetMasterJob = 11;
        public const int CMD_MpGetCurJob = 12;
        public const int CMD_MpGetSpecialOpStatus = 13;
        public const int CMD_MpGetJobDate = 14;
        public const int CMD_MpGetCartPos = 15;
        public const int CMD_MpGetPulsePos = 16;
        public const int CMD_MpGetFBPulsePos = 17;
        public const int CMD_MpGetServoSpeed = 18;
        public const int CMD_MpGetFBSpeed = 19;
        public const int CMD_MpGetTorque = 20;
        public const int CMD_MpGetSysTimes = 21;
        public const int CMD_MpGetJogSpeed = 22;
        public const int CMD_MpGetJogCoord = 23;
        public const int CMD_MpPutVarData = 24;
        public const int CMD_MpWriteIO = 25;
        public const int CMD_MpPutPosVarData = 26;
        public const int CMD_MpCancelError = 27;
        public const int CMD_MpResetAlarm = 28;
        public const int CMD_MpSetMode = 29;
        public const int CMD_MpSetCycle = 30;
        public const int CMD_MpSetServoPower = 31;
        public const int CMD_MpSetMasterJob = 32;
        public const int CMD_MpSetCurJob = 33;
        public const int CMD_MpStartJob = 34;
        public const int CMD_MpHold = 35;
        public const int CMD_MpWaitForJobEnd = 36;
        public const int CMD_MpDeleteJob = 37;
        public const int CMD_MpConvertJobPtoR = 38;
        public const int CMD_MpConvertJobRtoP = 39;
        public const int CMD_MpIMOV = 40;
        public const int CMD_MpMOVJ = 41;
        public const int CMD_MpMOVL = 42;
        public const int CMD_MpPulseMOVJ = 43;
        public const int CMD_MpPulseMOVL = 44;
        public const int CMD_MpGetRobotTickCount = 45;
        public const int CMD_MpGetCarPosEx = 46;

    }

    public enum PosVarType {
        Robot = 5,
        Base = 6,
        Station = 7
    }

    public enum PosType {
        Pulse = 0,
        Base = 16,
        Robot = 17,
        Tool = 18,
        User = 19,
        MasterTool = 20
    }

    public enum VariableType {
        B = 1,
        I = 2,
        D = 3,
        R = 4,
        P_Robot = 5,
        P_Base = 6,
        P_Station = 7
    }

    public class PostionVar {
        decimal _sOrX;

        public PostionVar() { }

        public decimal sOrX {
            get { return _sOrX; }
        }

        public decimal SOrX {
            get { return _sOrX / 1000; }
        }
        decimal _lOrY;

        public decimal lOrY {
            get { return _lOrY; }
        }

        public decimal LOrY {
            get { return _lOrY / 1000; }
        }
        decimal _uOrZ;

        public decimal uOrZ {
            get { return _uOrZ; }
        }

        public decimal UOrZ {
            get { return _uOrZ / 1000; }
        }
        decimal _bOrRx;

        public decimal bOrRx {
            get { return _bOrRx; }
        }

        public decimal BOrRx {
            get { return _bOrRx / 10000; }
        }
        decimal _rOrRy;

        public decimal rOrRy {
            get { return _rOrRy; }
        }

        public decimal ROrRy {
            get { return _rOrRy / 10000; }
        }
        decimal _tOrRz;

        public decimal tOrRz {
            get { return _tOrRz; }
        }

        public decimal TOrRz {
            get { return _tOrRz / 10000; }
        }
        decimal _axis7;

        public decimal axis7 {
            get { return _axis7; }
        }

        public decimal Axis7 {
            get { return _axis7 / 1000; }
        }
        decimal _axis8;

        public decimal axis8 {
            get { return _axis8; }
        }

        public decimal Axis8 {
            get { return _axis8; }
        }
        bool noFlip;

        public bool NoFlip {
            get { return noFlip; }
        }
        bool lowArm;

        public bool LowArm {
            get { return lowArm; }
        }
        bool back;

        public bool Back {
            get { return back; }
        }
        bool rGt180;

        public bool RGt180 {
            get { return rGt180; }
        }
        bool tGt180;

        public bool TGt180 {
            get { return tGt180; }
        }

        /// <summary>
        /// 坐标初始化（毫米）
        /// </summary>
        /// <param name="x">坐标x轴位置</param>
        /// <param name="y">坐标y轴位置</param>
        /// <param name="z">坐标z轴位置</param>
        /// <param name="rx">x角度</param>
        /// <param name="ry">y角度</param>
        /// <param name="rz">z角度</param>
        public PostionVar(decimal x, decimal y, decimal z, decimal rx, decimal ry, decimal rz) {
            _sOrX = x * 1000;
            _lOrY = y * 1000;
            _uOrZ = z * 1000;
            _bOrRx = rx * 10000;
            _rOrRy = ry * 10000;
            _tOrRz = rz * 10000;
        }

        /// <summary>
        /// 坐标初始化（毫米）
        /// </summary>
        /// <param name="x">坐标x轴位置</param>
        /// <param name="y">坐标y轴位置</param>
        /// <param name="z">坐标z轴位置</param>
        /// <param name="rx">x角度</param>
        /// <param name="ry">y角度</param>
        /// <param name="rz">z角度</param>
        public PostionVar(decimal x, decimal y, decimal z, decimal rz, decimal axi7) {
            _sOrX = x * 1000;
            _lOrY = y * 1000;
            _uOrZ = z * 1000;
            _bOrRx = 1800000;
            _rOrRy = 0;
            _tOrRz = rz * 10000;
            _axis7 = axi7 * 1000;
        }

        /// <summary>
        /// 坐标初始化（微米）
        /// </summary>
        /// <param name="x">坐标x轴位置</param>
        /// <param name="y">坐标y轴位置</param>
        /// <param name="z">坐标z轴位置</param>
        /// <param name="rx">x角度</param>
        /// <param name="ry">y角度</param>
        /// <param name="rz">z角度</param>
        /// <param name="axis7"></param>
        /// <param name="axis8"></param>
        /// <param name="nf"></param>
        /// <param name="la"></param>
        /// <param name="b"></param>
        /// <param name="rgt"></param>
        /// <param name="tgt"></param>
        public PostionVar(decimal x, decimal y, decimal z, decimal rx, decimal ry, decimal rz, decimal axi7, decimal axi8,
            bool nf, bool la, bool b, bool rgt, bool tgt) {
            _sOrX = x;
            _lOrY = y;
            _uOrZ = z;
            _bOrRx = rx;
            _rOrRy = ry;
            _tOrRz = rz;
            _axis7 = axi7;
            _axis8 = axi8;
            noFlip = nf;
            lowArm = la;
            back = b;
            rGt180 = rgt;
            tGt180 = tgt;
        }

        /// <summary>
        /// 坐标初始化（毫米或微米）
        /// </summary>
        /// <param name="x">坐标x轴位置</param>
        /// <param name="y">坐标y轴位置</param>
        /// <param name="z">坐标z轴位置</param>
        /// <param name="rx">x角度</param>
        /// <param name="ry">y角度</param>
        /// <param name="rz">z角度</param>
        /// <param name="isUser">单位（true:毫米;）</param>
        internal PostionVar(decimal x, decimal y, decimal z, decimal rx, decimal ry, decimal rz, decimal axi7, bool isUser) {
            if (isUser) {
                _sOrX = x * 1000;
                _lOrY = y * 1000;
                _uOrZ = z * 1000;
                _bOrRx = rx * 10000;
                _rOrRy = ry * 10000;
                _tOrRz = rz * 10000;
                _axis7 = axi7 * 1000;
            } else {
                _sOrX = x;
                _lOrY = y;
                _uOrZ = z;
                _bOrRx = rx;
                _rOrRy = ry;
                _tOrRz = rz;
                _axis7 = axi7;
            }
        }
    }
}
