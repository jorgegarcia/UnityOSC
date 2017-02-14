using UnityEngine;
using System.Collections;
using UnityOSC;

namespace UnityOSC{
	public class SimpleOSCReceiver : MonoBehaviour {

		public delegate void OscCallback(OSCMessage messsage);

		private OSCHandler oscHandler;

		public SimpleOSCReceiver(string name, int port){
			oscHandler = OSCHandler.Instance;
			oscHandler.CreateServer (name, port);
		}

		public void OnMessageReceived(string oscAddress, OscCallback function)
		{
			// WHAT TO DO HERE????
		}
	}
}