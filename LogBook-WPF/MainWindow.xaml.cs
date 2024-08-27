using iNKORE.UI.WPF.Helpers;
using LogBook_WPF.Models;
using Microsoft.Win32;
using SQLite;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Windows.System;

namespace LogBook_WPF
{

    /// <summary>
    /// 主页面
    /// </summary>
    public partial class MainWindow : Window
    {
        internal SQLiteConnection conn;

        public IniFile configFile;

        public MainWindow()
        {
            Directory.CreateDirectory("Data");
            InitializeComponent();
            configFile = new IniFile("Data\\config.ini");
            conn = new SQLiteConnection("Data\\QSO.db3");
            conn.CreateTable<QSO>();
            UpdateDgv();
        }

        private void newQSO_Click(object sender, RoutedEventArgs e)
        {
            Window newQSO = new NewQSO();
            newQSO.ShowDialog();
            UpdateDgv();
        }

        /// <summary>
        /// 从数据库中更新列表显示
        /// </summary>
        private void UpdateDgv()
        {
            dgvQSOs.ItemsSource = null;
            List<QSO> qsoList = new List<QSO>();
            qsoList = conn.Table<QSO>().ToList<QSO>();
            //存入列表

            todayQSOCount.Text = "  Today QSO Counts: " + qsoList.Count.ToString();
            //计算当日QSO

            qsoList.Reverse();
            dgvQSOs.ItemsSource = qsoList;
            //更新列表

        }

        

