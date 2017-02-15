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

		public SimpleOSCReceiver(string name, int port){
			oscHandler = OSCHandler.Instance;
			oscHandler.CreateServer (name, port);
			callbacks = new Dictionary<string, OscCallback>();

			messageLoop = MessageLoop ();
		}

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