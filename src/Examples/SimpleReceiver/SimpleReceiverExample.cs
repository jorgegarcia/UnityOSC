using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class SimpleReceiverExample : MonoBehaviour {
	
	private OSCReciever reciever;

	public int port = 8338;
	
	// Use this for initialization
	void Start () {
		reciever = new OSCReciever();
		reciever.Open(port);
	}
	
	// Update is called once per frame
	void Update () {
		if(reciever.hasWaitingMessages()){
			OSCMessage msg = reciever.getNextMessage();
			Debug.Log(string.Format("message received: {0} {1}", msg.Address, DataToString(msg.Data)));
		}
	}
	
	private string DataToString(List<object> data)
	{
		string buffer = "";
		
		for(int i = 0; i < data.Count; i++)
		{
			buffer += data[i].ToString() + " ";
		}
		
		buffer += "\n";
		
		return buffer;
	}
}
