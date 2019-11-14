using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A12_UpDown
{
    public partial class Form1 : Form
    {
        #region//初始声明
        Product Pdt = new Product();
        SQL SQL = new SQL();
        DataTable dataTable = new DataTable();

        string Number = null;  //编号从1~10W
        string Date = null;    //日期
        string PartNumber = null;  //零件号 二维码扫码所得
        string Uptime = null;  //上升时间
        string Downtime = null; //下降时间
        double offset = 0;//定时器+底层调用时间补偿
             

        string[] Platfortime = new string[11];
        string[] PlatforPartNumber = new string[11];

        int PlatformNumber = 0;  //台架号 从1~10
        string Path = AppDomain.CurrentDomain.BaseDirectory;

        bool IsStart;

        public int A1 = 0;
        public int A2 = 0;
        public int A3 = 0;
        public int A4 = 0;
        public int A5 = 0;
        public int A6 = 0;
        public int A7 = 0;
        public int A8 = 0;
        public int A9 = 0;
        public int A10 = 0;
        public float DB = 0;

        Stopwatch stopwatch1 = new Stopwatch();
        Stopwatch stopwatch2 = new Stopwatch();
        Stopwatch stopwatch3 = new Stopwatch();
        Stopwatch stopwatch4 = new Stopwatch();
        Stopwatch stopwatch5 = new Stopwatch();
        Stopwatch stopwatch6 = new Stopwatch();
        Stopwatch stopwatch7 = new Stopwatch();
        Stopwatch stopwatch8 = new Stopwatch();
        Stopwatch stopwatch9 = new Stopwatch();
        Stopwatch stopwatch10 = new Stopwatch();

        public int updowncount1 = 0;
        public int updowncount2 = 0;
        public int updowncount3 = 0;
        public int updowncount4 = 0;
        public int updowncount5 = 0;
        public int updowncount6 = 0;
        public int updowncount7 = 0;
        public int updowncount8 = 0;
        public int updowncount9 = 0;
        public int updowncount10 = 0;

        public bool IsStart1;
        public bool IsStart2;
        public bool IsStart3;
        public bool IsStart4;
        public bool IsStart5;
        public bool IsStart6;
        public bool IsStart7;
        public bool IsStart8;
        public bool IsStart9;
        public bool IsStart10;


        public long[] Uptimedate1 = new long[100];
        public long[] Uptimedate2 = new long[100];
        public long[] Uptimedate3 = new long[100];
        public long[] Uptimedate4 = new long[100];
        public long[] Uptimedate5 = new long[100];
        public long[] Uptimedate6 = new long[100];
        public long[] Uptimedate7 = new long[100];
        public long[] Uptimedate8 = new long[100];
        public long[] Uptimedate9 = new long[100];
        public long[] Uptimedate10 = new long[100];

        public long[] Downtimedate1 = new long[100];
        public long[] Downtimedate2 = new long[100];
        public long[] Downtimedate3 = new long[100];
        public long[] Downtimedate4 = new long[100];
        public long[] Downtimedate5 = new long[100];
        public long[] Downtimedate6 = new long[100];
        public long[] Downtimedate7 = new long[100];
        public long[] Downtimedate8 = new long[100];
        public long[] Downtimedate9 = new long[100];
        public long[] Downtimedate10 = new long[100];
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (!IsStart)
            {
                try
                {
                    if (Pdt.Open())
                    {
                        ListBox_AddMsgToListBox("CAN卡连接成功！！！");
                    }
                    else
                    {
                        ListBox_AddMsgToListBox("!!!!!!!!!!!!!!!!!!CAN卡连接失败！！！");
                    }
                    timer1.Start();
                    ButtonStart.Text = "关闭设备";
                    ButtonStart.BackColor = Color.Red;
                    IsStart = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"打开失败: {ex.Message}", "Error", MessageBoxButtons.OK);
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show("确认退出?", "离开", MessageBoxButtons.OKCancel);
                if (dr != DialogResult.OK)
                    return;
                timer1.Stop();
                ButtonStart.Text = "打开设备";
                ButtonStart.BackColor = Color.Green;
                IsStart = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e) //10ms 界面状态刷新
        {
            #region //台架1
            if (IsStart1)
            {
                if (Pdt.GSM_UpDownMotorRunReq1 == 0x01 && Pdt.GSM_UpDownMotorSt1 == 0x00 && A1 == 0)
                {
                    stopwatch1.Reset();
                    stopwatch1.Start();
                    A1 = 1;
                    label19.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq1 == 0x01 && Pdt.GSM_UpDownMotorSt1 == 0x02 && A1 == 1)//上升到顶
                {
                    stopwatch1.Stop();
                    label20.Text = "上升时间：" + stopwatch1.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate1[updowncount1] = stopwatch1.ElapsedMilliseconds;
                    A1 = 2;
                    label19.Text = "升降状态：上升到顶";
                    stopwatch1.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq1 == 0x02 && Pdt.GSM_UpDownMotorSt1 == 0x00 && A1 == 2)
                {
                    stopwatch1.Reset();
                    stopwatch1.Start();
                    label19.Text = "升降状态：开始下降";
                    A1 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq1 == 0x02 && Pdt.GSM_UpDownMotorSt1 == 0x02 && A1 == 3)//下降到底
                {
                    stopwatch1.Stop();
                    label21.Text = "下降时间：" + stopwatch1.ElapsedMilliseconds.ToString() + " ms";
                    A1 = 0;
                    label19.Text = "升降状态：下降到底";
                    Downtimedate1[updowncount1] = stopwatch1.ElapsedMilliseconds;
                    updowncount1++;
                    label3.Text = "升降次数： " + updowncount1.ToString();
                    if (updowncount1 >= 10)
                    {
                        A1 = 0;
                        updowncount1 = 0;
                        double uptimeavg = Uptimedate1.Average()-offset;
                        double downtimeavg = Downtimedate1.Average()-offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "1");
                        SQLUpDate("'"+uptimeavg.ToString()+"'", "'" + downtimeavg.ToString() + "'", Platfortime[1], PlatforPartNumber[1], "1");
                        label28.Text = "台架1状态:结束";
                        IsStart1 = false;
                    }
                }
            }
            #endregion
            #region //台架2
            if (IsStart2)
            {
                if (Pdt.GSM_UpDownMotorRunReq2 == 0x01 && Pdt.GSM_UpDownMotorSt2 == 0x00 && A2 == 0)
                {
                    stopwatch2.Reset();
                    stopwatch2.Start();
                    A2 = 1;
                    label35.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq2 == 0x01 && Pdt.GSM_UpDownMotorSt2 == 0x02 && A2 == 1)
                {
                    stopwatch2.Stop();
                    label34.Text = "上升时间：" + stopwatch2.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate2[updowncount2] = stopwatch2.ElapsedMilliseconds;
                    A2 = 2;
                    label35.Text = "升降状态：上升到顶";
                    stopwatch2.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq2 == 0x02 && Pdt.GSM_UpDownMotorSt2 == 0x00 && A2 == 2)
                {
                    stopwatch2.Reset();
                    stopwatch2.Start();
                    label35.Text = "升降状态：开始下降";
                    A2 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq2 == 0x02 && Pdt.GSM_UpDownMotorSt2 == 0x02 && A2 == 3)//下降到底
                {
                    stopwatch2.Stop();
                    label33.Text = "下降时间：" + stopwatch2.ElapsedMilliseconds.ToString() + " ms";
                    Downtimedate2[updowncount2] = stopwatch2.ElapsedMilliseconds;
                    A2 = 0;
                    label35.Text = "升降状态：下降到底";
                    updowncount2++;
                    label4.Text = "升降次数： " + updowncount2.ToString();
                    if (updowncount2 >= 10)
                    {
                        A2 = 0;
                        updowncount2 = 0;
                        double uptimeavg = Uptimedate2.Average() - offset;
                        double downtimeavg = Downtimedate2.Average() - offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "2");
                        SQLUpDate("'" + uptimeavg.ToString() + "'", "'" + downtimeavg.ToString() + "'", Platfortime[2], PlatforPartNumber[2], "2");
                        label36.Text = "台架2状态:结束";
                        IsStart2 = false;
                    }
                }
            }
            #endregion
            #region  //台架3
            if (IsStart3)
            {
                if (Pdt.GSM_UpDownMotorRunReq3 == 0x01 && Pdt.GSM_UpDownMotorSt3 == 0x00 && A3 == 0)
                {
                    stopwatch3.Reset();
                    stopwatch3.Start();
                    A3 = 1;
                    label39.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq3 == 0x01 && Pdt.GSM_UpDownMotorSt3 == 0x02 && A3 == 1)
                {
                    stopwatch3.Stop();
                    label38.Text = "上升时间：" + stopwatch3.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate3[updowncount3] = stopwatch3.ElapsedMilliseconds;
                    A3 = 2;
                    label39.Text = "升降状态：上升到顶";
                    stopwatch3.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq3 == 0x02 && Pdt.GSM_UpDownMotorSt3 == 0x00 && A3 == 2)
                {
                    stopwatch3.Reset();
                    stopwatch3.Start();
                    label39.Text = "升降状态：开始下降";
                    A3 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq3 == 0x02 && Pdt.GSM_UpDownMotorSt3 == 0x02 && A3 == 3)//下降到底
                {
                    stopwatch3.Stop();
                    label37.Text = "下降时间：" + stopwatch3.ElapsedMilliseconds.ToString() + " ms";
                    Downtimedate3[updowncount3] = stopwatch3.ElapsedMilliseconds;
                    A3 = 0;
                    label39.Text = "升降状态：下降到底";
                    updowncount3++;
                    label5.Text = "升降次数： " + updowncount3.ToString();
                    if (updowncount3 >= 10)
                    {
                        A3 = 0;
                        updowncount3 = 0;
                        double uptimeavg = Uptimedate3.Average() - offset;
                        double downtimeavg = Downtimedate3.Average() - offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "3");
                        SQLUpDate("'" + uptimeavg.ToString() + "'", "'" + downtimeavg.ToString() + "'", Platfortime[3], PlatforPartNumber[3], "3");
                        label40.Text = "台架3状态:结束";
                        IsStart3 = false;
                    }
                }
            }
            #endregion 
            #region  //台架4
            if (IsStart4)
            {
                if (Pdt.GSM_UpDownMotorRunReq4 == 0x01 && Pdt.GSM_UpDownMotorSt4 == 0x00 && A4 == 0)
                {
                    stopwatch4.Reset();
                    stopwatch4.Start();
                    A4 = 1;
                    label43.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq4 == 0x01 && Pdt.GSM_UpDownMotorSt4 == 0x02 && A4 == 1)
                {
                    stopwatch4.Stop();
                    label42.Text = "上升时间：" + stopwatch4.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate4[updowncount4] = stopwatch4.ElapsedMilliseconds;
                    A4 = 2;
                    label43.Text = "升降状态：上升到顶";
                    stopwatch4.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq4 == 0x02 && Pdt.GSM_UpDownMotorSt4 == 0x00 && A4 == 2)
                {
                    stopwatch4.Reset();
                    stopwatch4.Start();
                    label43.Text = "升降状态：开始下降";
                    A4 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq4 == 0x02 && Pdt.GSM_UpDownMotorSt4 == 0x02 && A4 == 3)//下降到底
                {
                    stopwatch4.Stop();
                    label41.Text = "下降时间：" + stopwatch4.ElapsedMilliseconds.ToString() + " ms";
                    Downtimedate4[updowncount4] = stopwatch4.ElapsedMilliseconds;
                    A4 = 0;
                    label43.Text = "升降状态：下降到底";
                    updowncount4++;
                    label6.Text = "升降次数： " + updowncount4.ToString();
                    if (updowncount4 >= 10)
                    {
                        A4 = 0;
                        updowncount4 = 0; 
                        double uptimeavg = Uptimedate4.Average() - offset;
                        double downtimeavg = Downtimedate4.Average() - offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "4");
                        SQLUpDate("'" + uptimeavg.ToString() + "'", "'" + downtimeavg.ToString() + "'", Platfortime[4], PlatforPartNumber[4], "4");
                        label44.Text = "台架4状态:结束";
                        IsStart4 = false;
                    }
                }
            }
            #endregion 
            #region  //台架5
            if (IsStart5)
            {
                if (Pdt.GSM_UpDownMotorRunReq5 == 0x01 && Pdt.GSM_UpDownMotorSt5 == 0x00 && A5 == 0)
                {
                    stopwatch5.Reset();
                    stopwatch5.Start();
                    A5 = 1;
                    label47.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq5 == 0x01 && Pdt.GSM_UpDownMotorSt5 == 0x02 && A5 == 1)
                {
                    stopwatch5.Stop();
                    label46.Text = "上升时间：" + stopwatch5.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate5[updowncount5] = stopwatch5.ElapsedMilliseconds;
                    A5 = 2;
                    label47.Text = "升降状态：上升到顶";
                    stopwatch5.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq5 == 0x02 && Pdt.GSM_UpDownMotorSt5 == 0x00 && A5 == 2)
                {
                    stopwatch5.Reset();
                    stopwatch5.Start();
                    label47.Text = "升降状态：开始下降";
                    A5 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq5 == 0x02 && Pdt.GSM_UpDownMotorSt5 == 0x02 && A5 == 3)//下降到底
                {
                    stopwatch5.Stop();
                    label45.Text = "下降时间：" + stopwatch5.ElapsedMilliseconds.ToString() + " ms";
                    Downtimedate5[updowncount5] = stopwatch5.ElapsedMilliseconds;
                    A5 = 0;
                    label47.Text = "升降状态：下降到底";
                    updowncount5++;
                    label7.Text = "升降次数： " + updowncount5.ToString();
                    if (updowncount5 >= 10)
                    {
                        A5 = 0;
                        updowncount5 = 0;
                        double uptimeavg = Uptimedate5.Average() - offset;
                        double downtimeavg = Downtimedate5.Average() - offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "5");
                        SQLUpDate("'" + uptimeavg.ToString() + "'", "'" + downtimeavg.ToString() + "'", Platfortime[5], PlatforPartNumber[5], "5");
                        label48.Text = "台架5状态:结束";
                        IsStart5 = false;
                    }
                }
            }
            #endregion 
            #region  //台架6
            if (IsStart6)
            {
                if (Pdt.GSM_UpDownMotorRunReq6 == 0x01 && Pdt.GSM_UpDownMotorSt6 == 0x00 && A6 == 0)
                {
                    stopwatch6.Reset();
                    stopwatch6.Start();
                    A6 = 1;
                    label51.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq6 == 0x01 && Pdt.GSM_UpDownMotorSt6 == 0x02 && A6 == 1)
                {
                    stopwatch6.Stop();
                    label50.Text = "上升时间：" + stopwatch6.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate6[updowncount6] = stopwatch6.ElapsedMilliseconds;
                    A6 = 2;
                    label51.Text = "升降状态：上升到顶";
                    stopwatch6.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq6 == 0x02 && Pdt.GSM_UpDownMotorSt6 == 0x00 && A6 == 2)
                {
                    stopwatch6.Reset();
                    stopwatch6.Start();
                    label51.Text = "升降状态：开始下降";
                    A6 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq6 == 0x02 && Pdt.GSM_UpDownMotorSt6 == 0x02 && A6 == 3)//下降到底
                {
                    stopwatch6.Stop();
                    label49.Text = "下降时间：" + stopwatch6.ElapsedMilliseconds.ToString() + " ms";
                    Downtimedate6[updowncount6] = stopwatch6.ElapsedMilliseconds;
                    A6 = 0;
                    label51.Text = "升降状态：下降到底";
                    updowncount6++;
                    label8.Text = "升降次数： " + updowncount6.ToString();
                    if (updowncount6 >= 10)
                    {
                        A6 = 0;
                        updowncount6 = 0;
                        double uptimeavg = Uptimedate6.Average() - offset;
                        double downtimeavg = Downtimedate6.Average() - offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "6");
                        SQLUpDate("'" + uptimeavg.ToString() + "'", "'" + downtimeavg.ToString() + "'", Platfortime[6], PlatforPartNumber[6], "6");
                        label52.Text = "台架6状态:结束";
                        IsStart6 = false;
                    }
                }
            }
            #endregion
            #region  //台架7
            if (IsStart7)
            {
                if (Pdt.GSM_UpDownMotorRunReq7 == 0x01 && Pdt.GSM_UpDownMotorSt7 == 0x00 && A7 == 0)
                {
                    stopwatch7.Reset();
                    stopwatch7.Start();
                    A7 = 1;
                    label55.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq7 == 0x01 && Pdt.GSM_UpDownMotorSt7 == 0x02 && A7 == 1)
                {
                    stopwatch7.Stop();
                    label54.Text = "上升时间：" + stopwatch7.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate7[updowncount7] = stopwatch7.ElapsedMilliseconds;
                    A7 = 2;
                    label55.Text = "升降状态：上升到顶";
                    stopwatch7.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq7 == 0x02 && Pdt.GSM_UpDownMotorSt7 == 0x00 && A7 == 2)
                {
                    stopwatch7.Reset();
                    stopwatch7.Start();
                    label55.Text = "升降状态：开始下降";
                    A7 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq7 == 0x02 && Pdt.GSM_UpDownMotorSt7 == 0x02 && A7 == 3)//下降到底
                {
                    stopwatch7.Stop();
                    label53.Text = "下降时间：" + stopwatch7.ElapsedMilliseconds.ToString() + " ms";
                    Downtimedate7[updowncount7] = stopwatch7.ElapsedMilliseconds;
                    A7 = 0;
                    label55.Text = "升降状态：下降到底";
                    updowncount7++;
                    label9.Text = "升降次数： " + updowncount7.ToString();
                    if (updowncount7 >= 10)
                    {
                        A7 = 0;
                        updowncount7 = 0;
                        double uptimeavg = Uptimedate7.Average() - offset;
                        double downtimeavg = Downtimedate7.Average() - offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "7");
                        SQLUpDate("'" + uptimeavg.ToString() + "'", "'" + downtimeavg.ToString() + "'", Platfortime[7], PlatforPartNumber[7], "7");
                        label56.Text = "台架7状态:结束";
                        IsStart7 = false;
                    }
                }
            }
            #endregion
            #region  //台架8
            if (IsStart8)
            {
                if (Pdt.GSM_UpDownMotorRunReq8 == 0x01 && Pdt.GSM_UpDownMotorSt8 == 0x00 && A8 == 0)
                {
                    stopwatch8.Reset();
                    stopwatch8.Start();
                    A8 = 1;
                    label59.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq8 == 0x01 && Pdt.GSM_UpDownMotorSt8 == 0x02 && A8 == 1)
                {
                    stopwatch8.Stop();
                    label58.Text = "上升时间：" + stopwatch8.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate8[updowncount8] = stopwatch8.ElapsedMilliseconds;
                    A8 = 2;
                    label59.Text = "升降状态：上升到顶";
                    stopwatch8.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq8 == 0x02 && Pdt.GSM_UpDownMotorSt8 == 0x00 && A8 == 2)
                {
                    stopwatch8.Reset();
                    stopwatch8.Start();
                    label59.Text = "升降状态：开始下降";
                    A8 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq8 == 0x02 && Pdt.GSM_UpDownMotorSt8 == 0x02 && A8 == 3)//下降到底
                {
                    stopwatch8.Stop();
                    label57.Text = "下降时间：" + stopwatch8.ElapsedMilliseconds.ToString() + " ms";
                   Downtimedate8[updowncount8] = stopwatch8.ElapsedMilliseconds;
                    A8 = 0;
                    label59.Text = "升降状态：下降到底";
                    updowncount8++;
                    label10.Text = "升降次数： " + updowncount8.ToString();
                    if (updowncount8 >= 10)
                    {
                        A8 = 0;
                        updowncount8 = 0;
                        double uptimeavg = Uptimedate8.Average() - offset;
                        double downtimeavg = Downtimedate8.Average() - offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "8");
                        SQLUpDate("'" + uptimeavg.ToString() + "'", "'" + downtimeavg.ToString() + "'", Platfortime[8], PlatforPartNumber[8], "8");
                        label60.Text = "台架8状态:结束";
                        IsStart8 = false;
                    }
                }
            }
            #endregion
            #region  //台架9
            if (IsStart9)
            {
                if (Pdt.GSM_UpDownMotorRunReq9 == 0x01 && Pdt.GSM_UpDownMotorSt9 == 0x00 && A9 == 0)
                {
                    stopwatch9.Reset();
                    stopwatch9.Start();
                    A9 = 1;
                    label63.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq9 == 0x01 && Pdt.GSM_UpDownMotorSt9 == 0x02 && A9 == 1)
                {
                    stopwatch9.Stop();
                    label62.Text = "上升时间：" + stopwatch9.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate9[updowncount9] = stopwatch9.ElapsedMilliseconds;
                    A9 = 2;
                    label63.Text = "升降状态：上升到顶";
                    stopwatch9.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq9 == 0x02 && Pdt.GSM_UpDownMotorSt9 == 0x00 && A9 == 2)
                {
                    stopwatch9.Reset();
                    stopwatch9.Start();
                    label63.Text = "升降状态：开始下降";
                    A9 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq9 == 0x02 && Pdt.GSM_UpDownMotorSt9 == 0x02 && A9 == 3)//下降到底
                {
                    stopwatch9.Stop();
                    label61.Text = "下降时间：" + stopwatch9.ElapsedMilliseconds.ToString() + " ms";
                    Downtimedate9[updowncount9] = stopwatch9.ElapsedMilliseconds;
                    A9 = 0;
                    label63.Text = "升降状态：下降到底";
                    updowncount9++;
                    label11.Text = "升降次数： " + updowncount9.ToString();
                    if (updowncount9 >= 10)
                    {
                        A9 = 0;
                        updowncount9 = 0;
                        double uptimeavg = Uptimedate9.Average() - offset;
                        double downtimeavg = Downtimedate9.Average() - offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "9");
                        SQLUpDate("'" + uptimeavg.ToString() + "'", "'" + downtimeavg.ToString() + "'", Platfortime[9], PlatforPartNumber[9], "9");
                        label64.Text = "台架9状态:结束";
                        IsStart9 = false;
                    }
                }
            }
            #endregion
            #region  //台架10
            if (IsStart10)
            {
                if (Pdt.GSM_UpDownMotorRunReq10 == 0x01 && Pdt.GSM_UpDownMotorSt10 == 0x00 && A10 == 0)
                {
                    stopwatch10.Reset();
                    stopwatch10.Start();
                    A10 = 1;
                    label67.Text = "升降状态：开始上升";
                }
                else if (Pdt.GSM_UpDownMotorRunReq10 == 0x01 && Pdt.GSM_UpDownMotorSt10 == 0x02 && A10 == 1)
                {
                    stopwatch10.Stop();
                    label66.Text = "上升时间：" + stopwatch10.ElapsedMilliseconds.ToString() + " ms";
                    Uptimedate10[updowncount10] = stopwatch10.ElapsedMilliseconds;
                    A10 = 2;
                    label67.Text = "升降状态：上升到顶";
                    stopwatch10.Reset();
                }
                if (Pdt.GSM_UpDownMotorRunReq10 == 0x02 && Pdt.GSM_UpDownMotorSt10 == 0x00 && A10 == 2)
                {
                    stopwatch10.Reset();
                    stopwatch10.Start();
                    label67.Text = "升降状态：开始下降";
                    A10 = 3;
                }
                else if (Pdt.GSM_UpDownMotorRunReq10 == 0x02 && Pdt.GSM_UpDownMotorSt10 == 0x02 && A10 == 3)//下降到底
                {
                    stopwatch10.Stop();
                    label65.Text = "下降时间：" + stopwatch10.ElapsedMilliseconds.ToString() + " ms";
                    Downtimedate10[updowncount10] = stopwatch10.ElapsedMilliseconds;
                    A10 = 0;
                    label67.Text = "升降状态：下降到底";
                    updowncount10++;
                    label12.Text = "升降次数： " + updowncount10.ToString();
                    if (updowncount10 >= 10)
                    {
                        A10 = 0;
                        updowncount10 = 0;
                        double uptimeavg = Uptimedate10.Average() - offset;
                        double downtimeavg = Downtimedate10.Average() - offset;
                        UpDownTimeCheck(uptimeavg, downtimeavg, "10");
                        SQLUpDate("'" + uptimeavg.ToString() + "'", "'" + downtimeavg.ToString() + "'", Platfortime[10], PlatforPartNumber[10], "10");
                        label68.Text = "台架10状态:结束";
                        IsStart10 = false;
                    }
                }
            }
            #endregion 
        }

        private void UpDownTimeCheck(double uptimeavg,double downtimeavg,string A) //升降时间判断
        {
            if (uptimeavg > 1800)
            {
                ListBox_AddMsgToListBox("台架'"+A+"'上升时间过大");
            }
            else if (uptimeavg < 1200)
            {
                ListBox_AddMsgToListBox("台架'" + A + "'上升时间偏小");
            }
            else
            {
                ListBox_AddMsgToListBox("台架'" + A + "'上升时间正常");
            }
            if (downtimeavg > 1800)
            {
                ListBox_AddMsgToListBox("台架'" + A + "'下降时间过大");
            }
            else if (downtimeavg < 1200)
            {
                ListBox_AddMsgToListBox("台架'" + A + "'下降时间偏小");
            }
            else
            {
                ListBox_AddMsgToListBox("台架2'" + A + "'下降时间正常");
            }
        } 
        private void ListBox_AddMsgToListBox(string MsgStr) //添加listbox信息最多保留500条消息
        {
            listBox1.Items.Add(MsgStr);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            if (listBox1.Items.Count > 500)
            {
                listBox1.Items.RemoveAt(0);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e) //自动查询
        {
            if (textBox2.Text.Length == 21)
            {
                if (0 == QrCode_IsCorrectCode(textBox2.Text))
                {
                    PartNumber = textBox2.Text.ToString();
                    string str1 = "SELECT 编号,日期,零件号,上升时间ms,下降时间ms FROM DPLA12 WHERE 零件号='" + PartNumber + "'";
                    dataTable = SQL.ExecuteQuery(str1);
                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("无此零件号", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        dataGridView1.DataSource = dataTable;
                        dataGridView1.AutoResizeColumns();
                        textBox2.Clear();
                    }
                }
            }
        }
        private string SQLADD()   //自动添加数据库
        {
            Number = File.ReadAllText(@"" + Path + "编号文本.txt");
            Date = DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "  " + DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + "";
            PartNumber = QRCTextBox.Text.ToString();
            Uptime = null;
            Downtime = null;
            String str1 = "INSERT INTO DPLA12 (编号,日期,零件号,上升时间ms,下降时间ms) VALUES('" + Number + "','" + Date + "','" + PartNumber + "','" + Uptime + "','" + Downtime + "')";

            String str2 = "SELECT 日期 FROM DPLA12 WHERE 日期 = '" + Date + "'and 零件号 = '" + PartNumber + "'";
            dataTable = SQL.ExecuteQuery(str2);
            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                ListBox_AddMsgToListBox("添加失败！" + PartNumber + "一分钟前零件号已存在！");
            }
            else
            {
                if (SQL.ExecuteUpdate(str1) != 0)
                {
                    PlatformNumber++;
                    if (PlatformNumber > 10)
                    {
                        PlatformNumber = 1;
                    }
                    Platfortime[PlatformNumber] = Date;
                    PlatforPartNumber[PlatformNumber] = PartNumber;
                    switch (PlatformNumber)
                    {
                        case 1: IsStart1 = true; label28.Text = "台架1状态:开始"; break;
                        case 2: IsStart2 = true; label36.Text = "台架2状态:开始"; break;
                        case 3: IsStart3 = true; label40.Text = "台架3状态:开始"; break;
                        case 4: IsStart4 = true; label44.Text = "台架4状态:开始"; break;
                        case 5: IsStart5 = true; label48.Text = "台架5状态:开始"; break;
                        case 6: IsStart6 = true; label52.Text = "台架6状态:开始"; break;
                        case 7: IsStart7 = true; label56.Text = "台架7状态:开始"; break;
                        case 8: IsStart8 = true; label60.Text = "台架8状态:开始"; break;
                        case 9: IsStart9 = true; label64.Text = "台架9状态:开始"; break;
                        case 10: IsStart10 = true; label68.Text = "台架10状态:开始"; break;
                    }

                    ListBox_AddMsgToListBox("添加成功零件号" + PartNumber + "   时间" + Date + "   总计编号" + Number + "   台架号" + PlatformNumber + "");
                    int NumberCount = Convert.ToInt32(Number);
                    NumberCount++;
                    FileStream fs = new FileStream(@"" + Path + "编号文本.txt", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write("" + NumberCount + "");
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                    return "OK";
                }
            }
            return "NOK";
        }

        private void SQLUpDate(string Uptime, string Downtime, string Platfortime, string PlatforPartNumber, string PlatformNumber)//更改数据
        {
            string str = "Update DPLA12 set 上升时间ms = '" + Uptime + "',下降时间ms='" + Downtime + "' WHERE 日期='" + Platfortime + "'and 零件号='" + PlatforPartNumber + "'";
            if (SQL.ExecuteUpdate(str) != 0)
            {
                ListBox_AddMsgToListBox("台架" + PlatformNumber.ToString() + "升降耐久完成！！！上升下降数据更新完成！！！");
            }
        }

        private void QRCTextBox_TextChanged(object sender, EventArgs e) //事件触发
        {
            if (IsStart)
            {
                if (QRCTextBox.Text.Length == 21)
                {
                    this.QRCTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    if (0 == QrCode_IsCorrectCode(QRCTextBox.Text))
                    {
                        if (SQLADD() == "OK")
                        {

                        }
                    }
                    else
                    {

                    }
                    QRCTextBox.Clear();
                }
                else if (QRCTextBox.Text.Length > 21)
                {
                    ListBox_AddMsgToListBox(QRCTextBox + "数据超长!");
                    QRCTextBox.Clear();
                }
                else
                {
                    if (QRCTextBox.Text.Length == 1)
                    {
                        if (QRCTextBox.Text != "+")
                        {
                            QRCTextBox.Clear();
                            ListBox_AddMsgToListBox(QRCTextBox.Text + "首字母错误!");
                        }
                    }
                }
            }
        }
        private byte QrCode_IsCorrectCode(string QrcStr)  //判断是否为正确的二维码
        {
            byte ret = 0;
            byte loop = 0;
            char[] qrcode = QrcStr.ToCharArray();
            if (qrcode[0] != '+')
            {
                ret = 1;
                listBox1.Items.Add(QrcStr + "首字母错误!");
            }
            else
            {
                for (loop = 1; loop < 21; loop++)
                {
                    if ((qrcode[loop] >= 'A') && (qrcode[loop] <= 'Z')
                        || (qrcode[loop] >= 'a') && (qrcode[loop] <= 'z')
                        || (qrcode[loop] >= '0') && (qrcode[loop] <= '9'))
                    {
                    }
                    else
                    {
                        listBox1.Items.Add(QrcStr + "第" + loop.ToString() + "字符错误！");
                        ret = 1;
                        break;
                    }
                }
            }
            return ret;
        }
    }
}
