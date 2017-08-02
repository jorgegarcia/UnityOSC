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
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace UnityOSC
{
	public sealed class OSCMessage : OSCPacket
	{
		#region Constructors
		public OSCMessage (string address)
		{
			_typeTag = DEFAULT.ToString();
			this.Address = address;
		}
		
		public OSCMessage (string address, object msgvalue)
		{
			_typeTag = DEFAULT.ToString();
			this.Address = address;
			Append(msgvalue);
		}
		#endregion
		
		#region Member Variables
		private const char INTEGER = 'i';
		private const char FLOAT   = 'f';
		private const char LONG	   = 'h';
		private const char DOUBLE  = 'd';
		private const char STRING  = 's';
		private const char BYTE    = 'b';
		private const char DEFAULT = ',';
		
		private string _typeTag;
		
		#endregion
		
		#region Properties
		#endregion
	
		#region Methods

		/// <summary>
		/// Specifies if the message is an OSC bundle.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		override public bool IsBundle() { return false; }
		
		/// <summary>
		/// Packs the OSC message to binary data.
		/// </summary>
		override public void Pack() 
		{	
			List<byte> data = new List<byte>();

			data.AddRange(OSCPacket.PackValue(_address));
			OSCPacket.PadNull(data);

			data.AddRange(OSCPacket.PackValue(_typeTag));
			OSCPacket.PadNull(data);

			foreach (object value in _data)
			{
				data.AddRange(OSCPacket.PackValue(value));
				if (value is string || value is byte[])
				{
					OSCPacket.PadNull(data);
				}
			}

			this._binaryData = data.ToArray();
		}
		
		/// <summary>
		/// Unpacks an OSC message.
		/// </summary>
		/// <param name="data">
		/// A <see cref="System.Byte[]"/>
		/// </param>
		/// <param name="start">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="OSCMessage"/>
		/// </returns>
		public static OSCMessage Unpack(byte[] data, ref int start)
		{
			string address = OSCPacket.UnpackValue<string>(data, ref start);
			OSCMessage message = new OSCMessage(address);

			char[] tags = OSCPacket.UnpackValue<string>(data, ref start).ToCharArray();
			foreach (char tag in tags)
			{
				object value;
				switch (tag)
				{
					case DEFAULT:
						continue;

					case INTEGER:
						value = OSCPacket.UnpackValue<int>(data, ref start);
						break;

					case LONG:
						value = OSCPacket.UnpackValue<long>(data, ref start);
						break;

					case FLOAT:
						value = OSCPacket.UnpackValue<float>(data, ref start);
						break;

					case DOUBLE:
						value = OSCPacket.UnpackValue<double>(data, ref start);
						break;

					case STRING:
						value = OSCPacket.UnpackValue<string>(data, ref start);
						break;

					case BYTE:
						value = OSCPacket.UnpackValue<byte[]>(data, ref start);
						break;

					default:
						Console.WriteLine("Unknown tag: " + tag);
						continue;
				}

				message.Append(value);
			}

			if(message.TimeStamp == 0)
			{
				message.TimeStamp = DateTime.Now.Ticks;
			}

			return message;
		}
		
		/// <summary>
		/// Appends a value to an OSC message.
		/// </summary>
		/// <param name="value">
		/// A <see cref="T"/>
		/// </param>
		public override void Append<T> (T value)
		{
			Type type = value.GetType();
			char typeTag = DEFAULT;

			switch (type.Name)
			{
				case "Int32":
					typeTag = INTEGER;
					break;

				case "Int64":
					typeTag = LONG;
					break;

				case "Single":
					typeTag = FLOAT;
					break;

				case "Double":
					typeTag = DOUBLE;
					break;

				case "String":
					typeTag = STRING;
					break;

				case "Byte[]":
					typeTag = BYTE;
					break;

				default:
					throw new Exception("Unsupported data type.");
			}

			_typeTag += typeTag;
			_data.Add(value);
		}
		#endregion
	}
}