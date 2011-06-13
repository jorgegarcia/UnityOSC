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

namespace UnityOSC
{
	/// <summary>
	/// Dispatches OSC messages to the specified destination address and port.
	/// </summary>
	
	public class OSCClient
	{
		#region Constructors
		public OSCClient (IPAddress address, int port)
		{
			_ipAddress = address;
			_port = port;
			Connect();
		}
		#endregion
		
		#region Member Variables
		private IPAddress _ipAddress;
		private int _port;
		private UdpClient _udpClient;
		#endregion
		
		#region Properties
		public IPAddress ClientIPAddress
		{
			get
			{
				return _ipAddress;
			}
		}
		
		public int Port
		{
			get
			{
				return _port;
			}
		}
		#endregion
	
		#region Methods
		/// <summary>
		/// Connects the client to a given remote address and port.
		/// </summary>
		public void Connect()
		{
			if(_udpClient != null) Close();
			_udpClient = new UdpClient();
			try
			{
				_udpClient.Connect(_ipAddress, _port);	
			}
			catch
			{
				throw new Exception(String.Format("Can't create client at IP address {0} and port {1}.", _ipAddress,_port));
			}
		}
		
		/// <summary>
		/// Closes the client.
		/// </summary>
		public void Close()
		{
			_udpClient.Close();
			_udpClient = null;
		}
		
		/// <summary>
		/// Sends an OSC packet to the defined destination and address of the client.
		/// </summary>
		/// <param name="packet">
		/// A <see cref="OSCPacket"/>
		/// </param>
		public void Send(OSCPacket packet)
		{
			byte[] data = packet.BinaryData;
			try 
			{
				_udpClient.Send(data, data.Length);
			}
			catch
			{
				throw new Exception(String.Format("Can't send OSC packet to client {0} : {1}", _ipAddress, _port));
			}
		}
		#endregion
	}
}

