using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ErrOwk.IniParser;
using LogBook_WPF.Models;
using Microsoft.Win32.SafeHandles;
using SQLite;
using Windows.Media.Audio;

namespace LogBook_WPF
{
    /// <summary>
    /// 新建QSO页面
    /// </summary>
    public partial class NewQSO : Window
    {
        public List<int> hours { get; set; }= new List<int>();
        public List<int> minutes { get; set; } = new List<int>();
        public List<int> seconds { get; set; } = new List<int>();

        public DateTime UTCTime = DateTime.UtcNow;

        public SQLiteConnection conn;

        public IniParser iniFile = new IniParser("Data\\config.ini");

        public NewQSO()
        {
            InitializeComponent();
            for (int i = 0; i < 24; i++)
            {
                hours.Add(i);
            }
            for (int i = 0; i < 60; i++)
            {
                minutes.Add(i);
                seconds.Add(i);
            }
            hourBox.ItemsSource = hours;
            minuteBox.ItemsSource = minutes;
            secondBox.ItemsSource = seconds;
            //配置时间选择列表

            hourBox.SelectedIndex = UTCTime.Hour;
            minuteBox.SelectedIndex = UTCTime.Minute;
            secondBox.SelectedIndex = UTCTime.Second;
            DateBox.Text = UTCTime.Date.ToString("d");
            //配置UTC时间

            conn = new SQLiteConnection("Data\\QSO.db3");
            conn.CreateTable<QSO>();
            //配置SQLite

            selfCallsign.Text = iniFile.Get("LogBook - WPF","selfCallsign");
            selfGrid.Text = iniFile.Get("LogBook - WPF", "selfGrid");
            selfWX.Text = iniFile.Get("LogBook - WPF", "selfWX");
            selfRIG.Text = iniFile.Get("LogBook - WPF", "selfRIG");
            selfANT.Text = iniFile.Get("LogBook - WPF", "selfANT");
            selfWatt.Text = iniFile.Get("LogBook - WPF", "selfWatt");
            selfQTH.Text = iniFile.Get("LogBook - WPF", "selfQTH");
            //从ini文件中读取保存的信息
        }


        private void qsoSave_Click(object sender, RoutedEventArgs e)
        {
            DateTime UTCTime = Convert.ToDateTime(DateBox.Text + " " + hourBox.Text + ":" + minuteBox.Text + ":" + secondBox.Text);
            if (selfCallsign.Text == "") { MessageBox.Show("请输入呼号"); return; }
            else if (toCallsign.Text == "") { MessageBox.Show("请输入呼号"); return; }
            else if (selfRST.Text == "") { MessageBox.Show("请输入信号报告"); return; }
            else if (toRST.Text == "") { MessageBox.Show("请输入信号报告"); return; }
            else if (modeBox.Text == "") { MessageBox.Show("请输入通信模式"); return; }
            else if (freq.Text == "") { MessageBox.Show("请输入发射频率"); return; }
            else if (bandBox.Text == "") { MessageBox.Show("请输入发射波段"); return; }
            else if (!string.IsNullOrEmpty(freq_rx.Text) && string.IsNullOrEmpty(bandRxBox.Text)) { MessageBox.Show("请输入接收波段"); return; }


            try { if (freq.Text != "") Convert.ToDouble(freq.Text); }
            catch 
            { 
                MessageBox.Show("请正确输入频率");
                return;
            }

            try { if (freq_rx.Text != "") Convert.ToDouble(freq_rx.Text); }
            catch
            {
                MessageBox.Show("请正确输入频率");
                return;
            }

            try { Convert.ToInt32(selfRST.Text); }
            catch 
            {
                MessageBox.Show("请正确输入RST");
                return;
            }

            try { Convert.ToInt32(toRST.Text); }
            catch 
            { 
                MessageBox.Show("请正确输入RST"); 
                return; 
            }

            try { if (selfWatt.Text != "") Convert.ToDouble(selfWatt.Text); }
            catch 
            { 
                MessageBox.Show("请正确输入功率"); 
                return; 
            }

            try { if (toWatt.Text != "") Convert.ToDouble(toWatt.Text); }
            catch 
            { 
                MessageBox.Show("请正确输入功率");
                return; 
            }

            

            QSO newQSO = new QSO()
            {
                UTCTime = UTCTime,
                selfCallsign = selfCallsign.Text,
                toCallsign = toCallsign.Text,
                mode = modeBox.Text,
                freq = Convert.ToDouble(freq.Text),
                band = bandBox.Text,
                selfRST = Convert.ToInt32(selfRST.Text),
                toRST = Convert.ToInt32(toRST.Text),
                selfANT = selfANT.Text,
                toANT = toANT.Text,
                selfRIG = selfRIG.Text,
                toRIG = toRIG.Text,
                selfGrid = selfGrid.Text,
                toGrid = toGrid.Text,
                selfQTH = selfQTH.Text,
                toQTH = toQTH.Text,
                selfWX = selfWX.Text,
                toWX = toWX.Text,
                RMKS = RMKS.Text,
                isRcvdQSL = isRcvdQSL.IsChecked,
                isSentQSL = isSentQSL.IsChecked,
                prop_mode = propModeBox.Text,
                satName = satNameBox.Text,
            };
            if (selfWatt.Text != "") newQSO.selfWatt = Convert.ToDouble(selfWatt.Text);
            if (toWatt.Text != "") newQSO.toWatt = Convert.ToDouble(toWatt.Text);
            if (freq_rx.Text != "") 
            { 
                newQSO.freq_rx = Convert.ToDouble(freq_rx.Text);
                newQSO.band_rx = bandRxBox.Text;
            }

            conn.Insert(newQSO);
            //将数据转换并检验后存入数据库中

            iniFile.Update("LogBook - WPF", "selfCallsign", selfCallsign.Text);
            iniFile.Update("LogBook - WPF", "selfGrid", selfGrid.Text);
            iniFile.Update("LogBook - WPF", "selfWX", selfWX.Text);
            iniFile.Update("LogBook - WPF", "selfRIG", selfRIG.Text);
            iniFile.Update("LogBook - WPF", "selfANT", selfANT.Text);
            iniFile.Update("LogBook - WPF", "selfWatt", selfWatt.Text);
            iniFile.Update("LogBook - WPF", "selfQTH", selfQTH.Text);
            //向ini文件保存信息

            this.Close();
            
        }

