/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2020 by Kuxii
*/

namespace LowNet.Data
{
    /// <summary>
    /// Server Logsettings
    /// </summary>
    public enum Logsettings
    {
        /// <summary>
        /// No Log
        /// </summary>
        Logging_None,
        /// <summary>
        /// Only Warnings
        /// </summary>
        Logging_Warning,
        /// <summary>
        /// Only Errors
        /// </summary>
        Logging_Error,
        /// <summary>
        /// Normal Logging Info,Warning,Error
        /// </summary>
        Logging_Normal,
        /// <summary>
        /// Only Debug Logging Loggt Info,Warnin,Error,Debug
        /// </summary>
        Logging_Debug
    }
}
