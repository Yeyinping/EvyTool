using GHJTool.Common;
using GHJTool.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public delegate void UpdateTreeViewDelegate();

    /// <summary>
    /// SingleFileAnalysis.xaml 的交互逻辑
    /// </summary>
    public partial class SingleFileAnalysis : UserControl
    {
        #region 变量
        GisVisionEx.GisVisionEx gisVisionEx = new GisVisionEx.GisVisionEx();
        UpdateTreeViewDelegate UpdateTreeViewEvent;
        #endregion

        public SingleFileAnalysis()
        {
            InitializeComponent();
            UseTheScrollViewerScrolling(this.tvDataPos);
            UseTheScrollViewerScrolling(this.tvPeak);
            //this.UiPeakInfo.DataContext = singleFileAnalysisViewModel;
            this.UiPeakInfos.DataContext = singleFileAnalysisViewModel;
            this.chbSaveResult.DataContext = singleFileAnalysisViewModel;

            this.cmbCurveType.Items.Add(GisVisionEx.CalcAlgoTypeEnum.TypeN.ToString());
            this.cmbCurveType.Items.Add(GisVisionEx.CalcAlgoTypeEnum.TypeU.ToString());
            this.cmbCurveType.SelectedIndex = 0;
            singleFileAnalysisViewModel.CurveType = this.cmbCurveType.SelectedItem.ToString().Equals(GisVisionEx.CalcAlgoTypeEnum.TypeN.ToString()) ? GisVisionEx.CalcAlgoTypeEnum.TypeN : GisVisionEx.CalcAlgoTypeEnum.TypeU;

            this.UpdateTreeViewEvent += new UpdateTreeViewDelegate(UpdateUITreeView);

            StartThread();
        }

        #region CalculatePeakThread
        List<Dictionary<string, string>> dicPeakPosList = new List<Dictionary<string, string>>();
        List<TestDataClass> csvTestDataList = new List<TestDataClass>();
        bool CalculatePeakEnable = false;

        Thread thCalculatePeak = null;
        bool IsThCalculatePeakEnable = false;

        public void StartThread()
        {
            try
            {

                if (thCalculatePeak == null || !thCalculatePeak.IsAlive)
                {
                    IsThCalculatePeakEnable = true;
                    thCalculatePeak = new Thread(ThFunCalculatePeak)
                    {
                        Name = $"CalculatePeak线程",
                        IsBackground = true
                    };
                    thCalculatePeak.Start();
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
                IsThCalculatePeakEnable = false;

                if (thCalculatePeak != null && thCalculatePeak.IsAlive)
                {
                    thCalculatePeak.Abort();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
            }
        }

        private void ThFunCalculatePeak()
        {
            string strErr = "";

            while (IsThCalculatePeakEnable)
            {
                Thread.Sleep(5);
                try
                {
                    if (CalculatePeakEnable)
                    {
                        CalculatePeakEnable = false;

                        
                        ShowLog("计算Peak开始。。。。。。");
                        Stopwatch sw = Stopwatch.StartNew();
                        bool bRtn = CalculatePeaks(singleFileAnalysisViewModel.FilePathList, singleFileAnalysisViewModel.Node, singleFileAnalysisViewModel.CurveType, out dicPeakPosList, out csvTestDataList, out strErr);
                        if (!bRtn)
                            ShowLog(strErr);
                        else
                        {
                            sw.Stop();
                            strErr = $"{sw.ElapsedMilliseconds / 1000d},";
                            UpdateTreeViewEvent?.BeginInvoke(null, null);
                        }
                        ShowLog($"计算Peak结束，耗时：{strErr}s。");
                    }
                }
                catch (Exception e)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}异常：{e.Message}，{e.StackTrace}";

                }
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btnCalculatePeak.IsEnabled = true;
                }));
            }
        }

        private void UpdateUITreeView()
        {
            UpdataPeakPos(dicPeakPosList);
            UpdateDataPOs(csvTestDataList);
        }
        #endregion

        #region TreeViewDataPos
        private ObservableCollection<TreeViewNodeItem> rawDataNodeItems = new ObservableCollection<TreeViewNodeItem>();
        TreeViewNodeItem rawdatarootnode = new TreeViewNodeItem() { Content = "DataPos" };

        void UpdateDataPOs(List<TestDataClass> csvTestData)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,new Action(() =>
            {

                this.tvDataPos.ItemsSource = null;
                rawDataNodeItems.Clear();
                rawdatarootnode.Children.Clear();

                for (int i = 0; i < csvTestData.Count; i++)
                {
                    TreeViewNodeItem chinode = new TreeViewNodeItem() { Content = $"DataPos_{i+1}" };

                    TreeViewNodeItem globalavgrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.GlobalAvg.ToString()}" };
                    TreeViewNodeItem globalsumrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.GlobalSum.ToString()}" };
                    
                    TreeViewNodeItem roiavgwrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.RoiAvgWithoutBlackLevel.ToString()}" };
                    TreeViewNodeItem roisumwrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.RoiSumWithoutBlackLevel.ToString()}" };

                    TreeViewNodeItem roiavgrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.RoiAvg.ToString()}" };
                    TreeViewNodeItem roisumrootnode = new TreeViewNodeItem() { Content = $"{GisVisionEx.CalcAlgorithm.RoiSum.ToString()}" };

                    for (int it = 0; it < csvTestData[i].nPoint; it++)
                    {
                        globalavgrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：({csvTestData[i].xArray[it].ToString("F3")},{csvTestData[i].yGlobalAvg[it].ToString("F3")})" });
                        globalsumrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：({csvTestData[i].xArray[it].ToString("F3")},{csvTestData[i].yGlobalSum[it].ToString("F3")})" });

                        roiavgwrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：({csvTestData[i].xArray[it].ToString("F3")},{csvTestData[i].yRoiAvgWithoutBlackLevel[it].ToString("F3")})" });
                        roisumwrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：({csvTestData[i].xArray[it].ToString("F3")},{csvTestData[i].yRoiSumWithoutBlackLevel[it].ToString("F3")})" });

                        roiavgrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：({csvTestData[i].xArray[it].ToString("F3")},{csvTestData[i].yRoiAvg[it].ToString("F3")})" });
                        roisumrootnode.Children.Add(new TreeViewNodeItem() { Content = $"Step{it + 1}：({csvTestData[i].xArray[it].ToString("F3")},{csvTestData[i].yRoiSum[it].ToString("F3")})" });
                    }
                    chinode.Children.Add(globalavgrootnode);
                    chinode.Children.Add(globalsumrootnode);
                    chinode.Children.Add(roiavgwrootnode);
                    chinode.Children.Add(roisumwrootnode);
                    chinode.Children.Add(roiavgrootnode);
                    chinode.Children.Add(roisumrootnode);
                    rawdatarootnode.Children.Add(chinode);
                }
                rawDataNodeItems.Add(rawdatarootnode);
                tvDataPos.ItemsSource = rawDataNodeItems;
            }));
        }
        #endregion

        #region TreeViewPeakPos

        SingleFileAnalysisViewModel singleFileAnalysisViewModel = new SingleFileAnalysisViewModel();

        private ObservableCollection<TreeViewNodeItem> PeakNodeItems = new ObservableCollection<TreeViewNodeItem>();
        TreeViewNodeItem peakrootnode = new TreeViewNodeItem() { Content = "PeakPos" };

        private void UpdataPeakPos(List<Dictionary<string, string>> dicpeakpos)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {

                this.tvPeak.ItemsSource = null;
                PeakNodeItems.Clear();
                peakrootnode.Children.Clear();

                foreach (var item in dicpeakpos)
                {
                    TreeViewNodeItem chinode = new TreeViewNodeItem() { Content = $"Peak_{dicpeakpos.IndexOf(item)+1}" };
                    foreach (var it in item)
                    {
                        chinode.Children.Add(new TreeViewNodeItem() { Content = $"{it.Key}：{it.Value}" });
                    }
                    peakrootnode.Children.Add(chinode);
                }
                PeakNodeItems.Add(peakrootnode);
                tvPeak.ItemsSource = PeakNodeItems;
            }));
        }

        #region AnalysisMethod

        public bool CalculatePeak(string filepath,string writefile,int writeIndex,int node, GisVisionEx.CalcAlgoTypeEnum curvetype,out Dictionary<string, string> dicpeakpos,out TestDataClass csvTestData,out string strErr)
        {
            strErr = "";
            dicpeakpos = new Dictionary<string, string>();
            csvTestData = new TestDataClass();

            try
            {

                #region 读取CSV数据

                string csvfile = filepath;
                string strline;
                string[] aryline;
                StreamReader mysr = new StreamReader(csvfile, System.Text.Encoding.UTF8);

                bool bfirstline = true;
                int index = 3;
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

                    csvTestData.xArray.Add(double.Parse(aryline[index - 1].Trim()));

                    csvTestData.yGlobalAvg.Add(double.Parse(aryline[index + 3 * 0].Trim()));
                    csvTestData.yGlobalSum.Add(double.Parse(aryline[index + 3 * 1].Trim()));

                    csvTestData.yRoiAvgWithoutBlackLevel.Add(double.Parse(aryline[index + 3 * 2].Trim()));
                    csvTestData.yRoiSumWithoutBlackLevel.Add(double.Parse(aryline[index + 3 * 3].Trim()));

                    csvTestData.yRoiAvg.Add(double.Parse(aryline[index + 3 * 4].Trim()));
                    csvTestData.yRoiSum.Add(double.Parse(aryline[index + 3 * 5].Trim()));

                    csvTestData.nPoint = csvTestData.xArray.Count;
                }

                #endregion

                #region 角度寻优FindPeak 
                //gisVisionEx.CalcAlgoType = curvetype;//启用UI选择时启用
                gisVisionEx.CalcAlgoType = GisVisionEx.CalcAlgoTypeEnum.TypeN;

                csvTestData.CopyTo(out TestDataClass tmpdata);

                #region GlobalAvg
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：GlobalAvg FindPeak 开始";

                int iRtn = gisVisionEx.AngleOptimizing(out strErr,
                                                   csvTestData.xArray.ToArray(),//roughAdjustTestData.xArray.ToArray(),
                                                   csvTestData.yGlobalAvg.ToArray(),//roughAdjustTestData.yRoiSumWithoutBlackLevel.ToArray(),
                                                   node,//拟合阶数
                                                   out double rmsError,
                                                   out double GlobalAvgBestAngle,
                                                   out double GlobalAvgBrightness,
                                                   out double[] GlobalAvgPolyXs,
                                                   out double[] GlobalAvgPolyYs);
                if (iRtn != 0)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：FindPeak 失败。{strErr}";
                    return false;
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：GlobalAvg FindPeak 完成。Peak坐标(最佳角度,亮度值)=({GlobalAvgBestAngle}°,{GlobalAvgBrightness})";

                dicpeakpos.Add("GlobalAvg", $"({GlobalAvgBestAngle.ToString("F3")},{GlobalAvgBrightness.ToString("F3")})");
                #endregion

                #region GlobalSum
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：GlobalSum FindPeak 开始";
                iRtn = gisVisionEx.AngleOptimizing(out strErr,
                                                   csvTestData.xArray.ToArray(),//roughAdjustTestData.xArray.ToArray(),
                                                   csvTestData.yGlobalSum.ToArray(),//roughAdjustTestData.yRoiSumWithoutBlackLevel.ToArray(),
                                                   node,//拟合阶数
                                                   out rmsError,
                                                   out double GlobalSumBestAngle,
                                                   out double GlobalSumBrightness,
                                                   out double[] GlobalSumPolyXs,
                                                   out double[] GlobalSumPolyYs);
                if (iRtn != 0)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：FindPeak 失败。{strErr}";
                    return false;
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：GlobalSum FindPeak 完成。Peak坐标(最佳角度,亮度值)=({GlobalSumBestAngle}°,{GlobalSumBrightness})";

                dicpeakpos.Add("GlobalSum", $"({GlobalSumBestAngle.ToString("F3")},{GlobalSumBrightness.ToString("F3")})");
                #endregion

                #region RoiAvgWithoutBlackLevel
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiAvgWithoutBlackLevel FindPeak 开始";
                iRtn = gisVisionEx.AngleOptimizing(out strErr,
                                                   csvTestData.xArray.ToArray(),//roughAdjustTestData.xArray.ToArray(),
                                                   csvTestData.yRoiAvgWithoutBlackLevel.ToArray(),//roughAdjustTestData.yRoiSumWithoutBlackLevel.ToArray(),
                                                   node,//拟合阶数
                                                   out rmsError,
                                                   out double RoiAvgWithoutBlackLevelBestAngle,
                                                   out double RoiAvgWithoutBlackLevelBrightness,
                                                   out double[] RoiAvgWithoutBlackLevelPolyXs,
                                                   out double[] RoiAvgWithoutBlackLevelPolyYs);
                if (iRtn != 0)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：FindPeak 失败。{strErr}";
                    return false;
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiAvgWithoutBlackLevel FindPeak 完成。Peak坐标(最佳角度,亮度值)=({RoiAvgWithoutBlackLevelBestAngle}°,{RoiAvgWithoutBlackLevelBrightness})";

                dicpeakpos.Add("RoiAvgWithoutBlackLevel", $"({RoiAvgWithoutBlackLevelBestAngle.ToString("F3")},{RoiAvgWithoutBlackLevelBrightness.ToString("F3")})");
                #endregion

                #region RoiSumWithoutBlackLevel
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiSumWithoutBlackLevel FindPeak 开始";
                iRtn = gisVisionEx.AngleOptimizing(out strErr,
                                                   csvTestData.xArray.ToArray(),//roughAdjustTestData.xArray.ToArray(),
                                                   csvTestData.yRoiAvgWithoutBlackLevel.ToArray(),//roughAdjustTestData.yRoiSumWithoutBlackLevel.ToArray(),
                                                   node,//拟合阶数
                                                   out rmsError,
                                                   out double RoiSumWithoutBlackLevelBestAngle,
                                                   out double RoiSumWithoutBlackLevelBrightness,
                                                   out double[] RoiSumWithoutBlackLevelPolyXs,
                                                   out double[] RoiSumWithoutBlackLevelPolyYs);
                if (iRtn != 0)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：FindPeak 失败。{strErr}";
                    return false;
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiSumWithoutBlackLevel FindPeak 完成。Peak坐标(最佳角度,亮度值)=({RoiAvgWithoutBlackLevelBestAngle}°,{RoiAvgWithoutBlackLevelBrightness})";

                dicpeakpos.Add("RoiSumWithoutBlackLevel", $"({RoiSumWithoutBlackLevelBestAngle.ToString("F3")},{RoiSumWithoutBlackLevelBrightness.ToString("F3")})");
                #endregion

                #region RoiAvg
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiAvg FindPeak 开始";
                iRtn = gisVisionEx.AngleOptimizing(out strErr,
                                                   csvTestData.xArray.ToArray(),//roughAdjustTestData.xArray.ToArray(),
                                                   csvTestData.yRoiAvg.ToArray(),//roughAdjustTestData.yRoiSumWithoutBlackLevel.ToArray(),
                                                   node,//拟合阶数
                                                   out rmsError,
                                                   out double RoiAvgBestAngle,
                                                   out double RoiAvgBrightness,
                                                   out double[] RoiAvgPolyXs,
                                                   out double[] RoiAvgPolyYs);
                if (iRtn != 0)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：FindPeak 失败。{strErr}";
                    return false;
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiAvg FindPeak 完成。Peak坐标(最佳角度,亮度值)=({RoiAvgBestAngle}°,{RoiAvgBrightness})";

                dicpeakpos.Add("RoiAvg", $"({RoiAvgBestAngle.ToString("F3")},{RoiAvgBrightness.ToString("F3")})");
                #endregion

                #region RoiSum
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiSum FindPeak 开始";
                iRtn = gisVisionEx.AngleOptimizing(out strErr,
                                                   csvTestData.xArray.ToArray(),//roughAdjustTestData.xArray.ToArray(),
                                                   csvTestData.yRoiSum.ToArray(),//roughAdjustTestData.yRoiSumWithoutBlackLevel.ToArray(),
                                                   node,//拟合阶数
                                                   out rmsError,
                                                   out double RoiSumBestAngle,
                                                   out double RoiSumBrightness,
                                                   out double[] RoiSumPolyXs,
                                                   out double[] RoiSumPolyYs);
                if (iRtn != 0)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}：FindPeak 失败。{strErr}";
                    return false;
                }
                strErr = $"{MethodBase.GetCurrentMethod().Name}：数据类型：RoiSum FindPeak 完成。Peak坐标(最佳角度,亮度值)=({RoiSumBestAngle}°,{RoiSumBrightness})";

                dicpeakpos.Add("RoiSum", $"({RoiSumBestAngle.ToString("F3")},{RoiSumBrightness.ToString("F3")})");
                #endregion

                #endregion

                if (singleFileAnalysisViewModel.IsExportResult2Csv)
                {
                    /*Task.Factory.StartNew(() =>*/ WriteCsvData(writefile, $"{writeIndex},{DateTime.Now.ToString("yyyyMMdd-HHmmssfff")},{node},{GlobalAvgBestAngle},{GlobalAvgBrightness },,{ GlobalSumBestAngle},{GlobalSumBrightness },,{ RoiAvgWithoutBlackLevelBestAngle},{RoiAvgWithoutBlackLevelBrightness },,{RoiSumWithoutBlackLevelBestAngle},{RoiSumWithoutBlackLevelBrightness},,{RoiAvgBestAngle},{RoiAvgBrightness},,{RoiSumBestAngle},{RoiSumBrightness},")/*)*/;
                }

                return true;
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}"; ShowLog(strErr);
                return false;
            }
        }

        //static int WriteCsvIndex = 0;
        static string writeFileName = null;
        public bool CalculatePeaks(ObservableCollection<string> filepaths,int node, GisVisionEx.CalcAlgoTypeEnum curvetype, out List<Dictionary<string, string>> disPeakPosList,out List<TestDataClass> csvTestDataList,out string strErr)
        {
            bool bresult = true;
            strErr = "";
            disPeakPosList = new List<Dictionary<string, string>>();
            csvTestDataList = new List<TestDataClass>();

            bool isFirstCreateCsv = true;
            if ((writeFileName == null || writeFileName == "") && singleFileAnalysisViewModel.IsExportResult2Csv)
            {
                writeFileName = CreateHeader(singleFileAnalysisViewModel.Node);
                isFirstCreateCsv = false;
            }
            if (isFirstCreateCsv && (!singleFileAnalysisViewModel.IsExportResult2SameCsv))
                writeFileName = CreateHeader(singleFileAnalysisViewModel.Node);

            foreach (var it in filepaths)
            {
                int index = filepaths.IndexOf(it);
                Dictionary<string, string> dicpeakpos = new Dictionary<string, string>();
                TestDataClass csvData = new TestDataClass();

                bresult = CalculatePeak(it, writeFileName, index, node, curvetype, out dicpeakpos, out csvData,out strErr);
                if(!bresult)
                {
                    break;
                }
                disPeakPosList.Add(dicpeakpos);
                csvTestDataList.Add(csvData);
            }

            return bresult;
        }
        #endregion
        #endregion

        #region WriteCsv
        private string CreateHeader(int node)
        {
            try
            {
                string rootpath = $@"D:\PeakData\";

                string foldername = $"{DateTime.Now.ToString("yyyyMMddHH")}";

                string folderpath = $@"{rootpath}\{foldername}\";

                if (false == Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }


                string csvfile = $@"{folderpath}\peakdata_{node}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.csv";
                if(singleFileAnalysisViewModel.IsExportResult2SameCsv)
                    csvfile = $@"{folderpath}\peakdata_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.csv";

                string header = "";
                header += $",,,GlobalAvg,,,GlobalSum,,,RoiAvgWithoutBlackLevel,,,RoiSumWithoutBlackLevel,,,RoiAvg,,,RoiSum,";
                File.AppendAllText(csvfile, $"{header}\r\n", Encoding.UTF8);

                header = "";
                header += $"Index,WriteTime,Order,Angle,Brightness,,Angle,Brightness,,Angle,Brightness,,Angle,Brightness,,Angle,Brightness,,Angle,Brightness,";

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
        private void BtnCalcPeak_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            string strErr = "";
            try
            {
                btn.IsEnabled = false;

                CalculatePeakEnable = true;

                //Dictionary<string, string> dicPeakPos = new Dictionary<string, string>();
                //List<Dictionary<string, string>> dicPeakPosList = new List<Dictionary<string, string>>();

                //TestDataClass csvTestData = new TestDataClass();
                //List<TestDataClass> csvTestDataList = new List<TestDataClass>();

                ////Task task = Task.Factory.StartNew(() => CalculatePeak(singleFileAnalysisViewModel.FilePath, singleFileAnalysisViewModel.Node,singleFileAnalysisViewModel.CurveType, out dicPeakPos,out csvTestData));
                ////task.Wait();

                //Stopwatch sw = Stopwatch.StartNew();

                ////使用Task多2秒耗时
                ////Task<bool> task = Task<bool>.Factory.StartNew(() => CalculatePeaks(singleFileAnalysisViewModel.FilePathList, singleFileAnalysisViewModel.Node, singleFileAnalysisViewModel.CurveType, out dicPeakPosList, out csvTestDataList,out strErr));
                ////task.Wait();
                ////if(!task.Result)
                ////    ShowLog(strErr);
                //bool bRtn = CalculatePeaks(singleFileAnalysisViewModel.FilePathList, singleFileAnalysisViewModel.Node, singleFileAnalysisViewModel.CurveType, out dicPeakPosList, out csvTestDataList, out strErr);
                //if (!bRtn)
                //    ShowLog(strErr);

                //sw.Stop();
                //ShowLog($"计算Peak任务耗时：{sw.ElapsedMilliseconds / 1000d}s");

                ////dicPeakPosList.Add(dicPeakPos);
                ///*Task.Factory.StartNew(() => */UpdataPeakPos(dicPeakPosList)/*)*/;


                ////csvTestDataList.Add(csvTestData);
                ///*Task.Factory.StartNew(() => */UpdateDataPOs(csvTestDataList)/*)*/;

            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
                ShowLog(strErr);
            }
            finally
            {
                
            }
        }

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

                #region 界面卡顿
                //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                //{
                //    ObservableCollection<string> filepathlist = new ObservableCollection<string>();
                //    ObservableCollection<string> filenamelist = new ObservableCollection<string>();
                //    if (filepath != null && filepath.Length > 0)
                //    {
                //        singleFileAnalysisViewModel.FilePath = filepath[0];
                //        singleFileAnalysisViewModel.FileName = System.IO.Path.GetFileName(filepath[0]);

                //        //FilePaths
                //        Task.Factory.StartNew(() =>
                //        {

                //            for (int i = 0; i < filepath.Length; i++)
                //            {
                //                filepathlist.Add(filepath[i]);
                //                filenamelist.Add(System.IO.Path.GetFileName(filepath[i]));
                //            }

                //        });
                //        singleFileAnalysisViewModel.FilePathList = filepathlist;
                //        singleFileAnalysisViewModel.FileNameList = filenamelist;
                //    }


                //}));
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

                ShowLog("导入文件完成\r\n");
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
                ShowLog(strErr);
            }
            finally
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btn.IsEnabled = true;
                }));
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
            this.Dispatcher.BeginInvoke(new Action(()=> {
                txtLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff ：")}{strlog}\r\n");
                txtLog.ScrollToEnd();
            }));
        }
        #endregion

        #region 滚动条内部非普通控件也可以使用鼠标滚动
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

        private void cmbCurveType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string curvetype = this.cmbCurveType.SelectedItem.ToString();
            singleFileAnalysisViewModel.CurveType = curvetype.Equals(GisVisionEx.CalcAlgoTypeEnum.TypeN.ToString()) ? GisVisionEx.CalcAlgoTypeEnum.TypeN : GisVisionEx.CalcAlgoTypeEnum.TypeU;
        }
    }

}
