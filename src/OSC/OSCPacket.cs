//
//	  UnityOSC - Open Sound Control interface for the Unity3d game engine
//
//	  Copyright (c) 2012 Jorge Garcia Martin
//	  Last edit: Gerard Llorach 2nd August 2017
//
// 	  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// 	  documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// 	  the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
// 	  and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// 	  The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// 	  of the Software.
//
// 	  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// 	  TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// 	  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// 	  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// 	  IN THE SOFTWARE.
//

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace UnityOSC
{
	/// <summary>
	/// Models a OSC Packet over an OSC stream.
	/// </summary>
	abstract public class OSCPacket
	{
		#region Member Variables
		protected List<object> _data;
		protected byte[] _binaryData;
		protected string _address;
		protected long _timeStamp;
        public OSCServer server;
		#endregion
		
		#region Properties
		public string Address
		{
			get
			{
				return _address;
			}
			set
			{
				Trace.Assert(string.IsNullOrEmpty(value) == false);
				_address = value;
			}
		}
				
		public List<object> Data
		{
			get
			{
				return _data;
			}
		}
		
		public byte[] BinaryData
		{
			get
			{
				Pack();
				return _binaryData;
			}
		}
		
		public long TimeStamp
		{
			get
			{
				return _timeStamp;
			}
			set
			{
				_timeStamp = value;
			}
		}
		#endregion
	
		#region Methods
		abstract public bool IsBundle(); 
		abstract public void Pack();
		abstract public void Append<T>(T msgvalue);
		
		/// <summary>
		/// OSC Packet initialization.
		/// </summary>
		public OSCPacket ()
		{
			this._data = new List<object>();
		}
		
		/// <summary>
		/// Swap endianess given a data set.
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.Byte[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte[]"/>
		/// </returns>
		protected static byte[] SwapEndian(byte[] data)
		{
			byte[] swapped = new byte[data.Length];
			for(int i = data.Length - 1, j = 0 ; i >= 0 ; i--, j++)
			{
				swapped[j] = data[i];
			}
			return swapped;
		}
				
		/// <summary>
		/// Packs a value in a byte stream. Accepted types: Int32, Int64, Single, Double, String and Byte[].
		/// </summary>
		/// <param name="value">
		/// A <see cref="T"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Byte[]"/>
		/// </returns>
		protected static byte[] PackValue<T>(T value)
		{
			object valueObject = value;
			Type type = value.GetType();
			byte[] data = null;

			switch (type.Name)
			{
				case "Int32":
					data = BitConverter.GetBytes((int)valueObject);
					if (BitConverter.IsLittleEndian) data = SwapEndian(data);
					break;

				case "Int64":
					data = BitConverter.GetBytes((long)valueObject);
					if (BitConverter.IsLittleEndian) data = SwapEndian(data);
					break;

				case "Single":
					data = BitConverter.GetBytes((float)valueObject);
					if (BitConverter.IsLittleEndian) data = SwapEndian(data);
					break;

				case "Double":
					data = BitConverter.GetBytes((double)valueObject);
					if (BitConverter.IsLittleEndian) data = SwapEndian(data);
					break;

				case "String":
					data = Encoding.ASCII.GetBytes((string)valueObject);
					break;

				case "Byte[]":
					byte[] valueData = ((byte[])valueObject);
					List<byte> bytes = new List<byte>();
					bytes.AddRange(PackValue(valueData.Length));
					bytes.AddRange(valueData);
					data = bytes.ToArray();
					break;

				default:
					throw new Exception("Unsupported data type.");
			}
			return data;
		}
		
		/// <summary>
		/// Unpacks a value from a byte stream. Accepted types: Int32, Int64, Single, Double, String and Byte[].
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.Byte[]"/>
		/// </param>
		/// <param name="start">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="T"/>
		/// </returns>
		protected static T UnpackValue<T>(byte[] data, ref int start)
		{
			object msgvalue; //msgvalue is casted and returned by the function
			Type type = typeof(T);
			byte[] buffername;

			if (type.Name == "String")
			{
				int count = 0;
				for (int index = start; data[index] != 0; index++)	count++;

				msgvalue = Encoding.ASCII.GetString(data, start, count);
				start += count + 1;
				start = ((start + 3) / 4) * 4;
			}
			else if (type.Name == "Byte[]")
			{
                int length = UnpackValue<int>(data, ref start);
                byte[] buffer = new byte[length];
                Array.Copy(data, start, buffer, 0, buffer.Length);
                start += buffer.Length;
                start = ((start + 3) / 4) * 4;

                msgvalue = buffer;
			}
			else
			{
				switch (type.Name)
				{
					case "Int32":
					case "Single"://this also serves for float numbers
						buffername = new byte[4];
						break;

					case "Int64":
					case "Double":
						buffername = new byte[8];
						break;

					default:
						throw new Exception("Unsupported data type.");
				}

				Array.Copy(data, start, buffername, 0, buffername.Length);
				start += buffername.Length;

				if (BitConverter.IsLittleEndian)
				{
					buffername = SwapEndian(buffername);
				}

				switch (type.Name)
				{
					case "Int32":
						msgvalue = BitConverter.ToInt32(buffername, 0);
						break;

					case "Int64":
						msgvalue = BitConverter.ToInt64(buffername, 0);
						break;

					case "Single":
						msgvalue = BitConverter.ToSingle(buffername, 0);
						break;

					case "Double":
						msgvalue = BitConverter.ToDouble(buffername, 0);
						break;

					default:
						throw new Exception("Unsupported data type.");
				}
			}

			return (T)msgvalue;
		}
		
		/// <summary>
		/// Unpacks an array of binary data.
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.Byte[]"/>
		/// </param>
		/// <returns>
		/// A <see cref="OSCPacket"/>
		/// </returns>
		public static OSCPacket Unpack(byte[] data)
		{
			int start = 0;
			return Unpack(data, ref start, data.Length);
		}
		
		/// <summary>
		/// Unpacks an array of binary data given reference start and end pointers.
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.Byte[]"/>
		/// </param>
		/// <param name="start">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="end">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="OSCPacket"/>
		/// </returns>
		public static OSCPacket Unpack(byte[] data, ref int start, int end)
		{
            if (data[start] == '#')
            {
                return OSCBundle.Unpack(data, ref start, end);
            }

            else return OSCMessage.Unpack(data, ref start);
		}		
		
		/// <summary>
		/// Pads null a list of bytes.
		/// </summary>
		/// <param name="data">
		/// A <see cref="List<System.Byte>"/>
		/// </param>
		protected static void PadNull(List<byte> data)
		{
			byte nullvalue = 0;
			int pad = 4 - (data.Count % 4);
			for(int i = 0; i < pad; i++)
				data.Add(nullvalue);
		}
		
		#endregion
	}
}
