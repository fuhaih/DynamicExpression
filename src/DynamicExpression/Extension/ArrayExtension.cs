using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicExpression.Extension
{
    /// <summary>
    /// 数组扩展方法
    /// </summary>
    public static class ArrayExtension
    {
        /// <summary>
        /// 判断两个数组元素是否依次想相等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="souce"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static bool ArrayEquals<T>(this T[] souce, T[] compare)
        { 
            if(souce.Length!=compare.Length) return false;
            for (int i = 0; i < souce.Length; i++)
            { 
                if(!souce[i].Equals(compare[i])) return false;
            }
            return true;
        }
    }
}
