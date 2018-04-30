//
//	  UnityOSC - Open Sound Control interface for the Unity3d game engine
//
//	  Copyright (c) 2012 Jorge Garcia Martin
//	  Last edit: Gerard Llorach 2nd August 2017
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


using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityOSC
{
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
}