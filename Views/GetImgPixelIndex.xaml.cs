using Microsoft.Win32;
using OpenCvSharp;
using System;
using System.Collections.Generic;
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
    /// GetImgPixelIndex.xaml 的交互逻辑
    /// </summary>
    public partial class GetImgPixelIndex : UserControl
    {
        GisVisionEx.GisVisionEx gisVisionEx = new GisVisionEx.GisVisionEx();

        public GetImgPixelIndex()
        {
            InitializeComponent();
        }

        private void ShowLog(string logstr)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                txtGetImgPixelIndexLog.AppendText($"{logstr}\r\n");
            }));
        }

        string FilePath = "";
        private void GetValue_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            string strErr = "";
            try
            {
                btn.IsEnabled = false;

                Mat src = Cv2.ImRead(FilePath, ImreadModes.AnyColor);

                #region 转换成单通道

                if (src.Channels() == 3)
                {
                    //转换成单通道
                    Cv2.CvtColor(src, src, ColorConversionCodes.BGR2GRAY);
                }
                else if(src.Channels() == 1)
                {
                    #region
                    //Mat image = src.Clone();
                    //Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2BGR);//用于显示，将灰度图转成彩色图

                    ////16位灰度图需要转换成8位的在进行二值化处理
                    //int depth = src.Depth();
                    //if (depth > 8 && depth < 16)
                    //{
                    //    double num = Math.Pow(2, depth - 8);
                    //    src /= num;
                    //    src.ConvertTo(src, MatType.CV_8UC1);
                    //}

                    ////获取最大灰度值
                    //Cv2.MinMaxLoc(src, out double minValue, out double maxValue, out OpenCvSharp.Point minloc, out OpenCvSharp.Point maxloc, null);
                    //string str = $"{MethodBase.GetCurrentMethod().Name}:获取最大灰度值坐标(maxX,maxY,maxValue)=({maxloc.X},{maxloc.Y},{maxValue})";

                    ////二值化处理
                    //double threshold = Cv2.Threshold(src, src, maxValue - 1, 255, ThresholdTypes.Binary);

                    ////形态学处理，MorphTypes.Open开操作，先腐蚀后膨胀，滤除很小的斑点
                    //Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));
                    //Cv2.MorphologyEx(src, src, MorphTypes.Open, element);
                    //Cv2.ImShow("MorphologyEx_Open", src);//显示处理后的图像
                    //src.ImWrite(@"D:\data\testimg\MorphologyEx_Open.png");

                    ////形态学处理，MorphTypes.Close闭操作，先膨胀后腐蚀，膨胀处理的斑点不可修复
                    ////Mat element1 = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(21, 21));
                    ////Cv2.MorphologyEx(src, src, MorphTypes.Close, element1);
                    ////Cv2.ImShow("MorphologyEx_Close", src);//显示处理后的图像

                    ////查找轮廓，RetrievalModes.External处理外轮廓（不在查找内部的轮廓）
                    //Cv2.FindContours(src, out OpenCvSharp.Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);
                    //Cv2.DrawContours(image, contours, -1, new Scalar(0, 0, 255),2);//显示查找到的轮廓
                    //image.ImWrite(@"D:\data\testimg\FindContours.png");

                    ////遍历输出查找到最亮灰度值区域对应的外接矩形
                    //for (int i = 0; i < contours.Length; i++)
                    //{
                    //    RotatedRect r_rect = Cv2.MinAreaRect(contours[i]);
                    //    Point2f[] coner = r_rect.Points();
                    //    for(int n = 0; n < 4; n++)
                    //    {
                    //        Cv2.Line(image, (OpenCvSharp.Point)coner[n] , (OpenCvSharp.Point)coner[(n + 1) % 4], new Scalar(0, 0, 255), 2);
                    //    }

                    //    image.ImWrite($@"D:\data\testimg\MinAreaRect_{i}_rx{r_rect.Center.X}_ry{r_rect.Center.Y}_angle{r_rect.Angle}.png");

                    //    ShowLog($"最亮点区域{i},外接矩形中心({r_rect.Center.X},{r_rect.Center.Y}),角度({r_rect.Angle}),宽*高({r_rect.Size.Width}*{r_rect.Size.Height})");
                    //}
                    #endregion
                }
                else
                {
                    ShowLog($"图像{FilePath},通道异常");
                    return;
                }
                #endregion

                string datatimestr = DateTime.Now.ToString("yyyyMMddHHmmss");
                string path = $@"D:\data\testimg\{datatimestr}";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                #region 图像处理
                Mat image = src.Clone();
                Cv2.CvtColor(image, image, ColorConversionCodes.GRAY2BGR);//用于显示，将灰度图转成彩色图

                //16位灰度图需要转换成8位的在进行二值化处理
                int depth = src.Depth();
                if (depth > 8 && depth < 16)
                {
                    double num = Math.Pow(2, depth - 8);
                    src /= num;
                    src.ConvertTo(src, MatType.CV_8UC1);
                }

                //高斯去噪
                Cv2.GaussianBlur(src, src, new OpenCvSharp.Size(21, 21), 21);

                //获取最大灰度值
                Cv2.MinMaxLoc(src, out double minValue, out double maxValue, out OpenCvSharp.Point minloc, out OpenCvSharp.Point maxloc, null);
                string str = $"{MethodBase.GetCurrentMethod().Name}:获取最大灰度值坐标(maxX,maxY,maxValue)=({maxloc.X},{maxloc.Y},{maxValue})";
                ShowLog(str);

                //二值化处理
                double threshold = Cv2.Threshold(src, src, maxValue*0.5, 255, ThresholdTypes.Binary);

                //形态学处理，MorphTypes.Open开操作，先腐蚀后膨胀，滤除很小的斑点
                Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(9, 9));
                Cv2.MorphologyEx(src, src, MorphTypes.Open, element);
                Cv2.ImShow("MorphologyEx_Open", src);//显示处理后的图像
                src.ImWrite($@"{path}\MorphologyEx_Open.png");


                ////形态学处理，MorphTypes.Close闭操作，先膨胀后腐蚀，膨胀处理的斑点不可修复
                //Mat element1 = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(21, 21));
                //Cv2.MorphologyEx(src, src, MorphTypes.Close, element1);
                //Cv2.ImShow("MorphologyEx_Close", src);//显示处理后的图像

                //查找轮廓，RetrievalModes.External处理外轮廓（不在查找内部的轮廓）
                Cv2.FindContours(src, out OpenCvSharp.Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);
                Cv2.DrawContours(image, contours, -1, new Scalar(0, 0, 255), 2);//显示查找到的轮廓
                image.ImWrite($@"{path}\FindContours.png");

                if(contours.Length<1)
                {
                    ShowLog("当前图像查找不到轮廓");
                    return;
                }

                //遍历输出查找到最亮灰度值区域对应的外接矩形
                for (int i = 0; i < contours.Length; i++)
                {
                    #region 外接矩形
                    //RotatedRect r_rect = Cv2.MinAreaRect(contours[i]);
                    //Point2f[] coner = r_rect.Points();
                    //for (int n = 0; n < 4; n++)
                    //{
                    //    Cv2.Line(image, (OpenCvSharp.Point)coner[n], (OpenCvSharp.Point)coner[(n + 1) % 4], new Scalar(0, 0, 255), 2);
                    //}
                    //Cv2.Line(image, (int)(r_rect.Center.X-10),(int)(r_rect.Center.Y), (int)(r_rect.Center.X + 10), (int)(r_rect.Center.Y),new Scalar(0, 0, 255), 4);
                    //Cv2.Line(image, (int)(r_rect.Center.X), (int)(r_rect.Center.Y - 10), (int)(r_rect.Center.X), (int)(r_rect.Center.Y + 10), new Scalar(0, 0, 255), 4);

                    //image.ImWrite($@"{path}\MinAreaRect_{i}_rx{r_rect.Center.X}_ry{r_rect.Center.Y}_angle{r_rect.Angle}.png");

                    ////ShowLog($"最亮点区域{i},外接矩形中心({r_rect.Center.X},{r_rect.Center.Y}),角度({r_rect.Angle}),宽*高({r_rect.Size.Width}*{r_rect.Size.Height})");
                    #endregion

                    #region 内切圆
                    //int dist = 0, maxdist = 0;
                    //OpenCvSharp.Point center = new OpenCvSharp.Point();
                    //for (int n = 0; n < src.Cols; n++)
                    //{
                    //    for (int m = 0; m < src.Rows; m++)
                    //    {
                    //        dist = (int)Cv2.PointPolygonTest(contours[i], new Point2f(n, m), true);
                    //        if (dist > maxdist)
                    //        {
                    //            maxdist = dist;
                    //            center = new OpenCvSharp.Point(n, m);
                    //        }
                    //    }
                    //}
                    //Cv2.Circle(image, (OpenCvSharp.Point)center, (int)maxdist, new Scalar(0, 255, 255));
                    //Cv2.Line(image, (int)(center.X - 10), (int)(center.Y), (int)(center.X + 10), (int)(center.Y), new Scalar(0, 0, 255), 4);
                    //Cv2.Line(image, (int)(center.X), (int)(center.Y - 10), (int)(center.X), (int)(center.Y + 10), new Scalar(0, 0, 255), 4);
                    //image.ImWrite($@"{path}\Circle{i}_cx{center.X}_cy{center.Y}_r{maxdist}.png");

                    Cv2.MinEnclosingCircle(contours[i], out Point2f center, out float radius); 
                    Cv2.Circle(image, (OpenCvSharp.Point)center, (int)radius, new Scalar(0, 255, 255));
                    Cv2.Line(image, (int)(center.X - 10), (int)(center.Y), (int)(center.X + 10), (int)(center.Y), new Scalar(0, 0, 255), 4);
                    Cv2.Line(image, (int)(center.X), (int)(center.Y - 10), (int)(center.X), (int)(center.Y + 10), new Scalar(0, 0, 255), 4);
                    image.ImWrite($@"{path}\Circle{i}_cx{center.X}_cy{center.Y}_r{radius}.png");
                    #endregion
                }

                #endregion
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
            }
            finally
            {
                btn.IsEnabled = true;
            }
        }

        private void LoadPath_Click(object sender, RoutedEventArgs e)
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
                dialog.Filter = "PNG文件|*.png|JPEG文件|*.jpeg|BMP文件|*.bmp";

                var dlgresult = dialog.ShowDialog();
                if (dlgresult == true)
                {
                    FilePath = dialog.FileName;
                }

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    txtPath.Text = FilePath;
                }));
            }
            catch (Exception exp)
            {
                strErr = $"{MethodBase.GetCurrentMethod().Name}：{exp.Message},{exp.StackTrace}";
            }
            finally
            {
                btn.IsEnabled = true;
            }
        }
    }
}