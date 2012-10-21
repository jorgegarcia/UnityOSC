//
//	  UnityOSC - Open Sound Control interface for the Unity3d game engine
//
//	  Copyright (c) 2012 Jorge Garcia Martin
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

