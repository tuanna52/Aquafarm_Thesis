using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using System.Threading;
using System.IO;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using LiveCharts.Configurations;
using System.Globalization;

namespace Win_Thesis
{
    public partial class Form1 : Form
    {
        JObject JsonData;

        public ChartValues<MeasureModel> PH_ChartValue { get; set; }
        public ChartValues<MeasureModel> DO_ChartValue { get; set; }
        public ChartValues<MeasureModel> EC_ChartValue { get; set; }
        public ChartValues<MeasureModel> RTD_ChartValue { get; set; }

        public LineSeries PH_Serie { get; set; }
        public LineSeries DO_Serie { get; set; }
        public LineSeries EC_Serie { get; set; }
        public LineSeries RTD_Serie { get; set; }

        public System.Windows.Forms.Timer Timer { get; set; }

        string PH;
        string DO;
        string EC;
        string RTD;
        string Date;
        string Time;

        float PH_Value;
        float DO_Value;
        float EC_Value;
        float RTD_Value;


        public Form1()
        {
            InitializeComponent();
            dashboard.Visible = true;
            graph.Visible = false;

            Thread t = new Thread(new ThreadStart(SecondThread));
            t.IsBackground = true;
            t.Start();

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);


            draw_pH();
            draw_DO();
            draw_EC();
            draw_Temp();

            SetAxisLimits(DateTime.Now);

            //The next code simulates data changes every 1000 ms
            Timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            Timer.Tick += TimerOnTick;
            Timer.Start();
        }

        public void SecondThread()
        {
            string[] data_from_firebase = new string[2];
            // Variable to wait when data_from_firebase[1] has the first data
            int has_first_data = 0;

            while (true)
            {
                string URL = "https://aquafarm-c9b41.firebaseio.com/Hanoi/AquaFarm.json";
                HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(URL);
                request1.ContentType = "application/json: charset = utf-8";
                HttpWebResponse response1 = request1.GetResponse() as HttpWebResponse;
                using (Stream responsestream = response1.GetResponseStream())
                {

                    StreamReader Read = new StreamReader(responsestream, Encoding.UTF8);
                    data_from_firebase[0] = Read.ReadToEnd();

                    // Parse the data(string type) get from Firebase to json Object
                    JsonData = JObject.Parse(data_from_firebase[0]);

                    // Get the last child of data queried from Firebase
                    var LastChild = JsonData.Last.Last;

                    if (has_first_data != 0)
                    {
                        if (data_from_firebase[0].Length != data_from_firebase[1].Length)
                        {
                            // Get the value of each child in the newest data
                            PH = (string)LastChild["pH"];
                            DO = (string)LastChild["DO"];
                            EC = (string)LastChild["Conductivity"];
                            RTD = (string)LastChild["Temperature"];
                            Time = (string)LastChild["time"];
                            Date = (string)LastChild["date"];

                            // Set the value to each index in dashboard
                            label25.Invoke(new Action(() => label25.Text = PH));
                            label14.Invoke(new Action(() => label14.Text = DO));
                            label17.Invoke(new Action(() => label17.Text = EC));
                            label20.Invoke(new Action(() => label20.Text = RTD));
                            label4.Invoke(new Action(() => label4.Text = Time));
                            label6.Invoke(new Action(() => label6.Text = Date));

                            // Parse string value to float value
                            PH_Value = float.Parse(PH, CultureInfo.InvariantCulture.NumberFormat);
                            DO_Value = float.Parse(DO, CultureInfo.InvariantCulture.NumberFormat);
                            EC_Value = float.Parse(EC, CultureInfo.InvariantCulture.NumberFormat);
                            RTD_Value = float.Parse(RTD, CultureInfo.InvariantCulture.NumberFormat);
                        }
                    }
                }
                // Assign data_from_firebase[1] the old data of data_from_firebase[0]
                data_from_firebase[1] = data_from_firebase[0];
                // When data_from_firebase[1] has the first data
                has_first_data = 1;
                //Thread.Sleep(1000);
            }
        }

        private void draw_pH()
        {
            //the ChartValues property will store our values array
            PH_ChartValue = new ChartValues<MeasureModel>();

            PH_Serie = new LineSeries
            {
                Values = PH_ChartValue,
                PointGeometrySize = 8,
                Stroke = System.Windows.Media.Brushes.Yellow,
                Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#6Fffff00"),
                Title = "pH"
            };

            phChart.AxisX.Add(new Axis
            {
                IsMerged = true,
                ShowLabels = false,
                Separator = new Separator
                {
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection(new double[] { 2 }),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(64, 79, 86))
                }
            });

