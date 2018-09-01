using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NokiKanColle.Data
{
    /// <summary>
    /// 数据抽象类
    /// </summary>
    public abstract class DataWrapper
    {
        /// <summary>
        /// 键名
        /// </summary>
        public abstract string Name { set; get; }
        /// <summary>
        /// 键值
        /// </summary>
        public abstract string Value { set; get; }

        /// <summary>
        /// 数据集合
        /// </summary>
        public List<DataWrapper> DataCollection = new List<DataWrapper>();//类似树状图类，包含多个数据

    }
}
