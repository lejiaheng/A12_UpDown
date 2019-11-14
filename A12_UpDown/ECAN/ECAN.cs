using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Dongzr.MidiLite;
using System.Threading;

using Extend;

namespace ECAN
{
    class TimerSetter
    {
        public static MmTimer Setter(int Interval, EventHandler tick)
        {
            MmTimer timer = new MmTimer();
            timer.Mode = MmTimerMode.Periodic;
            timer.Interval = Interval;
            timer.Tick += tick;
            return timer;
        }
    }
    [Flags]
    public enum ECANStatus : uint
    {
        STATUS_ERR = 0x00,
        STATUS_OK = 0x01,
    }

    [Flags]
    public enum ECANErrorCode : uint
    {
        ERR_CAN_OVERFLOW = 0x00000001, //CAN控制器内部FIFO溢出
        ERR_CAN_ERRALARM = 0x00000002, //CAN控制器错误报警
        ERR_CAN_PASSIVE = 0x00000004, //CAN控制器消极错误
        ERR_CAN_LOSE = 0x00000008, //CAN控制器仲裁丢失
        ERR_CAN_BUSERR = 0x00000010, //CAN控制器总线错误
        ERR_CAN_REG_FULL = 0x00000020, //CAN接收寄存器满
        ERR_CAN_REC_OVER = 0x00000040, //CAN接收寄存器溢出
        ERR_CAN_ACTIVE = 0x00000080, //CAN控制器主动错误
        ERR_DEVICEOPENED = 0x00000100, //设备已经打开
        ERR_DEVICEOPEN = 0x00000200, //打开设备错误
        ERR_DEVICENOTOPEN = 0x00000400, //设备没有打开
        ERR_BUFFEROVERFLOW = 0x00000800, //缓冲区溢出
        ERR_DEVICENOTEXIST = 0x00001000, //此设备不存在
        ERR_LOADKERNELDLL = 0x00002000, //装载动态库失败
        ERR_CMDFAILED = 0x00004000, //执行命令失败
        ERR_BUFFERCREATE = 0x00008000, //内存不足
        ERR_CANETE_PORTOPENED = 0x00010000, //端口已经被打开
        ERR_CANETE_INDEXUSED = 0x00020000, //设备索引号已经被占用
    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public struct CAN_OBJ
    {
        public UInt32 ID;              //报文ID                
        public UInt32 TimeStamp;       //接收到信息帧的时间标识, 从CAN控制器初始化开始计时
        public Byte TimeFlag;          //是否使用时间标识, 为1有效, 只在为接受帧时有意义
        public Byte SendType;          //发送帧类型, =0为正常发送, =1为单次发送, =2为自发自收 =3为单次自发自收 只在发送帧有效
        public Byte RemoteFlag;        //是否是远程帧
        public Byte ExternFlag;        //是否是扩展帧
        public Byte DataLen;           //数据长度(<=8), 即Data的长度
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Byte[] data;            //报文数据
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public Byte[] Reserved;        //系统保留

        public CAN_OBJ(uint m_id, byte[] m_data, byte len = 8, bool newData = false) //提供一个简单的构造
        {
            ID = m_id;
            TimeStamp = TimeFlag = 0; //无用
            SendType = 0; //默认正常发送, 只有在发送时有用.
            RemoteFlag = 0;
            ExternFlag = 0;//0数据帧  1扩展帧
            DataLen = len;
            if (newData) //自己可以选择每次重新生成一个8byte字节,还是复用.
            {
                data = new byte[8]; //注意这里不能 new byte[len] 结构体规定定长8
                m_data.CopyTo(data, 0);
            }
            else
                data = m_data;
            //Reserved = new byte[3];
            Reserved = null; //Reserved 这个字段无用
        }
    }

    public struct CAN_OBJ_ARRAY
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public CAN_OBJ[] array;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ERR_INFO
    {
        public ECANErrorCode ErrCode; //带枚举的错误码

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Passive_ErrData; //当产生的错误中有消极错误时表示为消极错误的错误标识数据。
        public byte ArLost_ErrData; //当产生的错误中有仲裁丢失错误时表示为仲裁丢失错误的错误标识数据。
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BOARD_INFO
    {
        public ushort hw_Version;     //硬件版本号，用16进制表示。比如0x0100表示V1.00。
        public ushort fw_Version;     //固件版本号，用16进制表示。
        public ushort dr_Version;     //驱动程序版本号，用16进制表示。
        public ushort in_Version;     //接口库版本号，用16进制表示。
        public ushort irq_Num;        //板卡所使用的中断号。
        public byte can_Num;          //表示有几路CAN通道。
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] str_Serial_Num; //此板卡的序列号。
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] str_hw_Type;    //硬件类型，比如“USBCAN V1.00”（注意：包括字符串结束符’\0’）。
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] Reserved;     //系统保留。

