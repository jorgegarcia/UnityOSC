using UnityEngine;
using System.Collections;
using UnityOSC;

public class TestOsc : MonoBehaviour {

	public string inName = "OSC Receiver";
	public int port = 50505;

	private SimpleOSCReceiver oscIn;

	// Use this for initialization
	void Start () {
		SimpleOSCReceiver oscIn = new SimpleOSCReceiver(inName, port);
		oscIn.OnMessageReceived ("/vrmin/vol", TestCallback);
		StartCoroutine (oscIn.GetEnumerator());
	}

	public void TestCallback(OSCMessage message){
		UnityEngine.Debug.Log ("Funciotn Running\nOSC Address is:  " + message.Address);

		foreach (float d in message.Data) {
			UnityEngine.Debug.Log ("\t" + d);

			System.Type type = d.GetType ();

			switch (type.Name) {
			case "Int32":
				UnityEngine.Debug.Log ("Int32");
				break;

			case "Int64":
				UnityEngine.Debug.Log ("Int64");
				break;

			case "Single":
				UnityEngine.Debug.Log ("Float");
				break;

			case "Double":
				UnityEngine.Debug.Log ("Double");
				break;

			case "String":
				UnityEngine.Debug.Log ("String");
				break;

			default:
				throw new System.Exception ("Unsupported data type.");
			}

		}
	}
}
