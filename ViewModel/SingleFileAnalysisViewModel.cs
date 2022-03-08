using GHJTool.Common;
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
    public class SingleFileAnalysisViewModel : INotifyPropertyChanged
    {
        #region Interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region TreeViewNodeItem

        private List<TreeViewNodeItem> _tvNodeItems = new List<TreeViewNodeItem>();

        public List<TreeViewNodeItem> TvNodeItems
        {
            get { return _tvNodeItems; }
            set { _tvNodeItems = value; RaisePropertyChanged("TvNodeItems"); }
        }
        public SingleFileAnalysisViewModel()
        {
            TreeViewNodeItem rootnode = new TreeViewNodeItem() ;
            TvNodeItems.Add(rootnode);
        }
        #endregion

        #region PeakUIModel
        private int _node = 2;
        /// <summary>
        /// 拟合阶数
        /// </summary>
        public int Node
        {
            get { return _node; }
            set { _node = value; RaisePropertyChanged("Node"); }
        }

        private GisVisionEx.CalcAlgoTypeEnum _curvetype;
        /// <summary>
        /// 拟合阶数
        /// </summary>
        public GisVisionEx.CalcAlgoTypeEnum CurveType
        {
            get { return _curvetype; }
            set { _curvetype = value; RaisePropertyChanged("CurveType"); }
        }
        

        #region 单个文件
        private string _filepath;
        /// <summary>
        /// 导入数据文件路径
        /// </summary>
        public string FilePath
        {
            get { return _filepath; }
            set { _filepath = value; RaisePropertyChanged("FilePath"); }
        }

        private string _filename;
        /// <summary>
        /// 导入数据文件名（不含目录）
        /// </summary>
        public string FileName
        {
            get { return _filename; }
            set { _filename = value; RaisePropertyChanged("FileName"); }
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

        private Dictionary<string, string> _dicPeakPos = new Dictionary<string, string>();
        /// <summary>
        /// 各数据类型Peak坐标词典
        /// </summary>
        public Dictionary<string, string> DicPeakPos
        {
            get { return _dicPeakPos; }
            set { _dicPeakPos = value; RaisePropertyChanged("DicPeakPos"); }
        }

        #endregion

        #region ExportResult
        private bool _isExportResult2Csv = true;
        /// <summary>
        /// 导出结果到文件
        /// </summary>
        public bool IsExportResult2Csv
        {
            get { return _isExportResult2Csv; }
            set { _isExportResult2Csv = value; RaisePropertyChanged("IsExportResult2Csv"); }
        }

        private bool _isExportResult2SameCsv = false;
        /// <summary>
        /// 导出结果到同一文件
        /// </summary>
        public bool IsExportResult2SameCsv
        {
            get { return _isExportResult2SameCsv; }
            set { _isExportResult2SameCsv = value; RaisePropertyChanged("IsExportResult2SameCsv"); }
        }
        
        #endregion
    }

    public class TreeViewNodeItem
    {
        public string Content { get; set; }
        public ObservableCollection<TreeViewNodeItem> Children { get; set; }
        public TreeViewNodeItem()
        {
            Children = new ObservableCollection<TreeViewNodeItem>();
        }
    }

}
