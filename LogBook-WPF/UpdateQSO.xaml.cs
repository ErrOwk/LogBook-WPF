using LogBook_WPF.Models;
using SQLite;
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

namespace LogBook_WPF
{
    /// <summary>
    /// 更新QSO
    /// </summary>
    public partial class UpdateQSO : Window
    {

        public SQLiteConnection conn;

        public int Id;

        //我也不知道为什么但是他就是这样写删除才不会崩
        public QSO qso1;

        public List<int> hours { get; set; } = new List<int>();
        public List<int> minutes { get; set; } = new List<int>();
        public List<int> seconds { get; set; } = new List<int>();

        public UpdateQSO(QSO qso)
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

            conn = new SQLiteConnection("Data\\QSO.db3");
            conn.CreateTable<QSO>();
            //配置SQLite

            qso1 = qso;

            DateBox.Text = qso.UTCTime.Date.ToString();
            hourBox.Text = qso.UTCTime.Hour.ToString();
            minuteBox.Text = qso.UTCTime.Minute.ToString();
            secondBox.Text = qso.UTCTime.Second.ToString();

            selfCallsign.Text = qso.selfCallsign;
            toCallsign.Text = qso.toCallsign;

            modeBox.Text = qso.mode;
            propModeBox.Text = qso.prop_mode;
            satNameBox.Text = qso.satName;

            freq.Text = qso.freq.ToString();
            freq_rx.Text = qso.freq_rx.ToString();

            bandBox.Text = qso.band;
            bandRxBox.Text = qso.band_rx;

            selfRST.Text = qso.selfRST.ToString();
            toRST.Text = qso.toRST.ToString();

            selfGrid.Text = qso.selfGrid;
            toGrid.Text = qso.toGrid;

            selfWX.Text = qso.selfWX;
            toWX.Text = qso.toWX;

            selfRIG.Text = qso.selfRIG;
            toRIG.Text = qso.toRIG;

            selfANT.Text = qso.selfANT;
            toANT.Text = qso.toANT;

            selfWatt.Text = qso.selfWatt.ToString();
            toWatt.Text = qso.toWatt.ToString();

            selfQTH.Text = qso.toQTH;
            toQTH.Text = qso.toQTH;

            RMKS.Text = qso.RMKS;

            isRcvdQSL.IsChecked = qso.isRcvdQSL;
            isSentQSL.IsChecked = qso.isSentQSL;

            Id = qso.Id;
            //将原信息替换到界面中
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
            else if (string.IsNullOrEmpty(freq_rx.Text) && string.IsNullOrEmpty(bandRxBox.Text)) { MessageBox.Show("请输入接收波段"); return; }


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
                Id = Id,
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

            conn.Update(newQSO);
            //将数据转换并检验后存入数据库中

            this.Close();
        }

        private void qsoDel_Click(object sender, RoutedEventArgs e)
        {
            //我也不知道为什么但是他就是这样才不会崩
            conn.Delete(qso1);
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
        /// 获取卫星的上行频率
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
            if (SatelliteFrequencies.TryGetValue(sat, out txFreq))
            {
                return txFreq;
            }
            else return "";
        }

        /// <summary>
        /// 获取卫星的下行频率
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
            if (SatelliteFrequencies.TryGetValue(sat, out rxFreq))
            {
                return rxFreq;
            }
            else return "";
        }

        /// <summary>
        /// 填写频率后自动转换波段
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
        /// 填写频率后自动转换波段
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
        /// 填写卫星后自动获取信息
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
