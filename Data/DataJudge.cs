using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NokiKanColle.Data
{
    /// <summary>
    /// 颜色集合,可以将颜色排序成一维或二维([Y][X])
    /// </summary>
    public class ColorCollection
    {
        /// <summary>
        /// 颜色集合colorCollection[y][x]
        /// </summary>
        private List<List<Color>> _colorArray = new List<List<Color>>();
        /// <summary>
        /// 颜色集合的长度（最大位置的点的x坐标为该值-1）
        /// </summary>
        public int X
        { get { return _colorArray.Count; } }
        /// <summary>
        /// 颜色集合的宽度（最大位置的点的y坐标为该值-1）
        /// </summary>
        public int Y
        {
            get
            {
                int y = 0;
                if (!IsExist()) return 0;
                foreach (List<Color> item in _colorArray)
                { if ((item.Count) > y) y = item.Count; }
                return y;
            }
        }
        /// <summary>
        /// 包含的颜色数
        /// </summary>
        public int Count { get { return X * Y; } }

        /// <summary>
        /// 获取二维颜色集的某一颜色
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color this[int x, int y]
        {
            get
            {
                if (_colorArray.Count > x && x > -1)
                {
                    if (_colorArray[x].Count > y && y > -1)
                    {
                        return _colorArray[x][y];
                    }
                }
                throw new Exceptions.DataErrorException("颜色数据数组下标异常！");
            }
            set
            {
                if (_colorArray.Count > x && x > -1)
                {
                    if (_colorArray[x].Count > y && y > -1)
                    { _colorArray[x][y] = value; }
                }

            }
        }

        /// <summary>
        /// 判断颜色集合中是否拥有数据
        /// </summary>
        /// <returns></returns>
        public bool IsExist()
        {
            if (_colorArray.Count > 0)
            {
                foreach (List<Color> item in _colorArray)
                {
                    if (item.Count > 0) continue;
                    else return false;
                }
            }
            else return false;
            return true;
        }
        /// <summary>
        /// 判断颜色集合中是否拥有该数据
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsExist(uint x, uint y = 0)
        {
            if (_colorArray.Count > x)
            {
                foreach (List<Color> item in _colorArray)
                {
                    if (item.Count > y) continue;
                    else return false;
                }
            }
            else return false;
            return true;
        }

        /// <summary>
        /// 读取为String
        /// </summary>
        /// <returns></returns>
        public string Read()
        {
            if (!IsExist()) return "";
            string str = "";

            for (int i = 0; i < _colorArray.Count; i++)
            {
                if (i == _colorArray.Count - 1)//最后一行
                {
                    for (int j = 0; j < _colorArray[i].Count; j++)
                    {
                        if (j == _colorArray[i].Count - 1)//最后一项
                            str += Function.Functions.ColorToStr16(_colorArray[i][j]);
                        else
                            str += Function.Functions.ColorToStr16(_colorArray[i][j]) + "|";
                    }
                }
                else
                {
                    for (int j = 0; j < _colorArray[i].Count; j++)
                    {
                        if (j == _colorArray[i].Count - 1)//最后一项
                            str += Function.Functions.ColorToStr16(_colorArray[i][j]);
                        else
                            str += Function.Functions.ColorToStr16(_colorArray[i][j]) + "|";
                    }
                    str += ",";
                }
            }
            return str;
        }
        /// <summary>
        /// 读取某一点的颜色
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public string ReadColor(int x, int y = 0)
        {
            if (!IsExist()) return "";
            if (!IsExist((uint)x, (uint)y)) return "";
            return Function.Functions.ColorToStr16(this[x, y]);
        }
        /// <summary>
        /// 写入颜色集合
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool Write(string Value)
        {
            if (Value == "") return false;
            try
            {
                string str = Value;
                _colorArray.Clear();

                string[] ColorHeight = str.Split(',');//整合颜色数据
                foreach (string i in ColorHeight)
                {
                    string[] ColorArray = i.Split('|');
                    List<Color> ChildList = new List<Color>();
                    foreach (string j in ColorArray)
                    {
                        ChildList.Add(Function.Functions.StringToColor(j));
                    }
                    _colorArray.Add(ChildList);
                }
                return true;
            }
            catch (Exception e)
            {
                Function.FunctionExceptionLog.Write("写入颜色信息失败。", e);
                _colorArray.Clear(); return false;
            }
        }

        public ColorCollection()
        {

        }
        public ColorCollection(string Value)
        {
            Write(Value);
        }
    }

    /// <summary>
    /// 用于进行页面判定的数据类
    /// </summary>
    public class DataJudge : DataWrapper
    {
        //需要初始化的参数
        /// <summary>
        /// 当前页面的名称
        /// </summary>
        public override string Name { set; get; }
        /// <summary>
        /// 键值（xy + " @ " + color）
        /// </summary>
        public override string Value
        {
            get { return XYToStr + " @ " + Color; }
            set
            {
                try
                {
                    string[] i = System.Text.RegularExpressions.Regex.Split(value, " @ "); //value.Split( ' ','@',' ');
                    XYToStr = i[0]; Color = i[1];
                }
                catch (Exception e)
                {
                    Function.FunctionExceptionLog.Write("写入判定坐标信息失败。", e);
                    XYToStr = "0,0"; Color = "";
                }
            }
        }
        /// <summary>
        /// 当为假时，若判断页面为假则返回真，判断页面为真则返回假
        /// </summary>
        public bool IsTrueResult = true;
        /// <summary>
        /// 需要判断的图像色集
        /// </summary>
        public string Color
        {
            set { ColorCollection.Write(value); }
            get { return ColorCollection.Read(); }
        }
        /// <summary>
        /// 需要判断的图像的起始坐标（相对于游戏窗口左上角）
        /// </summary>
        public string XYToStr
        {
            set
            {
                try
                {
                    string[] p = value.Split(',');
                    XY = new Point(Convert.ToInt32(p[0]), Convert.ToInt32(p[1]));
                }
                catch (Exception e)
                {
                    throw new Exceptions.DataErrorException($"图像的起始坐标数据错误！{e.Message}");
                }
            }
            get
            {
                return XY.X.ToString() + "," + XY.Y.ToString();
            }
        }
        /// <summary>
        /// 需要判断的图像的宽度
        /// </summary>
        public int Width
        { get { return ColorCollection.Y; } }
        /// <summary>
        /// 需要判断的图像的高度
        /// </summary>
        public int Height
        { get { return ColorCollection.X; } }

        /// <summary>
        /// 需要判断的图像的起始坐标（相对于游戏窗口左上角）
        /// </summary>
        public Point XY = Point.Empty;

        /// <summary>
        /// 起始坐标后的图像
        /// </summary>
        public ColorCollection ColorCollection = new ColorCollection();

        public DataJudge this[int i]
        {
            get => DataCollection[i] as DataJudge;
            set => DataCollection[i] = value;
        }

        public override string ToString()
        {
            return $"{this.Name}:{this.Value}";
        }

        /// <summary>
        /// 新建用于进行页面判定的数据
        /// </summary>
        /// <param name="value">键值（"xy" + " @ " + "color"）</param>
        /// <param name="name">键名</param>
        /// <param name="isNot">当为假时，若判断页面为假则返回真，判断页面为真则返回假</param>
        public DataJudge(string value, string name = "", bool isNot = true)
        {
            this.Name = name;
            this.Value = value;
            this.IsTrueResult = isNot;
        }
        public DataJudge(List<DataJudge> list)
        {
            foreach (var item in list)
            {
                DataCollection.Add(item);
            }
        }
    }
}