        /// <summary>
        /// 获取波段
        /// </summary>
        /// <param name="freq">频率单位MHz</param>
        /// <returns></returns>
        private string getBand(double freq)
        {
            if (freq >= 420.000 && freq <= 450.000)
            {
                return "70CM";
            }
            else if (freq >= 220.000 && freq <= 225.000)
            {
                return "1.25M";
            }
            else if (freq >= 144.000 && freq <= 148.000)
            {
                return "2M";
            }
            else if (freq >= 70.000 && freq <= 71.000)
            {
                return "4M";
            }
            else if (freq >= 50.000 && freq <= 54.000)
            {
                return "6M";
            }
            else if (freq >= 28.000 && freq <= 29.700)
            {
                return "10M";
            }
            else if (freq >= 24.890 && freq <= 24.990)
            {
                return "12M";
            }
            else if (freq >= 21.000 && freq <= 21.450)
            {
                return "15M";
            }
            else if (freq >= 18.068 && freq <= 18.168)
            {
                return "17M";
            }
            else if (freq >= 14.000 && freq <= 14.350)
            {
                return "20M";
            }
            else if (freq >= 10.100 && freq <= 10.150)
            {
                return "30M";
            }
            else if (freq >= 7.000 && freq <= 7.300)
            {
                return "40M";
            }
            else if (freq >= 5.250 && freq <= 5.450)
            {
                return "60M";
            }
            else if (freq >= 3.500 && freq <= 4.000)
            {
                return "80M";
            }
            else if (freq >= 1.800 && freq <= 2.000)
            {
                return "160M";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取卫星上行频率
        /// </summary>
        /// <param name="sat">卫星名称</param>
        /// <returns></returns>
        private string getTxFreq(string sat)
        {
            Dictionary<string, string> SatelliteFrequencies = new Dictionary<string, string>
            {
                {"SO-50", "145.85"},
                {"SO-121", "145.875"},
                {"AO-27", "145.8"},
                {"AO-91", "435.25"},
                {"UVSQ", "145.905"},
                {"PO-101", "437.5"},
                {"CAS-3H", "144.35"},
                {"ARISS", "145.99"},
                {"INSPR7", "145.97"},
                {"AO-92", "435.35"},
                {"TEVEL1", "145.97"},
                {"TEVEL2", "145.97"},
                {"TEVEL3", "145.97"},
                {"TEVEL4", "145.97"},
                {"TEVEL5", "145.97"},
                {"TEVEL6", "145.97"},
                {"TEVEL7", "145.97"},
                {"TEVEL8", "145.97"},
                {"IO-117", "435.307"},
                {"RS-44", "145.965"},
            };

            string txFreq;
            if (SatelliteFrequencies.TryGetValue(sat, out txFreq!))
            {
                return txFreq;
            }
            else return "";
        }

        /// <summary>
        /// 获取卫星下行频率
        /// </summary>
        /// <param name="sat">卫星名称</param>
        /// <returns></returns>
        private string getRxFreq(string sat)
        {
            Dictionary<string, string> SatelliteFrequencies = new Dictionary<string, string>
            {
                {"SO-50", "36.795"},
                {"SO-121", "436.666"},
                {"AO-27", "436.795"},
                {"AO-91", "145.96"},
                {"UVSQ", "437.02"},
                {"PO-101", "145.9"},
                {"CAS-3H", "437.2"},
                {"ARISS", "437.8"},
                {"INSPR7", "437.41"},
                {"AO-92", "145.88"},
                {"TEVEL1", "436.4"},
                {"TEVEL2", "436.4"},
                {"TEVEL3", "436.4"},
                {"TEVEL4", "436.4"},
                {"TEVEL5", "436.4"},
                {"TEVEL6", "436.4"},
                {"TEVEL7", "436.4"},
                {"TEVEL8", "436.4"},
                {"IO-117", "435.31"},
                {"RS-44", "435.6"},
            };

            string rxFreq;
            if (SatelliteFrequencies.TryGetValue(sat, out rxFreq!))
            {
                return rxFreq;
            }
            else return "";
        }

        /// <summary>
        /// 输入频率自动转换波段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void freq_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(freq.Text))
            {
                try
                {
                    bandBox.Text = getBand(Convert.ToDouble(freq.Text));
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 输入频率自动转换波段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void freq_rx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(freq_rx.Text))
            {
                try
                {
                    bandRxBox.Text = getBand(Convert.ToDouble(freq_rx.Text));
                }
                catch
                {
                    return;
                }

            }
        }

        /// <summary>
        /// 输入卫星自动填充
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void satNameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            freq.Text = getTxFreq(satNameBox.Text);
            freq_rx.Text = getRxFreq(satNameBox.Text);
            try
            {
                bandBox.Text = getBand(Convert.ToDouble(freq.Text));
            }
            catch
            {
                return;
            }
            try
            {
                bandRxBox.Text = getBand(Convert.ToDouble(freq_rx.Text));
            }
            catch
            {
                return;
            }
        }

    }
}
