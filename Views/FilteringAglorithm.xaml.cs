using GHJTool.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// FilteringAglorithm.xaml 的交互逻辑
    /// </summary>
    public partial class FilteringAglorithm : UserControl
    {
        FilteringAglorithmViewModel filteringAglorithmViewModel = new FilteringAglorithmViewModel();
        public FilteringAglorithm()
        {
            InitializeComponent();
            this.DataContext = filteringAglorithmViewModel;


            StartThread();
        }

        #region FilteringAlgorithmProcessThread

        Thread thFilteringAlgorithmProcess = null;
        bool IsThFilteringAlgorithmProcessEnable = false;
        bool FilteringAlgorithmProcessEnable = false;

        public void StartThread()
        {
            try
            {

                if (thFilteringAlgorithmProcess == null || !thFilteringAlgorithmProcess.IsAlive)
                {
                    IsThFilteringAlgorithmProcessEnable = true;
                    thFilteringAlgorithmProcess = new Thread(ThFunFilteringAlgorithmProcess)
                    {
                        Name = $"FilteringAlgorithmProcess线程",
                        IsBackground = true
                    };
                    thFilteringAlgorithmProcess.Start();
                }

            }
            catch (Exception e)
            {

            }
        }
        public void StopThread()
        {
            try
            {
                IsThFilteringAlgorithmProcessEnable = false;

                if (thFilteringAlgorithmProcess != null && thFilteringAlgorithmProcess.IsAlive)
                {
                    thFilteringAlgorithmProcess.Abort();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
            }
        }

        private void ThFunFilteringAlgorithmProcess()
        {
            string strErr = "";

            while (IsThFilteringAlgorithmProcessEnable)
            {
                Thread.Sleep(5);
                try
                {
                    if (FilteringAlgorithmProcessEnable)
                    {
                        FilteringAlgorithmProcessEnable = false;

                        FilteringProcess(this.filteringAglorithmViewModel.FilePathList);
                    }
                }
                catch (Exception e)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}异常：{e.Message}，{e.StackTrace}";

                }
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btnFilteringAlgorithmProcess.IsEnabled = true;
                }));
            }
        }

        private void FilteringProcess(ObservableCollection<string> filepaths)
        {
            int count = 0;
            foreach (var it in filepaths)
            {
                int index = filepaths.IndexOf(it);

                int iRtn = ThroughoutProcess(it, out bool bresult,out string resultstr, out string strErr);
                if (iRtn != 0)
                    this.filteringAglorithmViewModel.FilteringAlgorithmResultStr += $"{strErr}\r\n";
                if (bresult)
                {
                    count++;
                    
                    if (count > this.filteringAglorithmViewModel.NgTimes)
                    {
                        this.filteringAglorithmViewModel.FilteringAlgorithmResultStr += $"连续NG{this.filteringAglorithmViewModel.NgTimes}次，激光器不稳定！请关闭或更换激光器！\r\n";
                        break;
                    }

                    this.filteringAglorithmViewModel.FilteringAlgorithmResultStr += $"{resultstr}\r\n";
                }
            }
        }
        #endregion

        #region 滤除算法
        /// <summary>
        /// PowerTrend滤除算法
        /// powertrend策略1：所有powertrend值做和运算
        ///     sum>30000（可设定），数据被跑掉。
        ///     （此策略会误抛数据，如3、30。同时无法被滤除的情况：
        ///     当趋势线以0为轴对称，trend值分布在0轴的正负区间，globalsum值为0；）
        /// powertrend策略2：以中心点为位置对称点，计算对称位置
        ///     （如B2与B12）的点的差值的绝对值之和。若和值大于20000（可设定），则数据被抛掉。
        ///     （不能滤除的情况：如数据1、19、23、29、30、31）
        /// </summary>
        /// <param name="trendList">PowerTrend数据</param>
        /// <param name="subSum">输出策略2：趋势线中心对称位置差值的和值</param>
        /// <param name="globalSum">输出策略1：趋势线总和值</param>
        /// <param name="strErr"></param>
        /// <returns></returns>
        private int TrendFilteringAlgorithm(List<double> trendList,out double subSum,out double globalSum,out string strErr)
        {
            strErr = "";
            subSum = 0;
            globalSum = 0;
            int iRtn = -1;
            try
            {
                int listLen = trendList.Count;

                if (null == trendList|| listLen<1)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：输入数组为空。";
                    return -1;
                }

                globalSum = trendList.Sum();

                List<double> peak = new List<double>();
                if (0 == listLen % 2)//偶数集合
                {
                    for (int i = listLen / 2; i < listLen; i++)
                        peak.Add(Math.Abs(trendList[listLen - 1 - i] - trendList[i]));
                }
                else//奇数集合
                {
                    for (int i = listLen / 2 + 1; i < listLen; i++)
                        peak.Add(Math.Abs(trendList[listLen - 1 - i] - trendList[i]));
                }
                subSum = peak.Sum();

                iRtn = 0;
            }
            catch(Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}异常：{exp.Message}，{exp.StackTrace}";
            }
            return iRtn;
        }

        /// <summary>
        /// Percentage滤除算法
        /// percentage策略3.1：如数据19、23，只有单个差值绝对值瞬间超过1的，该数据被跑掉
        /// percentage策略3.2：以中间点对称，中心点左右位置对称点两两之差。
        ///     若差值绝对值大于0.5（可设定）个数大于等于2（可设定），则该数据被抛掉
        /// </summary>
        /// <param name="percentageList">Percentage数据</param>
        /// <param name="peakList">输出百分比中心对称位置的差值列表</param>
        /// <param name="strErr"></param>
        /// <returns></returns>
        private int PercentageFilteringAlgorithm(List<double> percentageList, out List<double> peakList, out string strErr)
        {
            strErr = "";
            peakList = new List<double>();
            int iRtn = -1;
            try
            {
                int listLen = percentageList.Count;

                if (null == percentageList || listLen < 1)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：输入数组为空。";
                    return -1;
                }

               
                if (0 == listLen % 2)//偶数集合
                {
                    for (int i = listLen / 2; i < listLen; i++)
                        peakList.Add(Math.Abs(percentageList[listLen - 1 - i] - percentageList[i]));
                }
                else//奇数集合
                {
                    for (int i = listLen / 2 + 1; i < listLen; i++)
                        peakList.Add(Math.Abs(percentageList[listLen - 1 - i] - percentageList[i]));
                }

                iRtn = 0;
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}异常：{exp.Message}，{exp.StackTrace}";
            }
            return iRtn;
        }
        #endregion

        #region 抛料处理
        /// <summary>
        /// 抛料处理
        /// </summary>
        /// <param name="trendList"></param>
        /// <param name="percentageList"></param>
        /// <param name="bMatchAnyOneFilteringAglo">true:满足滤除算法抛料条件，抛掉当前测试 false:不满足滤除算法抛料条件</param>
        /// <param name="strErr"></param>
        /// <returns></returns>
        private int ThroughoutProcess(string filename,out bool bMatchAnyOneFilteringAglo,out string resultStr,out string strErr)
        {
            strErr = "";
            resultStr = "";
            bMatchAnyOneFilteringAglo = false;

            List<double> trendList = new List<double>();
            List<double> percentageList = new List<double>();

            int iRtn = -1;
            try
            {
                #region 读取CSV数据

                string csvfile = filename;
                string strline;
                string[] aryline;
                StreamReader mysr = new StreamReader(csvfile, System.Text.Encoding.UTF8);

                bool bfirstline = true;
                while ((strline = mysr.ReadLine()) != null)
                {
                    if (bfirstline)
                    {
                        bfirstline = false;
                        continue;
                    }

                    if (strline.Contains("序号"))
                        continue;

                    aryline = strline.Split(new char[] { ',' });
                    if (aryline.Length < 3)
                        continue;

                    percentageList.Add(double.Parse(aryline[9].Trim()));

                    trendList.Add(double.Parse(aryline[10].Trim()));
                   
                }

                #endregion

                //获取配置参数
                double subsumtrend = this.filteringAglorithmViewModel.SubSumTrend;
                double globalsumtrend = this.filteringAglorithmViewModel.GlobalSumTrend;
                double dValuePercentage = this.filteringAglorithmViewModel.DValuePercentage;
                int dValuePercentageNum = this.filteringAglorithmViewModel.DValuePercentageNum;
                double energystepPercentage = this.filteringAglorithmViewModel.EnergyStepPercentage;

                //比较结果
                iRtn = TrendFilteringAlgorithm(trendList, out double subSum, out double globalSum, out strErr);
                if(iRtn != 0)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：{strErr}";
                    return -1;
                }
                iRtn = PercentageFilteringAlgorithm(percentageList, out List<double> peakList, out strErr);
                if (iRtn != 0)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：{strErr}";
                    return -1;
                }
                bool bLargerThenSubSumTrend, bLargerThenGlobalSumTrend, bLargerThenDValuePercentageNum, bLargerThenEnergyStepPercentage;
                bLargerThenSubSumTrend = DoubleLarger(subSum, subsumtrend);
                bLargerThenGlobalSumTrend = DoubleLarger(globalSum, globalsumtrend);

                int LargerThenDValuePercentageNum = 0;//百分比差值超过设定规格数量
                int LargerThenEnergyStepPercentageNum = 0;//超过百分比差值能量阶跃规格个数

                string peakListStr = "";
                foreach (var it in peakList)
                {
                    if (DoubleLarger(it, dValuePercentage))
                        LargerThenDValuePercentageNum++;
                    if (DoubleLarger(it, dValuePercentageNum))
                        LargerThenEnergyStepPercentageNum++;

                    peakListStr += $"{it.ToString("F4")}，";
                }
                peakListStr = peakListStr.Substring(0, peakListStr.Length - 2);

                bLargerThenDValuePercentageNum = LargerThenDValuePercentageNum > dValuePercentageNum ? true : false;
                bLargerThenEnergyStepPercentage = (LargerThenEnergyStepPercentageNum == 1 && LargerThenDValuePercentageNum == 1) ? true : false;

                bMatchAnyOneFilteringAglo = bLargerThenSubSumTrend | bLargerThenGlobalSumTrend | bLargerThenDValuePercentageNum | bLargerThenEnergyStepPercentage;

                if (bMatchAnyOneFilteringAglo)
                {
                    FilteringThroughoutTimes++;

                    ////csv header
                    //$"CurNgTime,写入时间,滤除算法,SubSumTrend,GlobalSum,PercentageList,,SubSumTrend趋势线差值之和,GlobalSumTrend趋势线总和,EnergyStepPercentage百分比差值能量阶跃，DValuePercentageNum百分比差值数量,"

                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("SubSumTrend", $"{subSum.ToString("F4")}");
                    result.Add("GlobalSumTrend", $"{globalSum.ToString("F4")}");
                    result.Add("PercentagePeakList", peakListStr);

                    string filteringStr = bLargerThenSubSumTrend ? "SubSumTrend" : (bLargerThenGlobalSumTrend? "GlobalSumTrend" : (bLargerThenDValuePercentageNum? "DValuePercentageNum" : "EnergyStepPercentage"));

                    resultStr = $"{FilteringThroughoutTimes},{DateTime.Now.ToString("yyyyMMdd-HH:mm:ss:fff")},{filteringStr},{subSum},{globalSum},{peakListStr},,{subsumtrend},{globalsumtrend},{ energystepPercentage},{dValuePercentage},{dValuePercentageNum},";

                    //记录Csv
                    if (FilteringThroughoutCsvFile == "" || FilteringThroughoutCsvFile == null)
                        FilteringThroughoutCsvFile = CreateFilteringThroughoutCsvHeader();

                    if (!File.Exists(FilteringThroughoutCsvFile))
                        FilteringThroughoutCsvFile = CreateFilteringThroughoutCsvHeader();

                    WriteCsvData(FilteringThroughoutCsvFile, resultStr);
                }

                return 0;

            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}异常：{exp.Message}，{exp.StackTrace}";
            }
            return iRtn;
        }


        #region WriteCsv

        static string FilteringThroughoutCsvFile;
        static int FilteringThroughoutTimes = 0;

        private string CreateFilteringThroughoutCsvHeader()
        {
            try
            {
                string rootpath = $@"D:\SAAData\TestData\Throughout\";

                string foldername = $"AA滤除数据-{DateTime.Now.ToString("yyyyMMdd-HH")}";

                string folderpath = $@"{rootpath}{foldername}";

                if (false == Directory.Exists(rootpath))
                {
                    Directory.CreateDirectory(rootpath);
                }

                string csvfile = $@"{folderpath}.csv";

                string header = "";

                header += $",,,结果,,,,配置参数,";
                File.AppendAllText(csvfile, $"{header}\r\n", Encoding.UTF8);

                header = "";
                header += $"CurNgTime,写入时间,滤除算法,SubSumTrend,GlobalSumTrend,PercentagePeakList,,SubSumTrend趋势线斜率对称点差值之和规格,GlobalSumTrend趋势线斜率总和规格,EnergyStepPercentage百分比对称点差值能量阶跃规格,DValuePercentage百分比对称点差值规格,DValuePercentageNum超百分比对称点差值规格数量,";
                File.AppendAllText(csvfile, $"{header}\r\n", Encoding.UTF8);

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
                string strErr = $"{MethodBase.GetCurrentMethod().Name}异常：{exp.Message}，{exp.StackTrace}";
            }
        }
        #endregion

        #region double比较大小
        /// <summary>
        /// 判断value1是否等于value2
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool DoubleEquals(double value1, double value2)
        {
            //双精度误差
            var DOUBLE_DELTA = 1E-6;
            return value1 == value2 || Math.Abs(value1 - value2) < DOUBLE_DELTA;
        }

        /// <summary>
        /// double比较value1是否大于value2
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool DoubleLarger(double value1, double value2)
        {
            //双精度误差
            var DOUBLE_DELTA = 1E-6;
            return value1 > (value2 + DOUBLE_DELTA);
        }

        /// <summary>
        /// double比较value1是否小于value2
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool DoubleSmaller(double value1, double value2)
        {
            //双精度误差
            var DOUBLE_DELTA = 1E-6;
            return value1 < (value2 - DOUBLE_DELTA);
        }
        #endregion

        #endregion

        #region 更新文件列表不卡顿写法
        int fileIndex;
        List<String> _filePaths;
        ObservableCollection<String> filePaths = new ObservableCollection<string>();
        ObservableCollection<String> fileNames = new ObservableCollection<string>();
        private void LoadFiles(List<string> files)
        {
            _filePaths = files;
            filePaths.Clear();
            //fileNames.Clear();
            fileIndex = 0;
            this.cmbfilelist.ItemsSource = filePaths;
            this.cmbfilelist.Dispatcher.BeginInvoke(DispatcherPriority.Background, new AddItemDelegate(addItem));
        }

        private delegate void AddItemDelegate();
        private void addItem()
        {
            if (fileIndex < _filePaths.Count)
            {
                filePaths.Add(_filePaths[fileIndex++]);
                //fileNames.Add(System.IO.Path.GetFileName(_filePaths[fileIndex++]));
                this.cmbfilelist.Dispatcher.BeginInvoke(DispatcherPriority.Background, new AddItemDelegate(addItem));
            }
        }

        #endregion

        private void BtnImportDataFile_Click(object sender, RoutedEventArgs e)
        {

            var btn = sender as Button;
            string strErr = "";
            try
            {
                btn.IsEnabled = false;

                string[] filepath = null;
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.AddExtension = true;
                dialog.RestoreDirectory = true;
                dialog.Filter = "CSV文件|*.csv";

                var dlgresult = dialog.ShowDialog();
                if (dlgresult == true)
                {
                    filepath = dialog.FileNames;
                }


                #region 逐个文件更新到列表。不卡死UI
                LoadFiles(filepath.ToList());
                filePaths.Clear();
                //fileNames.Clear();
                fileIndex = 0;
                this.cmbfilelist.ItemsSource = filePaths;
                this.cmbfilelist.Dispatcher.BeginInvoke(DispatcherPriority.Background, new AddItemDelegate(addItem));

                filteringAglorithmViewModel.FilePathList = filePaths;
                #endregion

                //ShowLog("导入文件完成\r\n");
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
                //ShowLog(strErr);
            }
            finally
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btn.IsEnabled = true;
                }));
            }
        }

        private void BtnFilteringAgloProcess_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            string strErr = "";
            try
            {
                btn.IsEnabled = false;

                FilteringAlgorithmProcessEnable = true;
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
            }
            finally
            {

            }
        }
    }
}
