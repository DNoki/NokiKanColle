using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static NokiKanColle.Function.Functions;

namespace NokiKanColle.Data
{
    /// <summary>
    /// 点击指令
    /// </summary>
    public class DataClick : DataWrapper
    {
        /// <summary>
        /// 该点击区域名字
        /// </summary>
        public override string Name { get; set; }
        /// <summary>
        /// 顶点字符串
        /// </summary>
        public override string Value
        {
            get { return Read(); }
            set { Write(value); }
        }
        /// <summary>
        /// 构成形状的一系列首尾相连的点（相对于游戏窗口左上角坐标）
        /// </summary>
        private List<Point> _coordinate = new List<Point>();

        /// <summary>
        /// 该区域拥有的顶点数量（两点则表示矩阵）
        /// </summary>
        public int Count
        { get { return _coordinate.Count; } }
        public DataClick this[int i]
        {
            get => DataCollection[i] as DataClick;
            set => DataCollection[i] = value;
        }

        /// <summary>
        /// 在一个多边形内随机取得一点坐标
        /// </summary>
        /// <returns></returns>
        private Point RanPolygonXY()
        {
            //思路：在一个多边形内随机取得一点坐标
            //先随机取得x的值，根据x的值取得y的极值（范围），再随机取得y的值
            //*不适用内多边形
            Point xy = Point.Empty;
            int XMax = 0, YMax = 0;
            int XMin = 2147483647, YMin = 2147483647;

            foreach (Point I in _coordinate)
            {
                if (I.X > XMax) XMax = I.X;//取X最大值
                if (I.X < XMin) XMin = I.X;//取X最小值
                if (I.Y > YMax) YMax = I.Y;//取Y最大值
                if (I.Y < YMin) YMin = I.Y;//取Y最小值
            }

            xy.X = Math.Abs(Random(XMin, XMax + 1));//取得x的值

            /*获取y的取值范围（极值）*/
            List<int> YExtremum = new List<int>();
            for (int i = 0; i < Count; i++)
            {
                if (XMin == XMax)
                {
                    YExtremum.Add(YMax);
                    YExtremum.Add(YMin);
                    break;
                }
                if (i == Count - 1)//最后一个点
                {
                    //判断x是否在最后一个点与第一个点之间
                    if ((_coordinate[i].X >= xy.X && xy.X >= _coordinate[0].X) || (_coordinate[i].X <= xy.X && xy.X <= _coordinate[0].X))
                    {
                        if ((_coordinate[0].X - _coordinate[i].X) == 0) continue;
                        /*/
                                   Y = ( X - X2 )( Y1 - Y2 )  
                                       ---------------------  + Y2
                                           ( X1 - X2 )
                        /*/
                        YExtremum.Add(Math.Abs(
                            ((xy.X - _coordinate[i].X) * (_coordinate[0].Y - _coordinate[i].Y))
                            / (_coordinate[0].X - _coordinate[i].X) + _coordinate[i].Y));
                    }
                    continue;
                }
                //判断x是否在两点线段之间
                if ((_coordinate[i].X >= xy.X && xy.X >= _coordinate[i + 1].X) || (_coordinate[i].X <= xy.X && xy.X <= _coordinate[i + 1].X))
                {
                    if ((_coordinate[i + 1].X - _coordinate[i].X) == 0) continue;
                    YExtremum.Add(Math.Abs(
                        ((xy.X - _coordinate[i].X) * (_coordinate[i + 1].Y - _coordinate[i].Y)) /
                        (_coordinate[i + 1].X - _coordinate[i].X) + _coordinate[i].Y));
                }
            }
            xy.Y = Math.Abs(Function.Functions.Random(YExtremum.Min(), YExtremum.Max() + 1));
            return xy;
        }

        /// <summary>
        /// 根据坐标数据随机产生一个坐标
        /// </summary>
        /// <returns></returns>
        public Point GetXY()
        {
            if (IsExist())
            {
                return RanPolygonXY();
            }
            else return Point.Empty;
        }

        /// <summary>
        /// 判断矩阵中是否拥有数据
        /// </summary>
        /// <returns></returns>
        public bool IsExist()
        { if (_coordinate.Count > 0) return true; else return false; }

        private string Read()
        {
            if (!IsExist()) return "";
            string str = "";

            for (int i = 0; i < _coordinate.Count; i++)
            {
                if (i == _coordinate.Count - 1)
                    str += _coordinate[i].X + "," + _coordinate[i].Y;
                else str += _coordinate[i].X + "," + _coordinate[i].Y + "|";
            }
            return str;
        }
        private bool Write(string Value)
        {
            if (Value == "") return false;
            try
            {
                string str = Value;
                _coordinate.Clear();

                string[] shape = str.Split('|');
                foreach (string i in shape)
                {
                    string[] pointArray = i.Split(',');
                    _coordinate.Add(new Point(Convert.ToInt32(pointArray[0]), Convert.ToInt32(pointArray[1])));
                }
                return true;
            }
            catch (Exception e)
            {
                Function.FunctionExceptionLog.Write("写入点击数据失败。", e);
                _coordinate.Clear(); return false;
            }
        }

        public override string ToString()
        {
            return $"{this.Name}:{this.Value}";
        }

        /// <summary>
        /// 新建点击数据对象
        /// </summary>
        /// <param name="value">值（"坐标1|坐标2……|坐标n"）</param>
        /// <param name="name">名称</param>
        public DataClick(string value, string name = "")
        {
            this.Name = name; this.Value = value;
        }
        /// <summary>
        /// 新建点击数据对象列表
        /// </summary>
        /// <param name="list"></param>
        public DataClick(List<DataClick> list)
        {
            foreach (var item in list)
            {
                DataCollection.Add(item);
            }
        }
    }
}
