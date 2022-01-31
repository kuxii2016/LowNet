using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LowNet.Utils
{
    /// <summary>
    /// LowNet Packet
    /// </summary>
    public class Store : IDisposable
    {
        #region Packet Private Propertys
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;
        #endregion

        #region Store Create
        /// <summary>
        /// Create empty Store
        /// </summary>
        public Store()
        {
            buffer = new List<byte>();
            readPos = 0;
        }

        /// <summary>
        /// Create Store with Id
        /// </summary>
        /// <param name="_id"></param>
        public Store(int _id)
        {
            buffer = new List<byte>();
            readPos = 0;
            PushInt(_id);
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
        #endregion

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
        public void PushByte(byte _value)=>buffer.Add(_value);

        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushBytes(byte[] _value)=>buffer.AddRange(_value);

        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushShort(short _value)=>buffer.AddRange(BitConverter.GetBytes(_value));

        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushInt(int _value)=>buffer.AddRange(BitConverter.GetBytes(_value));

        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushLong(long _value)=>buffer.AddRange(BitConverter.GetBytes(_value));

        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushDouble(double _value)=>buffer.AddRange(BitConverter.GetBytes(_value));

        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushFloat(float _value)=>buffer.AddRange(BitConverter.GetBytes(_value));

        /// <summary>
        /// Add data to the package
        /// </summary>
        /// <param name="_value"></param>
        public void PushBool(bool _value)=>buffer.AddRange(BitConverter.GetBytes(_value));

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
                throw new Exception(@"LowNetStore::PopByte()=>file .\F:\Git\LowNet\Utils\Store.cs line:239");
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
                throw new Exception(@"LowNetStore::PopByte[]()=>file .\F:\Git\LowNet\Utils\Store.cs line:261");
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
                throw new Exception(@"LowNetStore::PopShort()=>file .\F:\Git\LowNet\Utils\Store.cs line:282");
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
                throw new Exception(@"LowNetStore::PopInt()=>file .\F:\Git\LowNet\Utils\Store.cs line:303");
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
                throw new Exception(@"LowNetStore::PopLong()=>file .\F:\Git\LowNet\Utils\Store.cs line:324");
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
                throw new Exception(@"LowNetStore::PopDouble()=>file .\F:\Git\LowNet\Utils\Store.cs line:345");
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
                throw new Exception(@"LowNetStore::PopFloat()=>file .\F:\Git\LowNet\Utils\Store.cs line:366");
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
                throw new Exception(@"LowNetStore::PopBool()=>file .\F:\Git\LowNet\Utils\Store.cs line:387");
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
                throw new Exception(@"LowNetStore::PopAscii()=>file .\F:\Git\LowNet\Utils\Store.cs line:409");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Vector3 PopVector3(bool _moveReadPos = true)
        {
            try
            {
                return new Vector3(PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos));
            }
            catch
            {
                throw new Exception(@"LowNetStore::PopVector3()=>file .\F:\Git\LowNet\Utils\Store.cs line:425");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Vector2 PopVector2(bool _moveReadPos = true)
        {
            try
            {
                return new Vector2(PopFloat(_moveReadPos), PopFloat(_moveReadPos));
            }
            catch
            {
                throw new Exception(@"LowNetStore::PopVector2()=>file .\F:\Git\LowNet\Utils\Store.cs line:441");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Quaternion PopQuaternion(bool _moveReadPos = true)
        {
            try
            {
                return new Quaternion(PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos));
            }
            catch
            {
                throw new Exception(@"LowNetStore::PopQuaternion()=>file .\F:\Git\LowNet\Utils\Store.cs line:457");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Color PopColor(bool _moveReadPos = true)
        {
            try
            {
                return new Color(PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos), PopFloat(_moveReadPos));
            }
            catch
            {
                throw new Exception(@"LowNetStore::PopColor()=>file .\F:\Git\LowNet\Utils\Store.cs line:473");
            }
        }
        /// <summary>
        /// Read data from packet
        /// </summary>
        /// <param name="_moveReadPos"></param>
        /// <returns></returns>
        public Color PopColor32(bool _moveReadPos = true)
        {
            try
            {
                return new Color32(PopByte(_moveReadPos), PopByte(_moveReadPos), PopByte(_moveReadPos), PopByte(_moveReadPos));
            }
            catch
            {
                throw new Exception(@"LowNetStore::PopColor32()=>file .\F:\Git\LowNet\Utils\Store.cs line:489");
            }
        }
        #endregion

        #region Disposing
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
        #endregion
    }
}