using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using ZedGraph;


namespace DO_AN_FULL
{
    public partial class Form1 : Form
    {

        double t = 0, t1 = 0;
        double yk,kp,ki,kd,refe,velo,vk,ak;
        string SDatas = String.Empty;
        string DataVel = string.Empty;
        int mode,active;
        int stop_st,chieu;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            string[] ports = SerialPort.GetPortNames();
            comboBox1.DataSource = ports;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "Đồ thị vận tốc";
            myPane.XAxis.Title.Text = "Thời gian (s)";
            myPane.YAxis.Title.Text = "vận tốc";
            RollingPointPairList list = new RollingPointPairList(60000);
            LineItem curve = myPane.AddCurve("Vận tốc", list, Color.Red, SymbolType.None);
            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 15;
            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.MajorStep = 5;
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 100;
            myPane.AxisChange();

            GraphPane myPane1 = zedGraph.GraphPane;
            myPane1.Title.Text = "Đồ thị vị trí";
            myPane1.XAxis.Title.Text = "Thời gian (s)";
            myPane1.YAxis.Title.Text = "vị trí";
            RollingPointPairList list1 = new RollingPointPairList(60000);
            LineItem curve1 = myPane1.AddCurve("Vị trí", list1, Color.Red, SymbolType.None);
            myPane1.XAxis.Scale.Min = 0;
            myPane1.XAxis.Scale.Max = 15;
            myPane1.XAxis.Scale.MinorStep = 1;
            myPane1.XAxis.Scale.MajorStep = 2;
            myPane1.YAxis.Scale.Min = 0;
            myPane1.YAxis.Scale.Max = 10;
            myPane1.AxisChange();
        }
        void ClearZedGraph()
        {
            zedGraphControl1.GraphPane.CurveList.Clear(); // Xóa đường
            zedGraphControl1.GraphPane.GraphObjList.Clear(); // Xóa đối tượng

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();

            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "Đồ thị vận tốc";
            myPane.XAxis.Title.Text = "Thời gian (s)";
            myPane.YAxis.Title.Text = "Vận tốc";

            RollingPointPairList list = new RollingPointPairList(60000);
            LineItem curve = myPane.AddCurve("Vận tốc", list, Color.Red, SymbolType.None);

            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 15;
            myPane.XAxis.Scale.MinorStep = 1;
            myPane.XAxis.Scale.MajorStep = 5;
            myPane.YAxis.Scale.Min = 0;
            myPane.YAxis.Scale.Max = 100;

            zedGraphControl1.AxisChange();
        }
        void ClearZedGraph1()
        {
            zedGraph.GraphPane.CurveList.Clear(); // Xóa đường
            zedGraph.GraphPane.GraphObjList.Clear(); // Xóa đối tượng

            zedGraph.AxisChange();
            zedGraph.Invalidate();

            GraphPane myPane1 = zedGraph.GraphPane;
            myPane1.Title.Text = "Đồ thị vị trí";
            myPane1.XAxis.Title.Text = "Thời gian (s)";
            myPane1.YAxis.Title.Text = "vị trí";

            RollingPointPairList list1 = new RollingPointPairList(60000);
            LineItem curve1 = myPane1.AddCurve("Vị trí", list1, Color.Red, SymbolType.None);

            myPane1.XAxis.Scale.Min = 0;
            myPane1.XAxis.Scale.Max = 15;
            myPane1.XAxis.Scale.MinorStep = 1;
            myPane1.XAxis.Scale.MajorStep = 5;
            myPane1.YAxis.Scale.Min = 0;
            myPane1.YAxis.Scale.Max = 10;

            zedGraph.AxisChange();
        }

        public void draw_1()
        {

            if (zedGraphControl1.GraphPane.CurveList.Count <= 0)
                return;

            LineItem curve = zedGraphControl1.GraphPane.CurveList[0] as LineItem;

            if (curve == null)
                return;

            IPointListEdit list = curve.Points as IPointListEdit;

            if (list == null)
                return;

            list.Add(t1, vk); // Thêm điểm trên đồ thị

            Scale xScale = zedGraphControl1.GraphPane.XAxis.Scale;
            Scale yScale = zedGraphControl1.GraphPane.YAxis.Scale;

            // Tự động Scale theo trục x
            if (t > xScale.Max - xScale.MajorStep)
            {
                xScale.Max = t1 + xScale.MajorStep;
                xScale.Min = xScale.Max - 15;
            }

            // Tự động Scale theo trục y
            if (vk > yScale.Max - yScale.MajorStep)
            {
                yScale.Max = vk + yScale.MajorStep;
            }
            else if (vk < yScale.Min + yScale.MajorStep)
            {
                yScale.Min = vk - yScale.MajorStep;
            }
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }

