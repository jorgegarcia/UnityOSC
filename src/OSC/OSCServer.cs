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
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace UnityOSC
{
	/// <summary>
	/// Receives incoming OSC messages
	/// </summary>
	public class OSCServer
	{
		#region Constructors
		public OSCServer (int localPort)
		{
			_localPort = localPort;
			Connect();
		}
		#endregion
		
		#region Member Variables
		private UdpClient _udpClient;
		private int _localPort;
		private Thread _receiverThread;
		private OSCPacket _lastReceivedPacket;
		#endregion
		
		#region Properties
		public UdpClient UDPClient
		{
			get
			{
				return _udpClient;
			}
			set
			{
				_udpClient = value;
			}
		}
		
		public int LocalPort
		{
			get
			{
				return _localPort;
			}
			set
			{
				_localPort = value;
			}
		}
		
		public OSCPacket LastReceivedPacket
		{
			get
			{
				return _lastReceivedPacket;
			}
		}
		#endregion
	
		#region Methods
		
		/// <summary>
		/// Opens the server at the given port and starts the listener thread.
		/// </summary>
		public void Connect()
		{
			if(this._udpClient != null) Close();
			
			try
			{
				_udpClient = new UdpClient(_localPort);
				_receiverThread = new Thread(new ThreadStart(this.ReceivePool));
				_receiverThread.Start();
			}
			catch
			{
				throw new Exception(String.Format("Can't create server at port {0}", _localPort));
			}
		}
		
		/// <summary>
		/// Closes the server and terminates its listener thread.
		/// </summary>
		public void Close()
		{
			if(_receiverThread !=null) _receiverThread.Abort();
			_receiverThread = null;
			_udpClient.Close();
			_udpClient = null;
		}
		
		/// <summary>
		/// Receives and unpacks an OSC packet.
		/// </summary>
		/// <returns>
		/// A <see cref="OSCPacket"/>
		/// </returns>
		private OSCPacket Receive()
		{
			IPEndPoint ip = null;
			
			try
			{
				byte[] bytes = _udpClient.Receive(ref ip);

				if(bytes != null && bytes.Length > 0)
				{
					return OSCPacket.Unpack(bytes);
				}
			}
			catch
			{
				throw new Exception(String.Format("Can't unpack upcoming OSC data at port {0}", _localPort));
			}
			
			return null;
		}
		
		/// <summary>
		/// Thread pool that receives upcoming messages.
		/// </summary>
		private void ReceivePool()
		{
			while(true)
			{
				_lastReceivedPacket = Receive();
				_lastReceivedPacket.TimeStamp = long.Parse(String.Concat(DateTime.Now.Ticks));
			}
		}
		#endregion
	}
}

