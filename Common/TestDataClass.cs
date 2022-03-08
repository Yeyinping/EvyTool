using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHJTool.Common
{
    /// <summary>
    /// QWP AA测试数据
    /// </summary>
    public class TestDataClass
    {
        int _npoint;
        /// <summary>
        /// 调节步数个数(点的个数)
        /// </summary>
        public int nPoint
        {
            get { return _npoint; }
            set { _npoint = value; }
        }

        List<double> _xarray = new List<double>();
        /// <summary>
        /// 角度数组
        /// </summary>
        public List<double> xArray
        {
            get { return _xarray; }
            set { _xarray = value; }
        }

        #region 不同类型能量数组

        List<double> _yGlobalSum = new List<double>();
        /// <summary>
        /// GlobalSum
        /// </summary>
        public List<double> yGlobalSum
        {
            get { return _yGlobalSum; }
            set { _yGlobalSum = value; }
        }

        List<double> _yRoiSum = new List<double>();
        /// <summary>
        /// RoiSum
        /// </summary>
        public List<double> yRoiSum
        {
            get { return _yRoiSum; }
            set { _yRoiSum = value; }
        }

        List<double> _yRoiAvg = new List<double>();
        /// <summary>
        /// RoiAvg
        /// </summary>
        public List<double> yRoiAvg
        {
            get { return _yRoiAvg; }
            set { _yRoiAvg = value; }
        }

        List<double> _yGlobalAvg = new List<double>();
        /// <summary>
        /// GlobalAvg
        /// </summary>
        public List<double> yGlobalAvg
        {
            get { return _yGlobalAvg; }
            set { _yGlobalAvg = value; }
        }

        List<double> _yRoiAvgWithoutBlackLevel = new List<double>();
        /// <summary>
        /// RoiAvgWithoutBlackLevel
        /// </summary>
        public List<double> yRoiAvgWithoutBlackLevel
        {
            get { return _yRoiAvgWithoutBlackLevel; }
            set { _yRoiAvgWithoutBlackLevel = value; }
        }

        List<double> _yRoiSumWithoutBlackLevel = new List<double>();
        /// <summary>
        /// RoiSumWithoutBlackLevel
        /// </summary>
        public List<double> yRoiSumWithoutBlackLevel
        {
            get { return _yRoiSumWithoutBlackLevel; }
            set { _yRoiSumWithoutBlackLevel = value; }
        }

        List<double> _yGlobalSumMonitorData = new List<double>();
        /// <summary>
        /// GlobalSumMonitorData
        /// </summary>
        public List<double> yGlobalSumMonitorData
        {
            get { return _yGlobalSumMonitorData; }
            set { _yGlobalSumMonitorData = value; }
        }
        #endregion

        #region 不同类型能量对应角度数组，主要用于能量筛选

        List<double> _xArrayGlobalSum = new List<double>();
        /// <summary>
        /// GlobalSum角度
        /// </summary>
        public List<double> xArrayGlobalSum
        {
            get { return _xArrayGlobalSum; }
            set { _xArrayGlobalSum = value; }
        }

        List<double> _xArrayRoiSum = new List<double>();
        /// <summary>
        /// RoiSum角度
        /// </summary>
        public List<double> xArrayRoiSum
        {
            get { return _xArrayRoiSum; }
            set { _xArrayRoiSum = value; }
        }

        List<double> _xArrayRoiAvg = new List<double>();
        /// <summary>
        /// RoiAvg角度
        /// </summary>
        public List<double> xArrayRoiAvg
        {
            get { return _xArrayRoiAvg; }
            set { _xArrayRoiAvg = value; }
        }

        List<double> _xArrayGlobalAvg = new List<double>();
        /// <summary>
        /// GlobalAvg角度
        /// </summary>
        public List<double> xArrayGlobalAvg
        {
            get { return _xArrayGlobalAvg; }
            set { _xArrayGlobalAvg = value; }
        }

        List<double> _xArrayRoiAvgWithoutBlackLevel = new List<double>();
        /// <summary>
        /// RoiAvgWithoutBlackLevel角度
        /// </summary>
        public List<double> xArrayRoiAvgWithoutBlackLevel
        {
            get { return _xArrayRoiAvgWithoutBlackLevel; }
            set { _xArrayRoiAvgWithoutBlackLevel = value; }
        }

        List<double> _xArrayRoiSumWithoutBlackLevel = new List<double>();
        /// <summary>
        /// RoiSumWithoutBlackLevel角度
        /// </summary>
        public List<double> xArrayRoiSumWithoutBlackLevel
        {
            get { return _xArrayRoiSumWithoutBlackLevel; }
            set { _xArrayRoiSumWithoutBlackLevel = value; }
        }

        List<double> _xArrayGlobalSumMonitorData = new List<double>();
        /// <summary>
        /// GlobalSumMonitorData角度
        /// </summary>
        public List<double> xArrayGlobalSumMonitorData
        {
            get { return _xArrayGlobalSumMonitorData; }
            set { _xArrayGlobalSumMonitorData = value; }
        }
        #endregion

        #region 不同数据类型能量趋势线斜率
        List<double> _trendGlobalSum = new List<double>();
        /// <summary>
        /// trendGlobalSum
        /// </summary>
        public List<double> trendGlobalSum
        {
            get { return _trendGlobalSum; }
            set { _trendGlobalSum = value; }
        }

        List<double> _trendRoiSum = new List<double>();
        /// <summary>
        /// trendRoiSum
        /// </summary>
        public List<double> trendRoiSum
        {
            get { return _trendRoiSum; }
            set { _trendRoiSum = value; }
        }

        List<double> _trendRoiAvg = new List<double>();
        /// <summary>
        /// trendRoiAvg
        /// </summary>
        public List<double> trendRoiAvg
        {
            get { return _trendRoiAvg; }
            set { _trendRoiAvg = value; }
        }

        List<double> _trendGlobalAvg = new List<double>();
        /// <summary>
        /// trendGlobalAvg
        /// </summary>
        public List<double> trendGlobalAvg
        {
            get { return _trendGlobalAvg; }
            set { _trendGlobalAvg = value; }
        }

        List<double> _trendRoiAvgWithoutBlackLevel = new List<double>();
        /// <summary>
        /// trendRoiAvgWithoutBlackLevel
        /// </summary>
        public List<double> trendRoiAvgWithoutBlackLevel
        {
            get { return _trendRoiAvgWithoutBlackLevel; }
            set { _trendRoiAvgWithoutBlackLevel = value; }
        }

        List<double> _trendRoiSumWithoutBlackLevel = new List<double>();
        /// <summary>
        /// trendRoiSumWithoutBlackLevel
        /// </summary>
        public List<double> trendRoiSumWithoutBlackLevel
        {
            get { return _trendRoiSumWithoutBlackLevel; }
            set { _trendRoiSumWithoutBlackLevel = value; }
        }
        #endregion

        public void Clear()
        {
            _npoint = new int();
            _xarray.Clear();
            _yGlobalSum.Clear();
            _yRoiAvg.Clear();
            _yRoiSum.Clear();
            _yGlobalAvg.Clear();
            _yRoiAvgWithoutBlackLevel.Clear();
            _yRoiSumWithoutBlackLevel.Clear();
            _yGlobalSumMonitorData.Clear();

            _xArrayGlobalAvg.Clear();
            _xArrayGlobalSum.Clear();
            _xArrayGlobalSumMonitorData.Clear();
            _xArrayRoiAvg.Clear();
            _xArrayRoiSum.Clear();
            _xArrayRoiAvgWithoutBlackLevel.Clear();
            _xArrayRoiSumWithoutBlackLevel.Clear();

            _trendGlobalAvg.Clear();
            _trendGlobalSum.Clear();
            _trendRoiAvg.Clear();
            _trendRoiSum.Clear();
            _trendRoiAvgWithoutBlackLevel.Clear();
            _trendRoiSumWithoutBlackLevel.Clear();
        }

        public void CopyTo(out TestDataClass destdata)
        {
            destdata = new TestDataClass();

            destdata.nPoint = this.nPoint;

            foreach (var it in this.xArray)
                destdata.xArray.Add(it);

            #region 能量
            foreach (var it in this.yGlobalSum)
                destdata.yGlobalSum.Add(it);

            foreach (var it in this.yGlobalAvg)
                destdata.yGlobalAvg.Add(it);

            foreach (var it in this.yRoiAvg)
                destdata.yRoiAvg.Add(it);

            foreach (var it in this.yRoiSum)
                destdata.yRoiSum.Add(it);

            foreach (var it in this.yRoiAvgWithoutBlackLevel)
                destdata.yRoiAvgWithoutBlackLevel.Add(it);

            foreach (var it in this.yRoiSumWithoutBlackLevel)
                destdata.yRoiSumWithoutBlackLevel.Add(it);

            foreach (var it in this.yGlobalSumMonitorData)
                destdata.yGlobalSumMonitorData.Add(it);
            #endregion

            #region 角度
            foreach (var it in this.xArrayGlobalSum)
                destdata.xArrayGlobalSum.Add(it);

            foreach (var it in this.xArrayGlobalAvg)
                destdata.xArrayGlobalAvg.Add(it);

            foreach (var it in this.xArrayRoiAvg)
                destdata.xArrayRoiAvg.Add(it);

            foreach (var it in this.xArrayRoiSum)
                destdata.xArrayRoiSum.Add(it);

            foreach (var it in this.xArrayRoiAvgWithoutBlackLevel)
                destdata.xArrayRoiAvgWithoutBlackLevel.Add(it);

            foreach (var it in this.xArrayRoiSumWithoutBlackLevel)
                destdata.xArrayRoiSumWithoutBlackLevel.Add(it);

            foreach (var it in this.xArrayGlobalSumMonitorData)
                destdata.xArrayGlobalSumMonitorData.Add(it);
            #endregion

            #region 趋势线斜率
            foreach (var it in this.trendGlobalAvg)
                destdata.trendGlobalAvg.Add(it);

            foreach (var it in this.trendGlobalSum)
                destdata.trendGlobalSum.Add(it);

            foreach (var it in this.trendRoiAvg)
                destdata.trendRoiAvg.Add(it);

            foreach (var it in this.trendRoiSum)
                destdata.trendRoiSum.Add(it);

            foreach (var it in this.trendRoiAvgWithoutBlackLevel)
                destdata.trendRoiAvgWithoutBlackLevel.Add(it);

            foreach (var it in this.trendRoiSumWithoutBlackLevel)
                destdata.trendRoiSumWithoutBlackLevel.Add(it);
            #endregion
        }


    }

}
