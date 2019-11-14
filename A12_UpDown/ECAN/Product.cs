using Dongzr.MidiLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECAN;
using System.Threading;
using System.Diagnostics;

namespace A12_UpDown
{
    class Product
    {
        //电机是否升降
        public int GSM_UpDownMotorRunReq1 = 0;
        public int GSM_UpDownMotorRunReq2 = 0;
        public int GSM_UpDownMotorRunReq3 = 0;
        public int GSM_UpDownMotorRunReq4 = 0;
        public int GSM_UpDownMotorRunReq5 = 0;
        public int GSM_UpDownMotorRunReq6 = 0;
        public int GSM_UpDownMotorRunReq7 = 0;
        public int GSM_UpDownMotorRunReq8 = 0;
        public int GSM_UpDownMotorRunReq9 = 0;
        public int GSM_UpDownMotorRunReq10 = 0;
        
        //电机状态
        public int GSM_UpDownMotorSt1 = 0;
        public int GSM_UpDownMotorSt2 = 0;
        public int GSM_UpDownMotorSt3 = 0;
        public int GSM_UpDownMotorSt4 = 0;
        public int GSM_UpDownMotorSt5 = 0;
        public int GSM_UpDownMotorSt6 = 0;
        public int GSM_UpDownMotorSt7 = 0;
        public int GSM_UpDownMotorSt8 = 0;
        public int GSM_UpDownMotorSt9 = 0;
        public int GSM_UpDownMotorSt10 = 0;

        public const uint APPID = 0x30E;

        MmTimer timer;
        public CanProc can;
        public Product()
        {
            can = new CanProc();
            timer = TimerSetter.Setter(5, Timer_Tick);
        }
        public bool Open()
        {
            if (!can.open())
            {
                return false;
            }
            else
            {
                timer.Start();
                return true;
            }
        }
        public void Close()
        {
            can.Close();
            timer.Stop();
        }
        public void Error(string info)
        {
            throw new Exception(info);
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            can.AddBuf1(APPID);
            if (!can.RxFramesBuf1[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf1[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf1[APPID].Dequeue();
                    GSM_UpDownMotorRunReq1 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt1 = (frame.data[2] & 0x03);
                }
            }

            can.AddBuf2(APPID);
            if (!can.RxFramesBuf2[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf2[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf2[APPID].Dequeue();
                    GSM_UpDownMotorRunReq2 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt2 = (frame.data[2] & 0x03);
                }
            }

            can.AddBuf3(APPID);
            if (!can.RxFramesBuf3[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf3[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf3[APPID].Dequeue();
                    GSM_UpDownMotorRunReq3 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt3 = (frame.data[2] & 0x03);
                }
            }

            can.AddBuf4(APPID);
            if (!can.RxFramesBuf4[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf4[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf4[APPID].Dequeue();
                    GSM_UpDownMotorRunReq4 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt4 = (frame.data[2] & 0x03);
                }
            }

            can.AddBuf5(APPID);
            if (!can.RxFramesBuf5[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf5[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf5[APPID].Dequeue();
                    GSM_UpDownMotorRunReq5 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt5 = (frame.data[2] & 0x03);
                }
            }

            can.AddBuf6(APPID);
            if (!can.RxFramesBuf6[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf6[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf6[APPID].Dequeue();
                    GSM_UpDownMotorRunReq6 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt6 = (frame.data[2] & 0x03);
                }
            }

            can.AddBuf7(APPID);
            if (!can.RxFramesBuf7[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf7[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf7[APPID].Dequeue();
                    GSM_UpDownMotorRunReq7 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt7 = (frame.data[2] & 0x03);
                }
            }

            can.AddBuf8(APPID);
            if (!can.RxFramesBuf8[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf8[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf8[APPID].Dequeue();
                    GSM_UpDownMotorRunReq8 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt8 = (frame.data[2] & 0x03);
                }
            }

            can.AddBuf9(APPID);
            if (!can.RxFramesBuf9[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf9[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf9[APPID].Dequeue();
                    GSM_UpDownMotorRunReq9 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt9 = (frame.data[2] & 0x03);
                }
            }

            can.AddBuf10(APPID);
            if (!can.RxFramesBuf10[APPID].IsEmpty)
            {
                while (!can.RxFramesBuf10[APPID].IsEmpty)
                {
                    CAN_OBJ frame = can.RxFramesBuf10[APPID].Dequeue();
                    GSM_UpDownMotorRunReq10 = (frame.data[2] & 0x30) >> 4;
                    GSM_UpDownMotorSt10 = (frame.data[2] & 0x03);
                }
            }
        }
    }
}
