using LogBook_WPF.Models;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    /// 设置页面
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            tqslAddressBox.Text = configFile.Read("tqslAddr", "Upload");
            tqslStationNameBox.Text = configFile.Read("tqslStationName", "Upload");
            qrzCallsignBox.Text = configFile.Read("qrzCallsign", "Upload");
            //自动填入已有的信息
            
            string originPwd;
            try
            {
                StreamReader sr = new StreamReader("Data\\qrzPwd");
                Byte[] protectedPwd = Convert.FromBase64String(sr.ReadToEnd());
                originPwd = Encoding.UTF8.GetString(ProtectedData.Unprotect(protectedPwd, null, DataProtectionScope.CurrentUser));
                qrzPwdBox.Text = originPwd;
                //解密密码并填充
            }
            catch
            {
            }
        }

        IniFile configFile = new IniFile("Data\\config.ini");

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            configFile.Write("tqslAddr",tqslAddressBox.Text,"Upload");
            configFile.Write("tqslStationName",tqslStationNameBox.Text,"Upload");
            configFile.Write("qrzCallsign",qrzCallsignBox.Text,"Upload");
            //存储信息
            
            string originPwd = qrzPwdBox.Text;
            string protectedPwd = Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(originPwd),null,DataProtectionScope.CurrentUser));
            //将密码通过非对称加密方式加密

            StreamWriter sw = new StreamWriter("Data\\qrzPwd");
            sw.Write(protectedPwd);
            sw.Close(); 
            //将加密后的密码写入文件

            this.Close();
        }

        /// <summary>
        /// 选择TQSL执行文件地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowseTQ8_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".exe";
            ofd.Title = "请选择TQSL执行文件";
            if (ofd.ShowDialog() == true)
            {
                tqslAddressBox.Text = ofd.FileName;
            }
        }

        // 以下都是提示
        private void tqslAddressBox_GotFocus(object sender, RoutedEventArgs e)
        {
            tipsBlock.Text = "请填入TQSL执行文件位置";
        }

        private void tqslStationNameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            tipsBlock.Text = "请填入在TQSL程序中填写的台站位置";
        }

        private void qrzCallsignBox_GotFocus(object sender, RoutedEventArgs e)
        {
            tipsBlock.Text = "请填入QRZ.com的用户名(你的呼号)";
        }

        private void qrzPwdBox_GotFocus(object sender, RoutedEventArgs e)
        {
            tipsBlock.Text = "请填入QRZ.com的密码(密码会由Windows加密存储)";
        }
    }
}