        private void draw()
        {
            if (zedGraph.GraphPane.CurveList.Count <= 0)
                return;

            LineItem curve1 = zedGraph.GraphPane.CurveList[0] as LineItem;

            if (curve1 == null)
                return;

            IPointListEdit list1 = curve1.Points as IPointListEdit;

            if (list1 == null)
                return;

            list1.Add(t, yk); // Thêm điểm trên đồ thị

            Scale xScale = zedGraph.GraphPane.XAxis.Scale;
            Scale yScale = zedGraph.GraphPane.YAxis.Scale;

            // Tự động Scale theo trục x
            if (t > xScale.Max - xScale.MajorStep)
            {
                xScale.Max = t + xScale.MajorStep;
                xScale.Min = xScale.Max - 15;
            }

            // Tự động Scale theo trục y
            if (yk > yScale.Max - yScale.MajorStep)
            {
                yScale.Max = yk + yScale.MajorStep;
            }
            else if (yk < yScale.Min + yScale.MajorStep)
            {
                yScale.Min = yk - yScale.MajorStep;
            }

            zedGraph.AxisChange();
            zedGraph.Invalidate();
            zedGraph.Refresh();

        }

      



        private void button1_Click_1(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string[] arrList = serialPort1.ReadLine().Split('|');
                
                    SDatas = arrList[1];
                    double.TryParse(SDatas, out ak);
                    yk = ak / 45;
                    output.Text = yk.ToString();                             
                    DataVel = arrList[2];
                    double.TryParse(DataVel, out vk);
                    output_Vel.Text = vk.ToString();
                    rotation.Text = arrList[0];
            }
            catch
            {
                return;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(active == 0)
            draw();
            if (active == 1)
            draw_1();
            t = t + 0.01;
            t1 = t1 + 0.2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.BaudRate = 9600;
            serialPort1.PortName = comboBox1.Text;
            serialPort1.Open();
            label6.Text = "Connected";
            }
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (serialPort1.IsOpen)
            {
                if (checkBox1.CheckState == CheckState.Checked)
                    mode = 1;
                else
                    mode = 0;
                
                if (checkBox2.CheckState == CheckState.Checked)
                    active = 1;
                else
                    active = 0;
               

                if (active == 1)
                    timer1.Interval = 200;
                if (active == 0)
                    timer1.Interval = 10;
                timer1.Start();
                if (checkBox1.CheckState == CheckState.Unchecked)
                {
                    if (active == 0)
                        refe = (Convert.ToDouble(Ref.Text))*45;
                    else
                        refe = Convert.ToDouble(Vel.Text);
                    kp = Convert.ToDouble(Kp.Text);
                    ki = Convert.ToDouble(Ki.Text);
                    kd = Convert.ToDouble(Kd.Text);
                }
                else
                {
                    refe = 0;
                    kp = 0;
                    ki = 0;
                    kd = 0;
                }
                    serialPort1.Write(mode.ToString());
                    serialPort1.Write("\n");
                    serialPort1.Write(active.ToString());
                    serialPort1.Write("\n");
                    serialPort1.Write(refe.ToString());
                    serialPort1.Write("\n");
                    serialPort1.Write(kp.ToString());
                    serialPort1.Write("\n");
                    serialPort1.Write(ki.ToString());
                    serialPort1.Write("\n");
                    serialPort1.Write(kd.ToString());
                    serialPort1.Write("\n");
                }    
            }
        private void Stop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            yk = 0;
            vk = 0;
            t = 0;
            ClearZedGraph();
            ClearZedGraph1();
            if (serialPort1.IsOpen)
            {
              stop_st = 1;
              serialPort1.Write(stop_st.ToString());
              serialPort1.Write("\n");  
            }    
        }

    }
}
