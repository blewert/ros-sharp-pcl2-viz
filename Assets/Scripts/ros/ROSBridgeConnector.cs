using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RosSharp;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;

using System.Threading;


public class ROSBridgeConnector
{
    /// <summary>
    /// The URI to connect with
    /// </summary>
    protected readonly string uri;

    /// <summary>
    /// Timeout in seconds
    /// </summary>
    protected const int CONNECTION_TIMEOUT = 3;

    /// <summary>
    /// The socket
    /// </summary>
    protected RosSocket socket;


    /// <summary>
    /// Other ROS connection stuff: the serialiser
    /// </summary>
    protected RosSocket.SerializerEnum serializer = RosSocket.SerializerEnum.Microsoft;

    /// <summary>
    /// The protocol to connect with
    /// </summary>
    protected Protocol rosProtocol = Protocol.WebSocketSharp;

    /// <summary>
    /// Is this connector connected?
    /// </summary>
    private ManualResetEvent isConnected;
    
    public ROSBridgeConnector(string uri)
    {
        //Set uri, call initialise
        this.uri = uri;
        this.Initialize();
    }

    public ROSBridgeConnector(ROSConnectionInfo connectionInfo)
    {
        //Set serialiser + protocol
        this.serializer = connectionInfo.serializer;
        this.rosProtocol = connectionInfo.rosProtocol;

        //Set uri, call initialise
        this.uri = connectionInfo.uri;
        this.Initialize();
    }

    private void Initialize()
    {
        //Set is connected to false initially
        this.isConnected = new ManualResetEvent(false);
    }

    public void Connect()
    {
        //Start the connection 
        new Thread(ConnectAndWait).Start();
    }

    protected void ConnectAndWait()
    {
        //Taken from rosconnector.cs:
        //..

        //Connect to ros
        socket = ConnectToRos(rosProtocol, uri, OnConnected, OnClosed, serializer);

        //Show error message
        if (!isConnected.WaitOne(CONNECTION_TIMEOUT * 1000))
            Debug.LogWarning("Failed to connect to RosBridge at: " + uri);
    }

    public static RosSocket ConnectToRos(Protocol protocolType, string serverUrl, System.EventHandler onConnected = null, System.EventHandler onClosed = null, RosSocket.SerializerEnum serializer = RosSocket.SerializerEnum.Microsoft)
    {
        //Taken from rosconnector.cs
        //..

        //Find protocol
        IProtocol protocol = ProtocolInitializer.GetProtocol(protocolType, serverUrl);

        //Subscribe to events
        protocol.OnConnected += onConnected;
        protocol.OnClosed += onClosed;

        //Return a new socket
        return new RosSocket(protocol, serializer);
    }

    public void Close()
    {
        //Close the socket
        socket.Close();
    }

    private void OnConnected(object sender, System.EventArgs e)
    {
        isConnected.Set();
        Debug.Log("Connected to RosBridge: " + uri);
    }

    private void OnClosed(object sender, System.EventArgs e)
    {
        isConnected.Reset();
        Debug.Log("Disconnected from RosBridge: " + uri);
    }
}