        public override string ToString() //板卡序列和硬件类型都含有 \0, 简单点不管
        {
            string s =
                $"硬件版本号: {hw_Version} \t固件版本号: {fw_Version} \t驱动版本号: {dr_Version} \t接口库版本号: {in_Version} \tCAN通道数量: {can_Num}\n";
            ////$"板卡序列号: {Encoding.ASCII.GetString(str_Serial_Num)}" +
            //$"板卡序列号: {MyConvert.GetAsciiString(str_Serial_Num)}" +
            ////$"硬件类型: {Encoding.ASCII.GetString(str_hw_Type)}";
            //$"硬件类型: {MyConvert.GetAsciiString(str_hw_Type)}";
            return s;
        }
    }

    public struct CAN_STATUS
    {
        public byte ErrInterrupt; //中断记录，读操作会清除。
        public byte regMode;      //CAN控制器模式寄存器。
        public byte regStatus;    //CAN控制器状态寄存器。
        public byte regALCapture; //CAN控制器仲裁丢失寄存器。
        public byte regECCapture; //CAN控制器错误寄存器。
        public byte regEWLimit;   //CAN控制器错误警告限制寄存器。
        public byte regRECounter; //CAN控制器接收错误寄存器。
        public byte regTECounter; //CAN控制器发送错误寄存器。
        public uint Reserved;     //系统保留。
    }

    public struct INIT_CONFIG
    {
        public uint AccCode;
        public uint AccMask;
        public uint Reserved;
        public byte Filter;
        public byte Timing0;
        public byte Timing1;
        public byte Mode;
    }
    public static class ECANDLL
    {
        // 使用ControlCAN.dll 作为共用的dll.
        public const uint ERROR_RES = 0xFFFFFFFF;

        [DllImport("ECanVci.dll", EntryPoint = "OpenDevice")] //此函数用以打开设备。
        public static extern ECANStatus OpenDevice(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 Reserved);

        [DllImport("ECanVci.dll", EntryPoint = "CloseDevice")] //此函数用以关闭设备。
        public static extern ECANStatus CloseDevice(
            UInt32 DeviceType,
            UInt32 DeviceInd);


        [DllImport("ECanVci.dll", EntryPoint = "InitCAN")] //此函数用以初始化指定的CAN。
        public static extern ECANStatus InitCAN(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            ref INIT_CONFIG InitConfig);

        [DllImport("ECanVci.dll", EntryPoint = "ReadBoardInfo")] //此函数用以获取设备信息。
        public static extern ECANStatus ReadBoardInfo(
            UInt32 DevType,
            UInt32 DevIndex,
            ref BOARD_INFO BoardInfo
            );

        [DllImport("ECanVci.dll", EntryPoint = "ReadErrInfo")] //此函数用以获取最后一次错误信息。
        public static extern ECANStatus ReadErrInfo(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            out ERR_INFO ReadErrInfo);


        [DllImport("ECanVci.dll", EntryPoint = "ReadCanStatus")] //此函数用以获取CAN状态。
        public static extern ECANStatus ReadCanStatus(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            out CAN_STATUS ReadCanStatus);

        //[DllImport("ECANVCI.dll", EntryPoint = "GetReference")] //此函数用以获取设备的相应参数。
        //[DllImport("ECANVCI.dll", EntryPoint = "SetReference")] //此函数用以设置设备的相应参数，主要处理不同设备的特定操作。

        [DllImport("ECanVci.dll", EntryPoint = "GetReceiveNum")] //此函数用以获取指定接收缓冲区中接收到但尚未被读取的帧数。
        public static extern UInt64 GetReceiveNum(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd);

        [DllImport("ECanVci.dll", EntryPoint = "ClearBuffer")] //此函数用以清空指定缓冲区。
        public static extern ECANStatus ClearBuffer(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd);

        [DllImport("ECanVci.dll", EntryPoint = "StartCAN")] //此函数用以启动CAN。
        public static extern ECANStatus StartCAN(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd);


        [DllImport("ECanVci.dll", EntryPoint = "Transmit")] //返回实际发送的帧数。单帧发送
        public static extern UInt32 Transmit(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            ref CAN_OBJ Send,
            UInt32 length);


        [DllImport("ECanVci.dll", EntryPoint = "Transmit")] //返回实际发送的帧数。多帧连续发送
        public static extern UInt32 Transmit_array(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            ref CAN_OBJ_ARRAY send,
            UInt32 length);

        [DllImport("ECanVci.dll", EntryPoint = "Receive")] //返回实际读取到的帧数。如果返回值为0xFFFFFFFF，则表示读取数据失败，有错误发生，请调用ReadErrInfo函数来获取错误码。
        public static extern UInt32 Receive(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            out CAN_OBJ Receive,
            UInt32 length,
            UInt32 WaitTime);

        [DllImport("ECanVci.dll", EntryPoint = "Receive")] // 返回实际读取到的帧数。如果返回值为0xFFFFFFFF，则表示读取数据失败，有错误发生，请调用ReadErrInfo函数来获取错误码。
        public static extern UInt32 Receive_array(
            UInt32 DeviceType,
            UInt32 DeviceInd,
            UInt32 CANInd,
            out CAN_OBJ_ARRAY Receive,
            UInt32 length,
            UInt32 WaitTime);

        [DllImport("ECanVci.dll", EntryPoint = "ResetCAN")]
        public static extern ECANStatus ResetCAN(
        UInt32 DeviceType,
        UInt32 DeviceInd,
        UInt32 CANInd);
    }

    public class CanProc
    {
        const byte device = 4; //设备: usbcan2=4 USBCAN1=3
        const byte TickCycle = 50; //刷新周期, 最小的时间间隔
        const byte BufSize = 10; //一次发送或接收最大值, 根据CAN_OBJ_ARRAY定义
        MyTimer timer;
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf1 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf2 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf3 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf4 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf5 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf6 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf7 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf8 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf9 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public Dictionary<uint, MyQueue<CAN_OBJ>> RxFramesBuf10 = new Dictionary<uint, MyQueue<CAN_OBJ>>();
        public CanProc()
        {
            timer = new MyTimer(TickCycle, timer_tick); //绑定定时器
        }
        #region //监控10路报文
        public void AddBuf1(uint i) //监控某一个报文
        {
            if (!RxFramesBuf1.ContainsKey(i))
                RxFramesBuf1[i] = new MyQueue<CAN_OBJ>();
        }
        public void AddBuf2(uint i) 
        {
            if (!RxFramesBuf2.ContainsKey(i))
                RxFramesBuf2[i] = new MyQueue<CAN_OBJ>();
        }
        public void AddBuf3(uint i)
        {
            if (!RxFramesBuf3.ContainsKey(i))
                RxFramesBuf3[i] = new MyQueue<CAN_OBJ>();
        }
        public void AddBuf4(uint i)
        {
            if (!RxFramesBuf4.ContainsKey(i))
                RxFramesBuf4[i] = new MyQueue<CAN_OBJ>();
        }
        public void AddBuf5(uint i)
        {
            if (!RxFramesBuf5.ContainsKey(i))
                RxFramesBuf5[i] = new MyQueue<CAN_OBJ>();
        }
        public void AddBuf6(uint i)
        {
            if (!RxFramesBuf6.ContainsKey(i))
                RxFramesBuf6[i] = new MyQueue<CAN_OBJ>();
        }
        public void AddBuf7(uint i)
        {
            if (!RxFramesBuf7.ContainsKey(i))
                RxFramesBuf7[i] = new MyQueue<CAN_OBJ>();
        }
        public void AddBuf8(uint i)
        {
            if (!RxFramesBuf8.ContainsKey(i))
                RxFramesBuf8[i] = new MyQueue<CAN_OBJ>();
        }
        public void AddBuf9(uint i)
        {
            if (!RxFramesBuf9.ContainsKey(i))
                RxFramesBuf9[i] = new MyQueue<CAN_OBJ>();
        }
        public void AddBuf10(uint i)
        {
            if (!RxFramesBuf10.ContainsKey(i))
                RxFramesBuf10[i] = new MyQueue<CAN_OBJ>();
        }
        #endregion   
        public bool open()
        {
            if (ECANDLL.OpenDevice(device, 0, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.OpenDevice(device, 1, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.OpenDevice(device, 2, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.OpenDevice(device, 3, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.OpenDevice(device, 4, 0) != ECANStatus.STATUS_OK)
                return false;

            INIT_CONFIG init_config = new INIT_CONFIG();
            init_config.AccCode = 0;            //验收码
            init_config.AccMask = 0xFFFFFF;     //屏蔽码
            init_config.Filter = 0;             //滤波方式
            init_config.Timing0 = 0;            //固定是500速率  
            init_config.Timing1 = 0x1C;
            init_config.Mode = 0;               //不屏蔽
            //初始化设备
            if (ECANDLL.InitCAN(device, 0, 0, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.InitCAN(device, 0, 1, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.InitCAN(device, 1, 0, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.InitCAN(device, 1, 1, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.InitCAN(device, 2, 0, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.InitCAN(device, 2, 1, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.InitCAN(device, 3, 0, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.InitCAN(device, 3, 1, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.InitCAN(device, 4, 0, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.InitCAN(device, 4, 1, ref init_config) != ECANStatus.STATUS_OK)
                return false;
            //开始读写
            if (ECANDLL.StartCAN(device, 0, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.StartCAN(device, 0, 1) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.StartCAN(device, 1, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.StartCAN(device, 1, 1) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.StartCAN(device, 2, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.StartCAN(device, 2, 1) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.StartCAN(device, 3, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.StartCAN(device, 3, 1) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.StartCAN(device, 4, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.StartCAN(device, 4, 1) != ECANStatus.STATUS_OK)
                return false;
            ECANDLL.ClearBuffer(device, 0, 0); //清除当前缓存
            ECANDLL.ClearBuffer(device, 0, 1);
            ECANDLL.ClearBuffer(device, 1, 0);
            ECANDLL.ClearBuffer(device, 1, 1);
            ECANDLL.ClearBuffer(device, 2, 0);
            ECANDLL.ClearBuffer(device, 2, 1);
            ECANDLL.ClearBuffer(device, 3, 0);
            ECANDLL.ClearBuffer(device, 3, 1);
            ECANDLL.ClearBuffer(device, 4, 0);
            ECANDLL.ClearBuffer(device, 4, 1);
            timer.Start();
            return true;
        }
        public bool Close()
        {
            timer.Stop();
            if (ECANDLL.CloseDevice(device, 0) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.CloseDevice(device, 1) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.CloseDevice(device, 2) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.CloseDevice(device, 3) != ECANStatus.STATUS_OK)
                return false;
            if (ECANDLL.CloseDevice(device, 4) != ECANStatus.STATUS_OK)
                return false;
            return true;
        }

        void timer_tick(object sender, EventArgs e)
        {
            ReceiveBatchFrame1(0, 0);
            ReceiveBatchFrame2(0, 1);
            ReceiveBatchFrame3(1, 0);
            ReceiveBatchFrame4(1, 1);
            ReceiveBatchFrame5(2, 0);
            ReceiveBatchFrame6(2, 1);
            ReceiveBatchFrame7(3, 0);
            ReceiveBatchFrame8(3, 1);
            ReceiveBatchFrame9(4, 0);
            ReceiveBatchFrame10(4, 1);
        }

        #region  //批量读10路数据
        void ReceiveBatchFrame1(uint DeviceInd, uint CANInd) //批量读
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf1.ContainsKey(id)) 
                        RxFramesBuf1[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize); 
        }
        void ReceiveBatchFrame2(uint DeviceInd, uint CANInd) 
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf2.ContainsKey(id)) //只关心订阅的报文
                        RxFramesBuf2[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize); 
        }

        void ReceiveBatchFrame3(uint DeviceInd, uint CANInd)
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf3.ContainsKey(id)) //只关心订阅的报文
                        RxFramesBuf3[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize);
        }
        void ReceiveBatchFrame4(uint DeviceInd, uint CANInd)
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf4.ContainsKey(id)) //只关心订阅的报文
                        RxFramesBuf4[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize);
        }

        void ReceiveBatchFrame5(uint DeviceInd, uint CANInd)
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf5.ContainsKey(id)) //只关心订阅的报文
                        RxFramesBuf5[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize);
        }

        void ReceiveBatchFrame6(uint DeviceInd, uint CANInd)
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf6.ContainsKey(id)) //只关心订阅的报文
                        RxFramesBuf6[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize);
        }
        void ReceiveBatchFrame7(uint DeviceInd, uint CANInd)
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf7.ContainsKey(id)) //只关心订阅的报文
                        RxFramesBuf7[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize);
        }

        void ReceiveBatchFrame8(uint DeviceInd, uint CANInd)
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf8.ContainsKey(id)) //只关心订阅的报文
                        RxFramesBuf8[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize);
        }
        void ReceiveBatchFrame9(uint DeviceInd, uint CANInd)
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf9.ContainsKey(id)) //只关心订阅的报文
                        RxFramesBuf9[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize);
        }
        void ReceiveBatchFrame10(uint DeviceInd, uint CANInd)
        {
            uint sCount;
            do
            {
                sCount = ECANDLL.Receive_array(device, DeviceInd, CANInd, out CAN_OBJ_ARRAY _ARRAY, BufSize, 0);
                for (int i = 0; i < sCount; i++)
                {
                    uint id = _ARRAY.array[i].ID;
                    if (RxFramesBuf10.ContainsKey(id)) //只关心订阅的报文
                        RxFramesBuf10[id].Enqueue(_ARRAY.array[i]);
                }
            } while (sCount == BufSize);
        }
        #endregion
    }
}
