/*  __                  _   __     __ 
   / /   ____ _      __/ | / /__  / /_
  / /   / __ \ | /| / /  |/ / _ \/ __/
 / /___/ /_/ / |/ |/ / /|  /  __/ /_  
/_____/\____/|__/|__/_/ |_/\___/\__/  
Simple Unity3D Solution ©2022 by Kuxii
*/
using LowNet.Packets;
using LowNet.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace LowNet.Data
{
    /// <summary>
    /// Store is the Packet,
    /// Read Data from here.
    /// Or Add Data to Store and Send it
    /// </summary>
    public class Store : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        /// <summary>
        /// Create empty Store
        /// </summary>
        public Store()
        {
            buffer = new List<byte>();
            readPos = 0;
        }

        /// <summary>
        /// Create Store with Packetenum
        /// </summary>
        /// <param name="value"></param>
        public Store(LowNetpacketOrder value)
        {
            buffer = new List<byte>();
            readPos = 0;
            PushInt((int)value);
            PushInt((int)ClassUtils.CalculateChecksum(((LowNetpacketOrder)value).ToString()));
        }

        /// <summary>
        /// Create Store with Id
        /// </summary>
        /// <param name="value"></param>
        public Store(int value)
        {
            buffer = new List<byte>();
            readPos = 0;
            PushInt(value);
            PushInt((int)ClassUtils.CalculateChecksum(((LowNetpacketOrder)value).ToString()));
        }

        /// <summary>
        /// Create store from Byte Array
        /// </summary>
        /// <param name="_data"></param>
        public Store(byte[] _data)
        {
            buffer = new List<byte>();
            readPos = 0;
            SetBytes(_data);
        }

        #region Functions
        /// <summary>
        /// Set Store Bytes
        /// </summary>
        /// <param name="_data"></param>
        public void SetBytes(byte[] _data)
        {
            PushBytes(_data);
            readableBuffer = buffer.ToArray();
        }

        /// <summary>
        /// Write Packet Store Lenght
        /// </summary>
        public void WriteLength() => buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));

        /// <summary>
        /// Insert int on First from Store
        /// </summary>
        /// <param name="_value"></param>
        public void InsertInt(int _value) => buffer.InsertRange(0, BitConverter.GetBytes(_value));

        /// <summary>
        /// Get Store as ByteArray
        /// </summary>
        public byte[] ToArray { get { return readableBuffer = buffer.ToArray(); } }

        /// <summary>
        /// Get the Lenght of the Store
        /// </summary>
        public int Length { get { return buffer.Count; } }

        /// <summary>
        /// Get the Unread Lenght of the Store
        /// </summary>
        public int UnreadLength { get { return Length - readPos; } }

        /// <summary>
        /// Reset Store
        /// </summary>
        /// <param name="_shouldReset"></param>
        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                buffer.Clear();
                readableBuffer = null;
                readPos = 0;
            }
            else
            {
                readPos -= 4;
            }
        }
        #endregion

        #region Write Data
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushByte(byte _value)
        {
            buffer.Add(_value);
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushBytes(byte[] _value)
        {
            buffer.AddRange(_value);
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushShort(short _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushInt(int _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushLong(long _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushDouble(double _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushFloat(float _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushBool(bool _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushAscii(string _value)
        {
            PushInt(_value.Length);
            buffer.AddRange(Encoding.ASCII.GetBytes(_value));
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushVector3(Vector3 _value)
        {
            PushFloat(_value.x);
            PushFloat(_value.y);
            PushFloat(_value.z);
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushVector2(Vector2 _value)
        {
            PushFloat(_value.x);
            PushFloat(_value.y);
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushQuaternion(Quaternion _value)
        {
            PushFloat(_value.x);
            PushFloat(_value.y);
            PushFloat(_value.z);
            PushFloat(_value.w);
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushColor(Color _value)
        {
            PushFloat(_value.r);
            PushFloat(_value.g);
            PushFloat(_value.b);
            PushFloat(_value.a);
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushColor32(Color32 _value)
        {
            PushByte(_value.r);
            PushByte(_value.g);
            PushByte(_value.b);
            PushByte(_value.a);
        }
        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushObject(object _value)
        {
            byte[] obj = RawSerializeEx(_value);
            PushInt(obj.Length);
            PushBytes(obj);
        }
        #endregion

        #region Read Data
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public byte PopByte(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte _value = readableBuffer[readPos];
                if (_moveReadPos)
                {
                    readPos += 1;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_length"></param>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public byte[] PopBytes(int _length, bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] _value = buffer.GetRange(readPos, _length).ToArray();
                if (_moveReadPos)
                {
                    readPos += _length;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public short PopShort(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                short _value = BitConverter.ToInt16(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 2;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public int PopInt(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                int _value = BitConverter.ToInt32(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 4;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public long PopLong(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                long _value = BitConverter.ToInt64(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 8;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public double PopDouble(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                double _value = BitConverter.ToDouble(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 8;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'double'!");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public float PopFloat(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float _value = BitConverter.ToSingle(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 4;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public bool PopBool(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool _value = BitConverter.ToBoolean(readableBuffer, readPos);
                if (_moveReadPos)
                {
                    readPos += 1;
                }
                return _value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public string PopAscii(bool _moveReadPos = true)
        {
            try
            {
                int _length = PopInt();
                string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length);
                if (_moveReadPos && _value.Length > 0)
                {
                    readPos += _length;
                }
                return _value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Vector3 PopVector3(bool _moveReadPos = true)
        {
            return new Vector3(PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos));
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Vector3 PopVector2(bool _moveReadPos = true)
        {
            return new Vector2(PopFloat(_moveReadPos), PopFloat(_moveReadPos));
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Quaternion PopQuaternion(bool _moveReadPos = true)
        {
            return new Quaternion(PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos));
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Color PopColor(bool _moveReadPos = true)
        {
            return new Color(PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos));
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Color PopColor32(bool _moveReadPos = true)
        {
            return new Color32(PopByte(_moveReadPos), PopByte(_moveReadPos), PopByte(_moveReadPos), PopByte(_moveReadPos));
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <returns></returns>
        public object PopObject(Type obj)
        {
            try
            {
                int _length = PopInt();
                byte[] rawObj = PopBytes(_length);

                return RawDeserializeEx(rawObj, obj);
            }
            catch
            {
                throw new Exception("Could not read value of type 'object'!");
            }
        }
        #endregion

        private bool disposed = false;
        /// <summary>
        /// Dispose Store
        /// </summary>
        /// <param name="_disposing"></param>
        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }
        /// <summary>
        /// Sispose Store
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Object Serializer
        /// </summary>
        /// <param name="rawdatas"></param>
        /// <param name="anytype"></param>
        /// <returns></returns>
        public static object RawDeserializeEx(byte[] rawdatas, Type anytype)
        {
            int rawsize = Marshal.SizeOf(anytype);
            if (rawsize > rawdatas.Length)
                return null;
            GCHandle handle = GCHandle.Alloc(rawdatas, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            object retobj = Marshal.PtrToStructure(buffer, anytype);
            handle.Free();
            return retobj;
        }

        /// <summary>
        /// Object Serializer
        /// </summary>
        /// <param name="anything"></param>
        /// <returns></returns>
        public static byte[] RawSerializeEx(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            byte[] rawdatas = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdatas, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(anything, buffer, false);
            handle.Free();
            return rawdatas;
        }
    }
}