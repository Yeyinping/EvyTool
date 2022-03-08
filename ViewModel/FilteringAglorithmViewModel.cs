using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GHJTool.ViewModel
{
    public class FilteringAglorithmViewModel : INotifyPropertyChanged
    {
        #region Interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region 属性定义
        //1、添加moduleproduct4种滤除策略的可配置的参数：
        //    SubSumTrend趋势线相对位置差值之和，
        //    GlobalSumTrend趋势线总和，
        //    EnergyStepPercentage百分比相对位置差值能量阶跃
        //    DValuePercentageNum百分比相对位置差值数量
        private double _SubSumTrend = 20000;
        /// <summary>
        /// 趋势线相对位置差值之和规格
        /// </summary>
        public double SubSumTrend
        {
            get { return _SubSumTrend; }
            set { _SubSumTrend = value; RaisePropertyChanged("SubSumTrend"); }
        }

        private double _GlobalSumTrend = 30000;
        /// <summary>
        /// 趋势线总和规格
        /// </summary>
        public double GlobalSumTrend
        {
            get { return _GlobalSumTrend; }
            set { _GlobalSumTrend = value; RaisePropertyChanged("GlobalSumTrend"); }
        }

        private double _EnergyStepPercentage = 1.0;
        /// <summary>
        /// 百分比相对位置差值能量阶跃规格
        /// </summary>
        public double EnergyStepPercentage
        {
            get { return _EnergyStepPercentage; }
            set { _EnergyStepPercentage = value; RaisePropertyChanged("EnergyStepPercentage"); }
        }

        private double _DValuePercentage = 0.5;
        /// <summary>
        /// 百分比相对位置差值规格
        /// </summary>
        public double DValuePercentage
        {
            get { return _DValuePercentage; }
            set { _DValuePercentage = value; RaisePropertyChanged("DValuePercentage"); }
        }

        private int _DValuePercentageNum = 2;
        /// <summary>
        /// 百分比相对位置差值超过规格数量
        /// </summary>
        public int DValuePercentageNum
        {
            get { return _DValuePercentageNum; }
            set { _DValuePercentageNum = value; RaisePropertyChanged("DValuePercentageNum"); }
        }

        private int _NgTimes = 4;
        /// <summary>
        /// 连续NG次数
        /// </summary>
        public int NgTimes
        {
            get { return _NgTimes; }
            set { _NgTimes = value; RaisePropertyChanged("NgTimes"); }
        }

        private Dictionary<string, string> _FilteringAlgorithmResult = new Dictionary<string, string>();
        /// <summary>
        /// 滤除算法结果词典
        /// </summary>
        public Dictionary<string, string> FilteringAlgorithmResult
        {
            get { return _FilteringAlgorithmResult; }
            set { _FilteringAlgorithmResult = value; RaisePropertyChanged("FilteringAlgorithmResult"); }
        }

        private string _FilteringAlgorithmResultStr = "";
        /// <summary>
        /// 滤除算法结果字符串
        /// </summary>
        public string FilteringAlgorithmResultStr
        {
            get { return _FilteringAlgorithmResultStr; }
            set { _FilteringAlgorithmResultStr = value; RaisePropertyChanged("FilteringAlgorithmResultStr"); }
        }
        #endregion

        #region 多个文件
        private ObservableCollection<string> _filepathlist;
        /// <summary>
        /// 导入数据文件路径
        /// </summary>
        public ObservableCollection<string> FilePathList
        {
            get { return _filepathlist; }
            set { _filepathlist = value; RaisePropertyChanged("FilePathList"); }
        }

        private ObservableCollection<string> _filenameList;
        /// <summary>
        /// 导入数据文件名（不含目录）
        /// </summary>
        public ObservableCollection<string> FileNameList
        {
            get { return _filenameList; }
            set { _filenameList = value; RaisePropertyChanged("FileNameList"); }
        }
        #endregion

    }
}
