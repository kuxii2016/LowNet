using System;
using System.Collections.Generic;
using System.Text;

namespace LowNet.Utils
{
    public class ClassUtils
    {
        /// <summary>
        /// Get Classname and more Infos, For Ligging
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string TryGetClass(object script)
        {
            return script?.GetType().Namespace.Normalize() + "." + script?.GetType().Name.Normalize();
        }
    }
}