        private void dgvQSOs_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dgvQSOs.SelectedIndex != -1)
            {
                List<QSO> qsoList = new List<QSO>();
                qsoList = conn.Table<QSO>().ToList<QSO>();
                //获取QSO列表

                Window updateQSO = new UpdateQSO(qsoList[qsoList.Count - dgvQSOs.SelectedIndex - 1]);
                updateQSO.ShowDialog();
                //修改选中项

                UpdateDgv();
            }
        }

        private void exportQSO_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "保存.ADI文件";
            dlg.FileName = "exportQSO.adi";
            dlg.DefaultExt = ".adi";
            dlg.Filter = "ADIF Files|*.adi";
            //定义路径选择框
            if (dlg.ShowDialog() == true)
            {
                exportADIF(dlg.FileName);
            }
        }

        /// <summary>
        /// 通过集成exe上传数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToQRZ_Click(object sender, RoutedEventArgs e)
        {
            string originPwd = "";
            
            try
            {
                //解密密码
                StreamReader sr = new StreamReader("Data\\qrzPwd");
                Byte[] protectedPwd = Convert.FromBase64String(sr.ReadToEnd());
                originPwd = Encoding.UTF8.GetString(ProtectedData.Unprotect(protectedPwd, null, DataProtectionScope.CurrentUser));
            }
            catch (Exception ex)
            {
                MessageBox.Show("密码解密失败,错误信息:" + ex.Message);
                return;
            }
            exportADIF("exportqrz.adi");
            try
            {
                //工作方式参考https://gitee.com/yuzhenwu/x-qsl-amateur-radio-adif-tool,感谢并致以诚挚的73!
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "qrz_auto_import_from_lotw.exe";

                if (string.IsNullOrEmpty(configFile.Read("qrzCallsign", "Upload")))
                {
                    MessageBox.Show("无法获取到呼号,是否在设置中配置了呼号?");
                    return;
                };

                if (string.IsNullOrEmpty(originPwd))
                {
                    MessageBox.Show("无法获取到密码,是否保存了密码?");
                    return;
                };

                startInfo.Arguments = $"{configFile.Read("qrzCallsign", "QRZ")} {originPwd} --adif exportqrz.adi";

                Process.Start(startInfo);

                MessageBox.Show("导出成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error opening file: " + ex.Message);
            }
        }

        /// <summary>
        /// 导出QSO到adi文件
        /// </summary>
        /// <param name="path">文件路径</param>
        private void exportADIF(string path)
        {
            List<QSO> qsoList = new List<QSO>();
            qsoList = conn.Table<QSO>().ToList<QSO>();
            //获取QSO列表

            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine("<ADIF_VERS:5>3.1.4");
            sw.WriteLine("<CREATED_TIMESTAMP:15>" + DateTime.UtcNow.ToString("yyyyMMdd HHmmss"));
            sw.WriteLine("<PROGRAMID:10>WPFLOGBOOK");
            sw.WriteLine("<PROGRAMVERSION:5>1.1.0");
            sw.WriteLine("<EOH>");
            sw.WriteLine("");
            //写入头部内容

            foreach (QSO qso in qsoList)
            {

                //// 添加QSO记录
                sw.WriteLine("<CALL:" + qso.toCallsign.Length + ">" + qso.toCallsign);
                sw.WriteLine("   <MODE:" + qso.mode.Length + ">" + qso.mode);
                sw.WriteLine("   <QSO_DATE:8>" + qso.UTCTime.ToString("yyyyMMdd"));
                sw.WriteLine("   <TIME_ON:6>" + qso.UTCTime.ToString("HHmmss"));
                sw.WriteLine("   <FREQ:" + qso.freq.ToString().Length + ">" + qso.freq.ToString());
                sw.WriteLine("   <BAND:" + qso.band.Length + ">" + qso.band);

                if (!string.IsNullOrEmpty(qso.freq_rx.ToString()))
                {
                    sw.WriteLine("   <BAND_RX:" + qso.band_rx.Length + ">" + qso.band_rx);
                    sw.WriteLine("   <FREQ_RX:" + qso.freq_rx.ToString().Length + ">" + qso.freq_rx.ToString());
                }

                if (!string.IsNullOrEmpty(qso.selfGrid))
                {
                    sw.WriteLine("   <MY_GRIDSQUARE:" + qso.selfGrid.Length + ">" + qso.selfGrid);
                }

                if (!string.IsNullOrEmpty(qso.toGrid))
                {
                    sw.WriteLine("   <GRIDSQUARE:" + qso.toGrid.Length + ">" + qso.toGrid);
                }

                if (!string.IsNullOrEmpty(qso.selfWatt.ToString()))
                {
                    sw.WriteLine("   <TX_PWR:" + qso.selfWatt.ToString().Length + ">" + qso.selfWatt.ToString());
                }

                if (!string.IsNullOrEmpty(qso.toWatt.ToString()))
                {
                    sw.WriteLine("   <RX_PWR:" + qso.toWatt.ToString().Length + ">" + qso.toWatt.ToString());
                }

                if (!string.IsNullOrEmpty(qso.prop_mode))
                {
                    sw.WriteLine("   <PROP_MODE:" + qso.prop_mode.Length + ">" + qso.prop_mode);
                }

                if (!string.IsNullOrEmpty(qso.satName))
                {
                    sw.WriteLine("   <SAT_NAME:" + qso.satName.Length + ">" + qso.satName);
                }
                if (!String.IsNullOrEmpty(qso.RMKS))
                {
                    sw.WriteLine("   <COMMENT:" + qso.RMKS.Length + ">" + qso.RMKS);
                }
                sw.WriteLine("<EOR>");
            }

            sw.Close();
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            Window setting = new Settings();
            setting.ShowDialog();
        }

        private void exportToLoTW_Click(object sender, RoutedEventArgs e)
        {
            exportADIF("exportlotw.adi");
            if (string.IsNullOrEmpty(configFile.Read("tqslAddr", "Upload")))
            {
                MessageBox.Show("无法获取TQSL文件地址,是否在设置中配置了TQSL可执行文件地址?");
                return;
            };

            if (string.IsNullOrEmpty(configFile.Read("tqslStationName", "Upload")))
            {
                MessageBox.Show("无法获取到台站名称,是否在设置中配置了台站名称?(台站名称应当填入TQSL应用中的台站地址)");
                return;
            };

            //工作方式参考https://gitee.com/yuzhenwu/x-qsl-amateur-radio-adif-tool,感谢并致以诚挚的73!
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(configFile.Read("tqslAddr", "Upload"));
            startInfo.Arguments = "/C \"tqsl -q -l \"" + configFile.Read("tqslStationName", "Upload") + "\" -p \"Insecure\" -a all -u -d \"exportlotw.adi\"  2>temp.txt  ";
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.StandardOutputEncoding = Encoding.Default;

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();

            MessageBox.Show("导出任务执行完成");


        }
    }
}