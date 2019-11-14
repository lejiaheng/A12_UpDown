using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dongzr.MidiLite;

/// <summary>
/// 自定义的一些扩展, 提供方便的语法糖函数等
/// </summary>
namespace Extend
{
    /// <summary>
    /// 返回一个状态值
    /// </summary>
    public delegate bool GetValueBool();
    /// <summary>
    /// 返回一个ad采样值, 8位 byte
    /// </summary>
    /// <returns></returns>
    public delegate byte GetValueByte();
    /// <summary>
    /// 返回一个AD采样值
    /// </summary>
    /// <returns></returns>
    public delegate HW GetValueHW();
    /// <summary>
    /// 返回一个表单, 为了能用工厂模式
    /// </summary>
    /// <returns></returns>
    public delegate Form GetValueForm();

    /// <summary>
    /// 按钮按下后做的事情
    /// </summary>
    public delegate void ButtonAction();
    /// <summary>
    /// 定时刷新的动作
    /// </summary>
    public delegate void TimeRefresh();

    /// <summary>
    /// 默认的任务返回结果, 增加info为了提供额外信息
    /// </summary>
    public struct Res
    {
        public bool ok;
        public string info;

        public static Res Success() => new Res { ok = true };
        public static Res Error(string info = "") => new Res { ok = false, info = info };

        public static void AssertOk(Res res) //语法糖, 适用于大量顺序任务执行时,用这个可以用try catch,而不是每个任务判断
        {
            if (!res.ok)
                throw new Exception(res.info);
        }
    }

    /// <summary>
    /// 表示AD采样的高低量, AD信号专用
    /// </summary>
    public enum AD
    {
        高,
        低,
        错误,
    }
    public struct HW
    {
        public byte value;
        public AD ad;
        public bool valid; //是否激活
    }

    public class MyQueue<T>
    {
        /* 自己实现的经常要用到的队列 
         * 基本功能:
         *      线程安全
         *      可以入队和出队
         *      判断是否为空
         *      可以清空
         *      (可以防止无限增加)
         * 
         * 实现方法:
         *      1: 默认的queue实现不是线程安全的, 并且有默认的 resize操作, 会出现空白数据
         *      2: 使用线程安全的 concurrentqueue可以实现, 但是因为用的是链表实现, 没有clear, 不好处理无线增大情况.
         *      3: 使用原生的array. 缺点是大小只能一致. 优势是内存开销一致, 且自己可以实现循环列表来处理clear问题
        * */
        const int MAXLENGTH = 10000; //默认buf大小
        T[] _queue = new T[MAXLENGTH]; //buf池
        int _head, _tail;

        public bool IsEmpty => _tail == _head;

        public void Enqueue(T item)
        {
            _queue[_head] = item;
            _head++;
            _head %= MAXLENGTH;
        }

        public T Dequeue()
        {
            T item = _queue[_tail++];
            _tail %= MAXLENGTH;
            return item;
        }

        public void Clear()
        {
            _tail = _head % MAXLENGTH; //清空只是简单的把指针指向head即可
        }

        public int BatchDequeue(List<T> buf, int max = MAXLENGTH)
        {
            int cnt = 0;
            while (!IsEmpty)
            {
                buf.Add(Dequeue());
                cnt++;
            }
            return cnt;
        }

        public void BatchEnqueue(IEnumerable<T> buf)
        {
            foreach (var item in buf)
                Enqueue(item);
        }
    }

    /// <summary>
    /// 自己封装的针对Dictionary的封装 要求对于 str和整形下标都能取值
    /// </summary>
    public class MyMap<T>
    {

    }

    public class MyTimer          //多媒体定时器 Mmtimer的封装.
    {
        MmTimer _timer;
        private int Interval;
        private Action tick;

        public void Start() => _timer.Start();
        public void Stop() => _timer.Stop();
        public void Dispose() => _timer.Dispose();
        public MyTimer(int Interval, EventHandler tick)
        {
            _timer = Setter(Interval, tick);
        }
        public MyTimer(int Interval, Action tick)
        {
            this.Interval = Interval;
            this.tick = tick;
        }
        public static MmTimer Setter(int Interval, EventHandler tick)
        {
            MmTimer timer = new MmTimer();
            timer.Mode = MmTimerMode.Periodic;
            timer.Interval = Interval;
            timer.Tick += tick;
            return timer;
        }
    }
    public static class MyExtend
    {
        //创建一些函数式编程的语法支持
        /// <summary>
        /// 类似python的Range语法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<int> Range(this int value)
        {
            return new List<int>(value.IntToEnum());
        }

