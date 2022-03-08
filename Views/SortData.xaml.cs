using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GHJTool.Views
{
    /// <summary>
    /// SortData.xaml 的交互逻辑
    /// </summary>
    public partial class SortData : UserControl
    {
        public SortData()
        {
            InitializeComponent();

            thLoadPathAnalyze = new Thread(LoadPathAnalyze);
            thLoadPathAnalyze.Name = "LoadPathAnalyze";
            thLoadPathAnalyze.IsBackground = true;
            thLoadPathAnalyze.Start();
        }

        private Thread thLoadPathAnalyze = null;
        bool bstarting = false;
        void LoadPathAnalyze()
        {
            try
            {
                while (true)
                {
                    if (bstarting)
                    {
                        ReadCsv(FilePath);
                        bstarting = false;
                        this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { btnAnalyze.IsEnabled = true; }));
                    }
                    Thread.Sleep(5);
                }
            }
            catch (Exception exp)
            {
                string strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
            }
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            try
            {
                btn.IsEnabled = false;
                bstarting = true;
            }
            finally
            {
            }
        }

        string FilePath = "";

        private void LoadPathAnalyze_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            string strErr = "";

            try
            {
                btn.IsEnabled = false;


                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = false;
                dialog.AddExtension = true;
                dialog.RestoreDirectory = true;
                dialog.Filter = "CSV文件|*.csv";

                var dlgresult = dialog.ShowDialog();
                if (dlgresult == true)
                {
                    FilePath = dialog.FileName;
                }

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() => { txtPath.Text = FilePath; }));

            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
            }
            finally
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { btn.IsEnabled = true; }));
            }
        }

        void ReadCsv(string filepath)
        {
            #region 读取CSV数据

            string csvfile = filepath;
            string strline;
            string[] aryline;
            StreamReader mysr = new StreamReader(csvfile, System.Text.Encoding.UTF8);

            bool bfirstline = true;
            int index = 3;
            string datahead = "";
            int preGroupIndex = -1;
            string CurGroupIndexFile = "";
            while ((strline = mysr.ReadLine()) != null)
            {
                if (bfirstline && strline.Contains("CurveID"))
                {
                    bfirstline = false;
                    datahead = strline;
                    continue;
                }

                aryline = strline.Split(new char[] { ',' });
                if (aryline.Length < 2)
                    continue;
                int groupIndex = int.Parse(aryline[1].Trim());
                if (preGroupIndex == groupIndex)
                {
                    //组号相同写入同一个文件
                }
                else
                {
                    //创建新文件,并写入
                    CurGroupIndexFile = CreateFile(groupIndex, datahead);
                }
                WriteCsvData(CurGroupIndexFile,strline);

                preGroupIndex = groupIndex;
            }

            #endregion
        }

        #region WriteCsv
        private string CreateFile(int groupIndex,string headstr)
        {
            try
            {
                string rootpath = $@"D:\data\statistics\";

                string foldername = $"{DateTime.Now.ToString("yyyyMMdd")}";

                string folderpath = $@"{rootpath}\{foldername}\";

                if (false == Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }


                string csvfile = $@"{folderpath}\Index_{groupIndex}_{DateTime.Now.ToString("HHmmss")}.csv";

                File.AppendAllText(csvfile, $"{headstr}\r\n", Encoding.UTF8);

                return csvfile;
            }
            catch (Exception exp)
            {
                return null;
            }
        }
        private void WriteCsvData(string filename, string result)
        {
            try
            {
                if (!File.Exists(filename))
                    File.CreateText(filename);
                File.AppendAllText(filename, $"{result}\r\n", Encoding.UTF8);
            }
            catch (Exception exp)
            {

            }
        }
        #endregion

       
    }
}
