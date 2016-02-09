using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityOSC;
using System.Net;


public class OSCWrapper : MonoBehaviour {

	public string 	clientName = "SoundEngine";
	public string 	clientIPAdress = "127.0.0.1";
	public int 		clientPort = 7778;
	public float 	packetFrequency = 30f;		// In Hz


	public static OSCWrapper Instance;

	private Dictionary<string,object[]> _messages = new Dictionary<string, object[]>();

	public void Start()
	{
		OSCHandler.Instance.Init ();
		OSCHandler.Instance.CreateClient(clientName, IPAddress.Parse(clientIPAdress), clientPort);

		InvokeRepeating ("sendBundle", 1/packetFrequency, 1/packetFrequency);
	}

	public void Awake()
	{
		Instance = this;
	}


	// be aware that if, in the timeframe between two calls of sendbundle, you send two messages with the same adress,
	// only the last one will be sent. 
	public void send(string address, params object[] contenu)
	{
		_messages [address] = contenu;
	}

	public void sendBundle()
	{
		OSCBundle bundle = new OSCBundle ();
		foreach (KeyValuePair<string,object[]> p in _messages)
		{
			OSCMessage message = new OSCMessage(p.Key);
			foreach (object value in p.Value)
			{
				message.Append (value);
			}
			bundle.Append (message);
		}

		 OSCHandler.Instance.SendBundleToClient (clientName, bundle);

		_messages.Clear ();
	}
}
