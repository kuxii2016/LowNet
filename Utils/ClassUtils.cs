/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using System;
using System.Text;

namespace LowNet.Utils
{
    /// <summary>
    /// Lownet Class Utils
    /// </summary>
    public class ClassUtils
    {
        /// <summary>
        /// Checksum Datalist
        /// </summary>
        static uint[] CRC_DATA_SET = new uint[]
        {
            0x0, 0x1DB71064, 0x3B6E20C8, 0x26D930AC,
            0x76DC4190, 0x6B6B51F4, 0x4DB26158, 0x5005713C,
            0xEDB88320, 0xF00F9344, 0xD6D6A3E8, 0xCB61B38C,
            0x9B64C2B0, 0x86D3D2D4, 0xA00AE278, 0xBDBDF21C
        };

        /// <summary>
        /// Get Classname and more Infos, For Ligging
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string TryGetClass(object script)
        {
            string parsed = "";
            try
            {
                if (script != null)
                {
                    parsed = script.GetType().Namespace.Normalize();
                    parsed += "." + script.GetType().Name.Normalize();
                }
            }
            catch (Exception)
            {
                parsed = "null";
            }
            return parsed;
        }

        /// <summary>
        /// Get Checksum 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static uint CalculateChecksum(string str)
        {
            uint lowerNibbleResult;
            uint output;
            output = 0xffffffff;
            foreach (byte currentByte in Encoding.ASCII.GetBytes(str))
            {
                lowerNibbleResult = output >> 4 ^ CRC_DATA_SET[(output ^ currentByte) & 0xf];
                output = lowerNibbleResult >> 4 ^ CRC_DATA_SET[(lowerNibbleResult ^ currentByte >> 4) & 0xf];
            }
            return ~output;
        }

        /// <summary>
        /// Get Checksum 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// 
        public static uint CalculateShortChecksum(string str)
        {
            uint result = CalculateChecksum(str);
            return (ushort)((ushort)((result ^ 0xffffffff) >> 0x10) ^ (ushort)((result ^ 0xffffffff) & 0xffff));
        }
    }
}
