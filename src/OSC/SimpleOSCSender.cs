using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;

namespace UnityOSC{
	public class SimpleOSCSender {

		private OSCHandler oscHandler;
		private string _name;

		// <summary>
		// A Simple OSC Client for sending OSC message to a specified server.
		// </summary>
		// <param name=name> A name for the OSC client </param>
		// <param name=hostIP> The IP Address of the OSC Server </param>
		// <param name=port> The Port number for the OSC Server </param>
		public SimpleOSCSender(string name, string hostIP, int port){
			oscHandler = OSCHandler.Instance;
			_name = name;
		
			IPAddress address;
			if (IPAddress.TryParse (hostIP, out address)) {
				oscHandler.CreateClient (name, address, port);
			} else {
				throw new Exception ("Unable to create OSC Client: Invalid IP Address");
			}
		}

		// <summary>
		// Creates and sends an OSC Message to the defined OSC Server
		// </summary>
		// <param name=address> The OSC address to route the message </param>
		// <param name=values> A list of values to encode in the OSC message </param>
		public void SendMessage(string address, List<object> values){
			oscHandler.SendMessageToClient (_name, address, values);
		}
	}
}