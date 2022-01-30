/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace LowNet.Data
{
    /// <summary>
    /// Object Types
    /// </summary>
    public enum ObjectType
    {
        /// <summary>
        /// Player Object was can use Player
        /// </summary>
        PlayerObject,
        /// <summary>
        /// Server Object was can not Controll Player
        /// </summary>
        ServerObject,
        /// <summary>
        /// Map Object was is Mooving
        /// </summary>
        MapObject,
        /// <summary>
        /// Platform
        /// </summary>
        Platform,
        /// <summary>
        /// Playerobj
        /// </summary>
        Player,
        /// <summary>
        /// Vehicleobj
        /// </summary>
        Vehicle
    }
}