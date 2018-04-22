using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OSCReciever
{
    Queue<OSCMessage> _queue = new Queue<OSCMessage>();
    OSCServer _server;

    public void Open(int port)
    {
#if UNITY_EDITOR
        if(PlayerSettings.runInBackground == false)
        {
            Debug.LogWarning("Recommend PlayerSettings > runInBackground = true");
        } 
#endif
        if (_server != null)
        {
            _server.Close();
        }
        _server = new OSCServer(port);
        _server.SleepMilliseconds = 0;
        _server.PacketReceivedEvent += didRecievedEvent;
    }
    
    public void Close()
    {
        if (_server != null)
        {
            _server.Close();
            _server = null;
        }
    }
    
    void didRecievedEvent(OSCServer sender, OSCPacket packet)
    {
        lock (_queue)
        {
            if (packet.IsBundle())
            {
                var bundle = packet as OSCBundle;
                
                foreach(object obj in bundle.Data)
                {
                    OSCMessage msg = obj as OSCMessage;
                    _queue.Enqueue(msg);
                }
            }
            else
            {
                _queue.Enqueue(packet as OSCMessage);
            }
        }
    }
    
    public bool hasWaitingMessages()
    {
        lock (_queue)
        {
            return 0 < _queue.Count;
        }
    }
    
    public OSCMessage getNextMessage()
    {
        lock (_queue)
        {
            return _queue.Dequeue();
        }
    }
}