            phChart.AxisY.Add(new Axis
            {
                IsMerged = true,
                Separator = new Separator
                {
                    StrokeThickness = 1.5,
                    StrokeDashArray = new DoubleCollection(new double[] { 4 }),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(64, 79, 86))
                }
            });

            phChart.Series = new SeriesCollection
            {
                PH_Serie
            };
        }

        private void draw_DO()
        {
            //the ChartValues property will store our values array
            DO_ChartValue = new ChartValues<MeasureModel>();

            DO_Serie = new LineSeries
            {
                Values = DO_ChartValue,
                PointGeometrySize = 8,
                Stroke = System.Windows.Media.Brushes.Aqua,
                Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#6F00FFFF"),
                Title = "DO"
            };

            doChart.AxisX.Add(new Axis
            {
                IsMerged = true,
                ShowLabels = false,
                Separator = new Separator
                {
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection(new double[] { 2 }),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(64, 79, 86))
                }
            });

            doChart.AxisY.Add(new Axis
            {
                IsMerged = true,
                Separator = new Separator
                {
                    StrokeThickness = 1.5,
                    StrokeDashArray = new DoubleCollection(new double[] { 4 }),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(64, 79, 86))
                }
            });

            doChart.Series = new SeriesCollection
            {
                DO_Serie
            };
        }

        private void draw_EC()
        {
            //the ChartValues property will store our values array
            EC_ChartValue = new ChartValues<MeasureModel>();

            EC_Serie = new LineSeries
            {
                Values = EC_ChartValue,
                PointGeometrySize = 8,
                Stroke = System.Windows.Media.Brushes.MediumSpringGreen,
                Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#6F00fa9a"),
                Title = "EC"
            };

            ecChart.AxisX.Add(new Axis
            {
                IsMerged = true,
                ShowLabels = false,
                Separator = new Separator
                {
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection(new double[] { 2 }),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(64, 79, 86))
                }
            });

            ecChart.AxisY.Add(new Axis
            {
                IsMerged = true,
                Separator = new Separator
                {
                    StrokeThickness = 1.5,
                    StrokeDashArray = new DoubleCollection(new double[] { 4 }),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(64, 79, 86))
                }
            });

            ecChart.Series = new SeriesCollection
            {
                EC_Serie
            };
        }

        private void draw_Temp()
        {
            //the ChartValues property will store our values array
            RTD_ChartValue = new ChartValues<MeasureModel>();

            RTD_Serie = new LineSeries
            {
                Values = RTD_ChartValue,
                PointGeometrySize = 8,
                Stroke = System.Windows.Media.Brushes.LightCoral,
                Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#6Ff08080"),
                Title = "Temperature"
            };

            tempChart.AxisX.Add(new Axis
            {
                IsMerged = true,
                ShowLabels = false,
                Separator = new Separator
                {
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection(new double[] { 2 }),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(64, 79, 86))
                }
            });

            tempChart.AxisY.Add(new Axis
            {
                IsMerged = true,
                Separator = new Separator
                {
                    StrokeThickness = 1.5,
                    StrokeDashArray = new DoubleCollection(new double[] { 4 }),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(64, 79, 86))
                }
            });

            tempChart.Series = new SeriesCollection
            {
                RTD_Serie
            };
        }


        private void SetAxisLimits(DateTime now)
        {
            phChart.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            phChart.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(5).Ticks; //we only care about the last 5 seconds

            doChart.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            doChart.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(5).Ticks; //we only care about the last 5 seconds

            ecChart.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            ecChart.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(5).Ticks; //we only care about the last 5 seconds

            tempChart.AxisX[0].MaxValue = now.Ticks + TimeSpan.FromSeconds(1).Ticks; // lets force the axis to be 100ms ahead
            tempChart.AxisX[0].MinValue = now.Ticks - TimeSpan.FromSeconds(5).Ticks; //we only care about the last 5 seconds
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            var now = DateTime.Now;

            PH_ChartValue.Add(new MeasureModel
            {
                DateTime = now,
                Value = PH_Value
            });

            DO_ChartValue.Add(new MeasureModel
            {
                DateTime = now,
                Value = DO_Value
            });

            EC_ChartValue.Add(new MeasureModel
            {
                DateTime = now,
                Value = EC_Value
            });

            RTD_ChartValue.Add(new MeasureModel
            {
                DateTime = now,
                Value = RTD_Value
            });

            SetAxisLimits(now);

            //lets only use the last 10 values
            if (PH_ChartValue.Count > 10) PH_ChartValue.RemoveAt(0);
            if (DO_ChartValue.Count > 10) DO_ChartValue.RemoveAt(0);
            if (EC_ChartValue.Count > 10) EC_ChartValue.RemoveAt(0);
            if (RTD_ChartValue.Count > 10) RTD_ChartValue.RemoveAt(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dashboard.Visible = true;
            graph.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dashboard.Visible = false;
            graph.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
