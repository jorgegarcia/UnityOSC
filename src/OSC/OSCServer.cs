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
using System.Threading;
using System.Collections.Generic;

namespace UnityOSC
{
    public delegate void PacketReceivedEventHandler(OSCServer sender, OSCPacket packet);

	/// <summary>
	/// Receives incoming OSC messages
	/// </summary>
	public class OSCServer
    {
        #region Delegates
        public event PacketReceivedEventHandler PacketReceivedEvent;
        #endregion

        #region Constructors
        public OSCServer (int localPort)
		{
            PacketReceivedEvent += delegate(OSCServer s, OSCPacket p) { };

			_localPort = localPort;
			Connect();
		}
		#endregion
		
		#region Member Variables
		private UdpClient _udpClient;
		private int _localPort;
		private Thread _receiverThread;
		private OSCPacket _lastReceivedPacket;
		private int _sleepMilliseconds = 10;
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
			set{
				_lastReceivedPacket = value;
			}
		}

		/// <summary>
		/// "Osc Receive Loop" sleep duration per message.
		/// </summary>
		/// <value>The sleep milliseconds.</value>
		public int SleepMilliseconds
		{
			get
			{
				return _sleepMilliseconds;
			}
			set
			{
				_sleepMilliseconds = value;
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
			catch(Exception e)
			{
				throw e;
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
        /// A <see cref="OSCPacket"/>
		/// </summary>
		private void Receive()
		{
			IPEndPoint ip = null;
			
			try
			{
				byte[] bytes = _udpClient.Receive(ref ip);

				if(bytes != null && bytes.Length > 0)
				{
                    OSCPacket packet = OSCPacket.Unpack(bytes);

                    _lastReceivedPacket = packet;

                    PacketReceivedEvent(this, _lastReceivedPacket);	
				}
			}
			catch{
				throw new Exception(String.Format("Can't create server at port {0}", _localPort));
  			}
		}
		
		/// <summary>
		/// Thread pool that receives upcoming messages.
		/// </summary>
		private void ReceivePool()
		{
			while( true )
			{
				Receive();
				
				Thread.Sleep(_sleepMilliseconds);
			}
		}
		#endregion
	}
}

