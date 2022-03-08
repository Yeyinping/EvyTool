using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GHJTool.ViewModel
{
    public class RecursiveReadFileModel:INotifyPropertyChanged
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
        private string _InputDirPath;
        /// <summary>
        /// 输入递归目录路径
        /// </summary>
        public string InputDirPath
        {
            get { return _InputDirPath; }
            set { _InputDirPath = value; RaisePropertyChanged("InputDirPath"); }
        }

        private string _OutputDirPath;
        /// <summary>
        /// 输出文件目录路径
        /// </summary>
        public string OutputDirPath
        {
            get { return _OutputDirPath; }
            set { _OutputDirPath = value; RaisePropertyChanged("OutputDirPath"); }
        }
        #endregion
    }
}
