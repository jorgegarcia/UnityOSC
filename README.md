## UnityOSC v1.2.

Open Sound Control classes and API for the Unity 3d game engine

Based on Bespoke Open Sound Control Library by Paul Varcholik (pvarchol@bespokesoftware.org).
Licensed under MIT license.

## How to use

### Install 

Copy the src/Editor folder contents to the corresponding Editor/ folder of your Unity project. The rest can go to your e.g. Assets/ folder of the same project.

### Usage

There are two different approaches to use the plugin:

#### OSCHandler

The first and original approach works by modifying and initializing the `OSCHandler`. It is a Singleton handling your client & server connections and sending & receiving OSC data. Additionally, all incoming and outgoing messages are logged and can be viewed in an Editor Window.

Check the documentation below for more detailed instructions. 

#### OSCReceiver

The second approach is a simple `OSCReceiver` which only supports receiving OSC data but with the benefits of reduced complexity and a thread-safe `OSCMessage` queue. 

After opening a server connection by `OSCReceiver.Open(int port)` you can easily receive new OSCMessages via `OSCReceiver.getNextMessage()`. Always make sure there are OSCMessages available by using `OSCReceiver.hasWaitingMessages()` before trying to receive new ones.

## Documentation and examples of usage

docs/doxygen/html/index.html

docs/UnityOSC_manual.pdf

docs/UnityOSC & TouchOSC Integration.pdf 

Please head to the tests/ folder for examples of usage and a TouchOSC test Unity project.

## TODO

07.11 Change string concatenations to C# string builders.
