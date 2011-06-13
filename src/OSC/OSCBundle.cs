//
//	  UnityOSC - Open Sound Control interface for the Unity3d game engine
//
//	  Copyright (c) 2011 Jorge Garcia <info@jorgegarciamartin.com>
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, see <http://www.gnu.org/licenses/>.
//
//	  This code include portions of the Bespoke OSC Library by Paul Varcholik
//

using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityOSC
{
	/// <summary>
	/// Models a Bundle of the OSC protocol.
	/// Derived from a OSC Packet over a OSC Stream.
	/// </summary>
	public sealed class OSCBundle : OSCPacket
	{
		
		#region Constructors
		public OSCBundle()
		{
			_address = BUNDLE;
		}
		
		public OSCBundle(long timestamp)
		{
			_address = BUNDLE;
			_timeStamp = timestamp;
		}
		#endregion
		
		#region Member Variables
		private const string BUNDLE = "#bundle";
		
		#endregion
		
		#region Properties
		#endregion
	
		#region Methods
		
		/// <summary>
		/// Specifies if the packet is an OSC bundle.
		/// </summary>
		override public bool IsBundle() { return true; }
		
		/// <summary>
		/// Packs a bundle to be transported over an OSC stream.
		/// </summary>
		override public void Pack()
		{
			// TODO: Pack bundle with timestamp in NTP format
			
			throw new NotImplementedException("OSCBundle.Pack() : Not implemented method.");
		}

		/// <summary>
		/// Unpacks an OSC bundle from a data stream.
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
		/// A <see cref="OSCBundle"/>
		/// </returns>
		public static OSCBundle Unpack(byte[] data, ref int start, ref int end)
		{
			string address = OSCPacket.UnpackValue<string>(data, ref start);
			Trace.Assert(address == BUNDLE);
			
			long timeStamp = OSCPacket.UnpackValue<long>(data, ref start);
			OSCBundle bundle = new OSCBundle(timeStamp);
			
			while(start < end)
			{
				int length = OSCPacket.UnpackValue<int>(data, ref start);
				int packetEnd = start + length;
				bundle.Append(OSCPacket.Unpack(data, ref start, packetEnd));
			}
			
			return bundle;
		}
		
		/// <summary>
		/// Appends an OSC message to a bundle.
		/// </summary>
		/// <param name="msgvalue">
		/// A <see cref="T"/>
		/// </param>
		public override void Append<T> (T msgvalue)
		{
			Trace.Assert(msgvalue is OSCMessage);
			_data.Add(msgvalue);
		}
		#endregion			
	}
}
