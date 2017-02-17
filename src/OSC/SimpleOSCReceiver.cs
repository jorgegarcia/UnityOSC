using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityOSC;

namespace UnityOSC{
	public class SimpleOSCReceiver {

		public delegate void OscCallback(OSCMessage messsage);

		private OSCHandler oscHandler;
		private Dictionary<string, OscCallback> callbacks;

		private IEnumerator messageLoop;
		private float messageWait = .0333f;


		// <summary>
		// A Simple OSC Server for recieving and processing OSC messages.
		// </summary>
		// <param name=name> A name for the OSC Server </param>
		// <param name=port> The Port number for the OSC Server </param>
		public SimpleOSCReceiver(string name, int port){
			oscHandler = OSCHandler.Instance;
			oscHandler.CreateServer (name, port);
			callbacks = new Dictionary<string, OscCallback>();

			messageLoop = MessageLoop ();
		}


		// <summary>
		// Used to register callback methods for routing incoming messages based on the OSC Address.
		// Does not currently support full RegEx pattern matching.  Only checks for exact OSC Addresses.
		// </summary>
		// <param name=oscAddress> The OSC Address being routed  </param>
		// <param name=function> 
		// The callback function to route the OSC Message with the given address.
		// The function must take an OSCMessage as an argument.
		// </param>
		public void OnMessageReceived(string oscAddress, OscCallback function)
		{
			callbacks.Add (oscAddress, function);
		}


		public IEnumerator GetEnumerator(){
			return messageLoop;
		}

		IEnumerator MessageLoop(){
			while (true) {
				yield return new WaitForSecondsRealtime (messageWait);
			
				OSCHandler.Instance.UpdateLogs();
				Dictionary<string, ServerLog> servers = OSCHandler.Instance.Servers;

				foreach( KeyValuePair<string, ServerLog> item in servers )
				{
					if (item.Value.log.Count > 0) {   			// If we have received at least one packet
						int lastPacketIndex = item.Value.packets.Count - 1;
						string address = item.Value.packets [lastPacketIndex].Address;

						try{
							OscCallback f = callbacks [address];

							if (f != null) {
								UnityEngine.Debug.Log ("OSC Recieved - Running Function for " + item.Value.packets [lastPacketIndex].Address);
								OSCMessage mess = new OSCMessage (address);

								foreach (object d in item.Value.packets [lastPacketIndex].Data){
									mess.Append(d);
								}

								f (mess);
							}
						}
						catch(KeyNotFoundException e){
							UnityEngine.Debug.Log ("Error: Key Not Found in Dictionary: " + address + "\n" + e);
						}
					}
				}
			}
		}
	}
}