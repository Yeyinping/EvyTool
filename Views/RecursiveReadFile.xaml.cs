using GHJTool.ViewModel;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace GHJTool.Views
{
    /// <summary>
    /// RecursiveReadFile.xaml 的交互逻辑
    /// </summary>
    public partial class RecursiveReadFile : UserControl
    {
        RecursiveReadFileModel recursiveReadFileModel = new RecursiveReadFileModel();

        public RecursiveReadFile()
        {
            InitializeComponent();
            this.DataContext = recursiveReadFileModel;

            StartThread();
        }

        #region Thread

        bool RecursiveDirEnable = false;

        Thread thRecursiveDir = null;
        bool IsThRecursiveDirEnable = false;

        public void StartThread()
        {
            try
            {

                if (thRecursiveDir == null || !thRecursiveDir.IsAlive)
                {
                    IsThRecursiveDirEnable = true;
                    thRecursiveDir = new Thread(ThFunRecursiveDir)
                    {
                        Name = $"递归目录线程",
                        IsBackground = true
                    };
                    thRecursiveDir.Start();
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
                IsThRecursiveDirEnable = false;

                if (thRecursiveDir != null && thRecursiveDir.IsAlive)
                {
                    thRecursiveDir.Abort();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
            }
        }

        private void ThFunRecursiveDir()
        {
            string strErr = "";

            while (IsThRecursiveDirEnable)
            {
                Thread.Sleep(5);
                try
                {
                    if (RecursiveDirEnable)
                    {
                        RecursiveDirEnable = false;

                        string inputdirpath = this.recursiveReadFileModel.InputDirPath;
                        string outputdirpath = this.recursiveReadFileModel.OutputDirPath;
                        RecursiveReadDir(inputdirpath, outputdirpath);
                    }
                }
                catch (Exception e)
                {
                    strErr = $"{MethodBase.GetCurrentMethod().Name}异常：{e.Message}，{e.StackTrace}";

                }
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btnRecursiveDir.IsEnabled = true;
                }));
            }
        }


        #endregion

        #region ClickEvent
        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            string strErr = "";

            var btn = sender as Button;
            try
            {
                btn.IsEnabled = false;

                RecursiveDirEnable = true;
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
            }

        }

        private void BtnOutputDir_Click(object sender, RoutedEventArgs e)
        {
            string strErr = "";

            var btn = sender as Button;
            try
            {
                btn.IsEnabled = false;

                string path = GetPathFromOpenFileDialog();
                if (path != null || path != "")
                    this.recursiveReadFileModel.OutputDirPath = path;

            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
            }
            finally
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    btn.IsEnabled = true;
                }));
            }
        }

        private void BtnInputDir_Click(object sender, RoutedEventArgs e)
        {
            string strErr = "";

            var btn = sender as Button;
            try
            {
                btn.IsEnabled = false;

                string path = GetPathFromOpenFileDialog();
                if (path != null || path != "")
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.recursiveReadFileModel.InputDirPath = path;

                    }));
                }

            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
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

        #region Method
        private string GetPathFromOpenFileDialog()
        {
            string strErr = "";
            try
            {

                CommonOpenFileDialog open = new CommonOpenFileDialog();
                open.IsFolderPicker = true; //false:pick file,true:pick folder
                open.EnsureReadOnly = true;
                open.RestoreDirectory = true;
                open.Multiselect = false;
                open.Title = "打开目录";
                if (open.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return open.FileName;
                }

            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
            }

            return null;
        }

        private string ReadFile(string readfile)
        {
            #region 读取文件
            string strdata = string.Empty;
            using (StreamReader mysr = new StreamReader(readfile, System.Text.Encoding.UTF8))
            {
                strdata = mysr.ReadToEnd();
            }
            #endregion

            return strdata;
        }

        private void WriteFile(string writefile, string writedata)
        {
            using (FileStream fs = new FileStream(writefile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                //若不使用using StreamWriter需要程序源等待写入完成关闭，
                //否则提示“**文件正在被另一个进程使用，不允许执行操作”；
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(writedata);
                }
            }
        }

        private void RecursiveReadDir(string dirpath,string destdirpath)
        {
            string strErr = "";
            DirectoryInfo dir = new DirectoryInfo(dirpath);
            try
            {
                //判断所指的文件夹/文件是否存在   
                if (!dir.Exists)
                    return;
                DirectoryInfo dirD = dir as DirectoryInfo;
                FileSystemInfo[] files = dirD.GetFileSystemInfos();//获取文件夹下所有文件和文件夹   
                                                                   //对单个FileSystemInfo进行判断，如果是文件夹则进行递归操作   
                foreach (FileSystemInfo FSys in files)
                {
                    FileInfo fileInfo = FSys as FileInfo;

                    if (fileInfo != null)
                    {
                        //如果是文件，进行文件操作   
                        FileInfo readfile = new FileInfo(fileInfo.DirectoryName + "\\" + fileInfo.Name);//获取文件所在原始路径  

                        string writefile = null;
                       
                        string strExtension = System.IO.Path.GetExtension(fileInfo.Name);

                        switch (strExtension)
                        {
                            case ".cs":
                            case ".c":
                            case ".h":
                            case ".cpp":
                            case ".md":
                            case ".txt":
                            case ".cc":
                            case ".json":
                            case ".py":
                            case ".js":
                                {
                                    strExtension = $"{System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name)}_{strExtension.Substring(1)}";
                                    writefile = System.IO.Path.Combine(destdirpath, strExtension);
                                }
                                break;
                            default :
                                int _index = fileInfo.Name.LastIndexOf('_');
                                if (-1 == _index)
                                {
                                    writefile = System.IO.Path.Combine(destdirpath, fileInfo.Name);
                                }
                                else
                                {
                                    string name = fileInfo.Name.Substring(0, _index);
                                    string extension = fileInfo.Name.Substring(_index + 1);

                                    switch(extension)
                                    {
                                        
                                        case "cs":
                                        case "c":
                                        case "h":
                                        case "cpp":
                                        case "md":
                                        case "txt":
                                        case "cc":
                                        case "json":
                                        case "py":
                                        case "js":
                                            string newname = $"{name}.{extension}";
                                            writefile = System.IO.Path.Combine(destdirpath, newname);
                                            break;

                                        default:
                                            writefile = System.IO.Path.Combine(destdirpath, fileInfo.Name);
                                            break;
                                    }
                                }
                                break;
                        }

                        string wdata = ReadFile(readfile.FullName);
                        WriteFile(writefile, wdata);
                    }
                    else
                    {
                        //如果是文件夹，则进行递归调用  
                        string pp = FSys.Name;
                        string writedir = System.IO.Path.Combine(destdirpath, FSys.ToString());
                        if (!Directory.Exists(writedir))
                            Directory.CreateDirectory(writedir);
                        RecursiveReadDir(dirpath + "\\" + FSys.ToString(), writedir);
                    }
                }
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
            }
        }
        #endregion
    }
}