        public static IEnumerable<int> IntToEnum(this int value)
        {
            for (int i = 0; i < value; i++)
                yield return i;
        }

        public static uint ToUint(this IEnumerable<byte> data)
        {
            return MyConvert.ToUInt32(data.Take(4).ToArray());
        }

        public static ushort ToUshort(this IEnumerable<byte> data)
        {
            return MyConvert.ToUshort(data.Take(2).ToArray());
        }

        public static byte ToByte(this IEnumerable<byte> data)
        {
            return data.First();
        }


        //扩展函数, 供linq使用
        public static byte StrToByte(this string value)
        {
            return Convert.ToByte(value);
        }

        public static uint StrToUint(this string value)
        {
            return Convert.ToUInt32(value);
        }

        public static double StrToDouble(this string value)
        {
            return Convert.ToDouble(value);
        }

        public static void Assert(this bool check, string info)
        {
            if (!check)
                throw new Exception(info);
        }
    }

    public class Cursor
    {
        /*
         * 自定义的游标, 为了方便针对byte的解析
         * */
        IEnumerable<byte> cursor;
        public Cursor(IEnumerable<byte> origin)
        {
            cursor = origin;
        }

        public IEnumerable<byte> Next(int cnt)
        {
            var res = cursor.Take(cnt);
            cursor = cursor.Skip(cnt);
            return res;
        }

        public byte NextByte => Next(1).ToByte();
        public ushort NextUshort => Next(2).ToUshort();
        public uint NextUint => Next(4).ToUint();

        public IEnumerable<byte> All => cursor;
    }

    class MyConvert
    {
        /*
         * 自定义的快捷转化函数, 要求功能多样, 用法简单
         *      1: 因为普通的Convert函数是基于Little Endian, 而我们的通常用Big Endian
         *      2: 便于自定义取数据
         */

        //所有Get函数, 都是从byte[]中取数据
        public static bool GetBool(byte[] data, int startByte, int bit) //返回某个字节, 某个位的布尔值
        {
            return Convert.ToBoolean(data[startByte] & 1 << bit);
        }

        public static byte GetByte(byte[] data, int startByte, int startBit, int len) //不考虑跨字节
        {
            return Convert.ToByte(data[startByte] >> (startBit - len + 1) & 0xff >> (8 - len));
        }

        public static ushort GetUshort(byte[] data, int startByte, int startBit, int len) //不考虑跨字节
        {
            int tmp = MyConvert.ToUshort(data, startByte);
            return Convert.ToUInt16(tmp >> (startBit - len + 9) & 0xffff >> (16 - len));
        }

        public static string GetAsciiString(IEnumerable<byte> data)
        {
            return new StringBuilder().Append(data.Select((i) => (char)i).ToArray()).ToString();
        }

        public static string GetAsciiString(byte[] data, int startByte, int len)
        {
            return GetAsciiString(data.Skip(startByte).Take(len));
        }

        public static string GetHexString(IEnumerable<byte> data)
        {
            return string.Join(" ", data.Select(i => i.ToString("X2")));
        }

        public static uint ToUInt32(byte[] data, int start = 0, int len = 4)
        {
            return BitConverter.ToUInt32(data.Skip(start).Take(len).Reverse().ToArray(), 0);
        }

        public static ushort ToUshort(byte[] data, int start = 0)
        {
            return BitConverter.ToUInt16(data.Skip(start).Take(2).Reverse().ToArray(), 0);
        }
        public static short ToShort(byte[] data, int start = 0)
        {
            return BitConverter.ToInt16(data.Skip(start).Take(2).Reverse().ToArray(), 0);
        }

        //将各种类型转换成byte[] 类型
        public static byte[] GetBytes(uint d)
        {
            byte[] tmp = BitConverter.GetBytes(d);
            return tmp.Reverse().ToArray();
        }
        public static byte[] GetBytes(ushort d)
        {
            byte[] tmp = BitConverter.GetBytes(d);
            return tmp.Reverse().ToArray();
        }

        public static byte[] HexStringToBytes(string hexString)
        {
            return hexString.Split(' ').Select(i => Convert.ToByte(i, 16)).ToArray();
        }
    }

}
