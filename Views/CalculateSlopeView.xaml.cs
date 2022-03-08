using GHJTool.Common;
using GHJTool.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
    /// CalculateSlopeView.xaml 的交互逻辑
    /// </summary>
    public partial class CalculateSlopeView : UserControl
    {

        public CalculateSlopeView()
        {
            InitializeComponent();

            UseTheScrollViewerScrolling(this.tvDataPos);
            UseTheScrollViewerScrolling(this.tvPeak);
            //this.UiPeakInfo.DataContext = singleFileAnalysisViewModel;
            this.UiSlopeInfos.DataContext = singleFileAnalysisViewModel;
        }

        #region TreeViewDataPos
        private List<TreeViewNodeItem> rawDataNodeItems = new List<TreeViewNodeItem>();
        TreeViewNodeItem rawdatarootnode = new TreeViewNodeItem() { Content = "PowersData" };

        void UpdateDataPOs(List<List<TestDataClass>> csvTestData)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {

                this.tvDataPos.ItemsSource = null;
                rawDataNodeItems.Clear();
                rawdatarootnode.Children.Clear();

                for (int k = 0; k < csvTestData.Count; k++)
                {
                    TreeViewNodeItem rawdatasecondrootnode = new TreeViewNodeItem() { Content = $"Group_{k + 1}" };
                    
                    for (int i = 0; i < csvTestData[k].Count; i++)
                    {
                        TreeViewNodeItem chinode = new TreeViewNodeItem() { Content = $"CurveID_{i + 1}" };

                        TreeViewNodeItem globalavgrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.GlobalAvg.ToString()}" };
                        TreeViewNodeItem globalsumrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.GlobalSum.ToString()}" };

                        TreeViewNodeItem roiavgwrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.RoiAvgWithoutBlackLevel.ToString()}" };
                        TreeViewNodeItem roisumwrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.RoiSumWithoutBlackLevel.ToString()}" };

                        TreeViewNodeItem roiavgrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.RoiAvg.ToString()}" };
                        TreeViewNodeItem roisumrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.RoiSum.ToString()}" };

                        for (int it = 0; it < csvTestData[k][i].nPoint; it++)
                        {
                            globalavgrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：{csvTestData[k][i].yGlobalAvg[it].ToString("F3")}" });
                            globalsumrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：{csvTestData[k][i].yGlobalSum[it].ToString("F3")}" });

                            roiavgwrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：{csvTestData[k][i].yRoiAvgWithoutBlackLevel[it].ToString("F3")}" });
                            roisumwrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：{csvTestData[k][i].yRoiSumWithoutBlackLevel[it].ToString("F3")}" });

                            roiavgrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：{csvTestData[k][i].yRoiAvg[it].ToString("F3")}" });
                            roisumrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：{csvTestData[k][i].yRoiSum[it].ToString("F3")}" });
                        }
                        chinode.Children.Add(globalavgrootnode);
                        chinode.Children.Add(globalsumrootnode);
                        chinode.Children.Add(roiavgwrootnode);
                        chinode.Children.Add(roisumwrootnode);
                        chinode.Children.Add(roiavgrootnode);
                        chinode.Children.Add(roisumrootnode);
                        rawdatasecondrootnode.Children.Add(chinode);
                    }
                    rawdatarootnode.Children.Add(rawdatasecondrootnode);
                }
                rawDataNodeItems.Add(rawdatarootnode);
                tvDataPos.ItemsSource = rawDataNodeItems;
            }));
        }
        #endregion

        #region TreeViewSlopes

        SingleFileAnalysisViewModel singleFileAnalysisViewModel = new SingleFileAnalysisViewModel() ;

        private List<TreeViewNodeItem> SlopeNodeItems = new List<TreeViewNodeItem>();
        TreeViewNodeItem sloperootnode = new TreeViewNodeItem() { Content = "Slopes" };
        
        private void UpdataSlopes(List<List<Dictionary<string, string>>> dicpeakpos)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {

                this.tvPeak.ItemsSource = null;
                SlopeNodeItems.Clear();
                sloperootnode.Children.Clear();

                foreach (var its in dicpeakpos)
                {
                    TreeViewNodeItem slopesecondrootnode = new TreeViewNodeItem() { Content = $"Group_{dicpeakpos.IndexOf(its) + 1}" };

                    foreach (var item in its)
                    {
                        TreeViewNodeItem chinode = new TreeViewNodeItem() { Content = $"Slope_{its.IndexOf(item) + 1}" };
                        foreach (var it in item)
                        {
                            chinode.Children.Add(new TreeViewNodeItem() { Content = $"{it.Key}：{it.Value}" });
                        }
                        slopesecondrootnode.Children.Add(chinode);
                    }
                    sloperootnode.Children.Add(slopesecondrootnode);
                }
                SlopeNodeItems.Add(sloperootnode);
                tvPeak.ItemsSource = SlopeNodeItems;
            }));
        }

        #region AnalysisMethod

        bool isFirstCreateCsv = true;
        static string writeFileName = null;

        public void CalculateSlope(string filepath, string writefile,out List<Dictionary<string, string>> dicslopes, out List<TestDataClass> csvTestData)
        {
            string strErr = "";
            dicslopes = new List<Dictionary<string, string>>();
            csvTestData = new List<TestDataClass>();
            TestDataClass testData = new TestDataClass();

            try
            {

                #region 读取CSV数据

                string csvfile = filepath;
                string strline;
                string[] aryline;
                StreamReader mysr = new StreamReader(csvfile, System.Text.Encoding.UTF8);

                bool bfirstline = true;
                int index = 5;
                int interval = 1;
                int onegrouplens = 0;
                while ((strline = mysr.ReadLine()) != null)
                {
                    if (bfirstline)
                    {
                        bfirstline = false;
                        continue;
                    }

                    aryline = strline.Split(new char[] { ',' });
                    if (aryline.Length < 4)
                        continue;

                    if (!aryline[4].Trim().Equals("Source"))
                    {
                        if (aryline[4].Trim().Equals("Average"))
                        {
                            TestDataClass tmp = null;
                            testData.CopyTo(out tmp);
                            csvTestData.Add(tmp);
                            testData.Clear();
                            onegrouplens = 0;
                        }
                        continue;
                    }

                    testData.xArray.Add(++onegrouplens);
                    testData.yGlobalAvg.Add(double.Parse(aryline[index + interval * 0].Trim()));
                    testData.yGlobalSum.Add(double.Parse(aryline[index + interval * 1].Trim()));

                    testData.yRoiAvgWithoutBlackLevel.Add(double.Parse(aryline[index + interval * 2].Trim()));
                    testData.yRoiSumWithoutBlackLevel.Add(double.Parse(aryline[index + interval * 3].Trim()));

                    testData.yRoiAvg.Add(double.Parse(aryline[index + interval * 4].Trim()));
                    testData.yRoiSum.Add(double.Parse(aryline[index + interval * 5].Trim()));

                    testData.nPoint = testData.xArray.Count;
                }

                #endregion


                #region Slope

                #region GlobalAvg
                Dictionary<string, string> slopesGlobalAvg = new Dictionary<string, string>();

                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：GlobalAvg Calculate Slope 开始";
                foreach (var it in csvTestData)
                {
                    int groupindex = csvTestData.IndexOf(it);

                    double trend = double.MaxValue;

                    var taskGlobAvg = Task.Factory.StartNew(() => GetTrendValue(out strErr, out trend, it.yGlobalAvg.ToArray()));
                    taskGlobAvg.Wait();
                    int iRtn = taskGlobAvg.Result;
                    if (iRtn != 0)
                    {
                        strErr = $"{MethodBase.GetCurrentMethod().Name}：GlobalSum Calculate Slope{groupindex} 失败。{strErr}";
                        strErr += "\r\n";
                    }

                    slopesGlobalAvg.Add($"{groupindex}", $"{trend.ToString("F3")}");
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：GlobalAvg Calculate Slope 完成";
                #endregion

                #region GlobalSum
                Dictionary<string, string> slopesGlobalSum = new Dictionary<string, string>();

                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：GlobalSum Calculate Slope 开始";
                foreach (var it in csvTestData)
                {
                    int groupindex = csvTestData.IndexOf(it);

                    double trend = double.MaxValue;

                    var taskGlobalSum = Task.Factory.StartNew(() => GetTrendValue(out strErr, out trend, it.yGlobalSum.ToArray()));
                    taskGlobalSum.Wait();
                    int iRtn = taskGlobalSum.Result;
                    if (iRtn != 0)
                    {
                        strErr = $"{MethodBase.GetCurrentMethod().Name}：GlobalSum Calculate Slope{groupindex} 失败。{strErr}";
                        strErr += "\r\n";
                    }

                    slopesGlobalSum.Add($"{groupindex}", $"{trend.ToString("F3")}");
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：GlobalSum Calculate Slope 完成";
                #endregion

                #region RoiAvgWithoutBlackLevel
                Dictionary<string, string> slopesRoiAvgWithoutBlackLevel = new Dictionary<string, string>();

                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiAvgWithoutBlackLevel Calculate Slope 开始";
                foreach (var it in csvTestData)
                {
                    int groupindex = csvTestData.IndexOf(it);

                    double trend = double.MaxValue;

                    var taskRoiAvgWithoutBlackLevel = Task.Factory.StartNew(() => GetTrendValue(out strErr, out trend, it.yRoiAvgWithoutBlackLevel.ToArray()));
                    taskRoiAvgWithoutBlackLevel.Wait();
                    int iRtn = taskRoiAvgWithoutBlackLevel.Result;
                    if (iRtn != 0)
                    {
                        strErr = $"{MethodBase.GetCurrentMethod().Name}：RoiAvgWithoutBlackLevel Calculate Slope{groupindex} 失败。{strErr}";
                        strErr += "\r\n";
                    }

                    slopesRoiAvgWithoutBlackLevel.Add($"{groupindex}", $"{trend.ToString("F3")}");
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiAvgWithoutBlackLevel Calculate Slope 完成";
                #endregion

                #region RoiWithoutBlackLevelSum
                Dictionary<string, string> slopesRoiSumWithoutBlackLevel = new Dictionary<string, string>();

                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiSumWithoutBlackLevel Calculate Slope 开始";
                foreach (var it in csvTestData)
                {
                    int groupindex = csvTestData.IndexOf(it);

                    double trend = double.MaxValue;

                    var taskRoiSumWithoutBlackLevel = Task.Factory.StartNew(() => GetTrendValue(out strErr, out trend, it.yRoiSumWithoutBlackLevel.ToArray()));
                    taskRoiSumWithoutBlackLevel.Wait();
                    int iRtn = taskRoiSumWithoutBlackLevel.Result;
                    if (iRtn != 0)
                    {
                        strErr = $"{MethodBase.GetCurrentMethod().Name}：RoiSumWithoutBlackLevel Calculate Slope{groupindex} 失败。{strErr}";
                        strErr += "\r\n";
                    }

                    slopesRoiSumWithoutBlackLevel.Add($"{groupindex}", $"{trend.ToString("F3")}");
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiSumWithoutBlackLevel Calculate Slope 完成";
                #endregion

                #region RoiAvg
                Dictionary<string, string> slopesRoiAvg = new Dictionary<string, string>();

                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiAvg Calculate Slope 开始";
                foreach (var it in csvTestData)
                {
                    int groupindex = csvTestData.IndexOf(it);

                    double trend = double.MaxValue;

                    var taskRoiAvg = Task.Factory.StartNew(() => GetTrendValue(out strErr, out trend, it.yRoiAvg.ToArray()));
                    taskRoiAvg.Wait();
                    int iRtn = taskRoiAvg.Result;
                    if (iRtn != 0)
                    {
                        strErr = $"{MethodBase.GetCurrentMethod().Name}：RoiAvg Calculate Slope{groupindex} 失败。{strErr}";
                        strErr += "\r\n";
                    }

                    slopesRoiAvg.Add($"{groupindex}", $"{trend.ToString("F3")}");
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiAvg Calculate Slope 完成";
                #endregion

                #region RoiSum
                Dictionary<string, string> slopesRoiSum = new Dictionary<string, string>();

                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiSum Calculate Slope 开始";
                foreach (var it in csvTestData)
                {
                    int groupindex = csvTestData.IndexOf(it);

                    double trend = double.MaxValue;

                    var taskRoiSum = Task.Factory.StartNew(() => GetTrendValue(out strErr, out trend, it.yRoiSum.ToArray()));
                    taskRoiSum.Wait();
                    int iRtn = taskRoiSum.Result;
                    if (iRtn != 0)
                    {
                        strErr = $"{MethodBase.GetCurrentMethod().Name}：RoiSum Calculate Slope{groupindex} 失败。{strErr}";
                        strErr += "\r\n";
                    }

                    slopesRoiSum.Add($"{groupindex}", $"{trend.ToString("F3")}");
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiSum Calculate Slope 完成";
                #endregion


                for (int i = 0; i < slopesGlobalAvg.Count; i++)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("GlobalAvg", slopesGlobalAvg[i.ToString()]);
                    dic.Add("GlobalSum", slopesGlobalSum[i.ToString()]);
                    dic.Add("RoiAvgWithoutBlackLevel", slopesRoiAvgWithoutBlackLevel[i.ToString()]);
                    dic.Add("RoiSumWithoutBlackLevel", slopesRoiSumWithoutBlackLevel[i.ToString()]);
                    dic.Add("RoiAvg", slopesRoiAvg[i.ToString()]);
                    dic.Add("RoiSum", slopesRoiSum[i.ToString()]);

                    dicslopes.Add(dic);

                    WriteCsvData(writefile, $"{i},{DateTime.Now.ToString("yyyyMMdd-HH:mm:ss:fff")},{slopesGlobalAvg[i.ToString()]},{slopesGlobalSum[i.ToString()]},{slopesRoiAvgWithoutBlackLevel[i.ToString()]},{slopesRoiSumWithoutBlackLevel[i.ToString()]},{slopesRoiAvg[i.ToString()]},{slopesRoiSum[i.ToString()]},");
                }
                
                #endregion

            }
            catch(Exception exp) { strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}"; ShowLog(strErr); }
        }
        public void CalculateSlopes(ObservableCollection<string> filepaths, out List<List<Dictionary<string, string>>> dicslopes, out List<List<TestDataClass>> csvTestDatas)
        {
            string strErr = "";
            dicslopes = new List<List<Dictionary<string, string>>>();
            csvTestDatas = new List<List<TestDataClass>>();

            try
            {
                foreach (var it in filepaths)
                {
                    int index = filepaths.IndexOf(it);

                    List<Dictionary<string, string>> dicslope = new List<Dictionary<string, string>>();
                    List<TestDataClass> csvData = new List<TestDataClass>();

                    isFirstCreateCsv = true;
                    if ((writeFileName == null || writeFileName == "") && singleFileAnalysisViewModel.IsExportResult2Csv)
                    {
                        writeFileName = CreateHeader(index++);
                        isFirstCreateCsv = false;
                    }
                    if (isFirstCreateCsv && (!singleFileAnalysisViewModel.IsExportResult2SameCsv))
                        writeFileName = CreateHeader(index++);

                    CalculateSlope(it, writeFileName, out dicslope, out csvData);
                    dicslopes.Add(dicslope);
                    csvTestDatas.Add(csvData);
                }
            }
            catch (Exception exp) { strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}"; ShowLog(strErr); }
        }
        #endregion

        #region Slope Algorithm
        /// <summary>
        /// 获取power数据的趋势
        /// </summary>
        /// <param name="strErr"></param>
        /// <param name="kvalue"></param>
        /// <param name="powers"></param>
        /// <returns></returns>
        public static int GetTrendValue(out string strErr, out double vtrend, double[] powers)
        {
            strErr = "";
            vtrend = 0;
            int len = powers.Length;
            if (len < 1)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数组长度为空";
                return -1;
            }
            vtrend = Slope(powers.ToList(), len);

            return 0;
        }

        #region 计算slope
        /// <summary> 
        /// 线性回归计算散点数据趋势线斜率 
        /// </summary> 
        /// <param name="input">待计算趋势线斜率的数据</param> 
        /// <param name="period">数据长度</param> 
        /// <returns>返回计算结果</returns> 
        public static double Slope(List<double> input_y, int period)
        {
            List<double> input_x = new List<double>();
            for (int i = 1; i <= period; i++)
            {
                input_x.Add(i);
            }

            var copyInputValues_x = input_x.ToList();
            var copyInputValues_y = input_y.ToList();
            List<double> arr_xy = new List<double>();
            List<double> arr_xx = new List<double>();
            List<double> arr_x = new List<double>();
            List<double> arr_y = new List<double>();
            arr_x = copyInputValues_x;
            for (int j = copyInputValues_y.Count - period; j < copyInputValues_y.Count; j++)
                arr_y.Add(copyInputValues_y[j]);

            double x_arr_dataAv = arr_x.Take(period).Average();
            double y_arr_dataAv = arr_y.Take(period).Average();
            for (int i = 0; i < arr_x.Count; i++)
            {
                arr_x[i] = arr_x[i] - x_arr_dataAv;
                arr_y[i] = arr_y[i] - y_arr_dataAv;
                arr_xx.Add(arr_x[i] * arr_x[i]);
                arr_xy.Add(arr_y[i] * arr_x[i]);
            }
            double sumxx = arr_xx.Sum();
            double sumxy = arr_xy.Sum();

            var result = sumxy / sumxx;
            return result;
        }
        #endregion

        #endregion

        #endregion

        #region WriteCsv
        private string CreateHeader(int index)
        {
            try
            {
                string rootpath = $@"D:\SlopeData\";

                string foldername = $"{DateTime.Now.ToString("yyyyMMddHH")}";

                string folderpath = $@"{rootpath}\{foldername}\";

                if (false == Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }


                string csvfile = $@"{folderpath}\slopedata_{index}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.csv";
                if (singleFileAnalysisViewModel.IsExportResult2SameCsv)
                    csvfile = $@"{folderpath}\slope_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.csv";

                string header = "";
                header += $"CurverID,WriteTime,GlobalAvgSlope,GlobalSumSlope,RoiAvgWithoutBlackLevelSlope,RoiSumWithoutBlackLevelSlope,RoiAvgSlope,RoiSumSlope,";

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

            }
        }
        #endregion

        #region  ClickEvent

        private void BtnCalcSlope_Click(object sender, RoutedEventArgs e)
        {
            string strErr = "";
            try
            {
                List < List <Dictionary<string, string>>> dicSlopeList = new List<List<Dictionary<string, string>>>();

                List<List<TestDataClass>> csvTestDataList = new List<List<TestDataClass>>();



                Task task = Task.Factory.StartNew(() => CalculateSlopes(singleFileAnalysisViewModel.FilePathList, out dicSlopeList, out csvTestDataList));
                task.Wait();

                //dicPeakPosList.Add(dicPeakPos);
                Task.Factory.StartNew(() => UpdataSlopes(dicSlopeList));

                //csvTestDataList.Add(csvTestData);
                Task.Factory.StartNew(() => UpdateDataPOs(csvTestDataList));

            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
                ShowLog(strErr);
            }
        }

        private void BtnImportDataFile_Click(object sender, RoutedEventArgs e)
        {
            string strErr = "";
            try
            {
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

                #region UI卡顿
                //if (filepath != null && filepath.Length > 0)
                //{
                //    singleFileAnalysisViewModel.FilePath = filepath[0];
                //    singleFileAnalysisViewModel.FileName = System.IO.Path.GetFileName(filepath[0]);
                //    //FilePaths
                //    Task.Factory.StartNew(() =>
                //    {

                //        ObservableCollection<string> filepathlist = new ObservableCollection<string>();
                //        ObservableCollection<string> filenamelist = new ObservableCollection<string>();

                //        for (int i = 0; i < filepath.Length; i++)
                //        {
                //            filepathlist.Add(filepath[i]);
                //            filenamelist.Add(System.IO.Path.GetFileName(filepath[i]));
                //        }

                //        singleFileAnalysisViewModel.FilePathList = filepathlist;
                //        singleFileAnalysisViewModel.FileNameList = filenamelist;
                //    });
                //}
                #endregion

                #region 逐个文件更新到列表。不卡死UI
                LoadFiles(filepath.ToList());
                filePaths.Clear();
                //fileNames.Clear();
                fileIndex = 0;
                this.cmbfilelist.ItemsSource = filePaths;
                this.cmbfilelist.Dispatcher.BeginInvoke(DispatcherPriority.Background, new AddItemDelegate(addItem));

                singleFileAnalysisViewModel.FilePathList = filePaths;
                #endregion

                ShowLog("导入文件完成");
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
                ShowLog(strErr);
            }
        }

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

        #region Log
        private void ShowLog(string strlog)
        {
            this.Dispatcher.BeginInvoke(new Action(() => {
                txtLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff ：")}{strlog}\r\n");
                txtLog.ScrollToEnd();
            }));
        }
        #endregion

        #region 滚动条内部非普通控件也可以使用鼠标滚动
        /// <summary>
        /// 滚动条内部非普通控件也可以使用鼠标滚动
        /// </summary>
        /// <param name="fElement">指定使用滚动条的空间</param>
        public void UseTheScrollViewerScrolling(FrameworkElement fElement)
        {
            fElement.PreviewMouseWheel += (sender, e) =>
            {
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                fElement.RaiseEvent(eventArg);
            };
        }
        #endregion

    }
}
