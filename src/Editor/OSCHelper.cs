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

using UnityEngine;
using UnityEditor;
using UnityOSC;

using System;
using System.Collections.Generic;

/// <summary>
/// Helper to monitor incoming at outgoing OSC messages from the Unity Editor.
/// You should have this script placed at the /Editor folder.
/// Show the panel helper by selecting "Window->OSC Helper" from the Unity menu.
/// </summary>
public class OSCHelper : EditorWindow
{
	#region Member variables
	private string _status = "";
	private string _selected = "none";
	private List<string> _output = new List<string>();
	private int _portselected = 0;
	
	private Dictionary<string, ClientLog> _clients = new Dictionary<string, ClientLog>();
	private Dictionary<string, ServerLog> _servers = new Dictionary<string, ServerLog>();
	#endregion
	
	/// <summary>
	/// Initializes the OSC Helper and creates an entry in the Unity menu.
	/// </summary>
	[MenuItem("Window/OSC Helper")]
	static void Init ()
	{
		OSCHelper window = (OSCHelper)EditorWindow.GetWindow (typeof(OSCHelper));
		window.Show();
	}
	
	/// <summary>
	/// Executes OnGUI in the panel within the Unity Editor
	/// </summary>
	void OnGUI ()
	{	
		if(EditorApplication.isPlaying)
		{
			_status = "";
			GUILayout.Label(_status, EditorStyles.boldLabel);
			GUILayout.Label(String.Concat("SELECTED: ", _selected));
			
			_clients = OSCHandler.Instance.Clients;//Get the clients
			_servers = OSCHandler.Instance.Servers;//Get the servers
			
			foreach(KeyValuePair<string, ClientLog> pair in _clients)
			{
				if(GUILayout.Button(String.Format("Client '{0}' port: {1}", pair.Key, pair.Value.client.Port)))
				{
					_selected = pair.Key;
					_portselected = pair.Value.client.Port;
				}
			}
			
			foreach(KeyValuePair<string, ServerLog> pair in _servers)
			{
				if(GUILayout.Button(String.Format("Server '{0}' port: {1}", pair.Key, pair.Value.server.LocalPort)))
				{
					_selected = pair.Key;
					_portselected = pair.Value.server.LocalPort;
				}
			}
			
			GUILayout.TextArea(FromListToString(_output));
		}
		else
		{
			_status = "\n Enter the play mode in the Editor to see \n running clients and servers";
			GUILayout.Label(_status, EditorStyles.boldLabel);
		}
	}
	
	/// <summary>
	/// Updates the logs of the running clients and servers.
	/// </summary>
	void Update()
	{
		if(EditorApplication.isPlaying)
		{
			OSCHandler.Instance.UpdateLogs();
			
			if(_clients.ContainsKey(_selected) && _clients[_selected].client.Port == _portselected)
			{
				_output = _clients[_selected].log;
			}
			else if(_servers.ContainsKey(_selected) && _servers[_selected].server.LocalPort == _portselected)
			{
				_output = _servers[_selected].log;
			}
			
			Repaint();
		}
	}
	
	/// <summary>
	/// Formats a collection of strings to a single concatenated string.
	/// </summary>
	/// <param name="input">
	/// A <see cref="List<System.String>"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	private string FromListToString(List<string> input)
	{
		string output = "";
		
		foreach(string value in input)
		{
			output += value;
		}
		
		return output;	
	}
}