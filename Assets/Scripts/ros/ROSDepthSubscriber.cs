/**
 * @file       ROSDepthSubscriber.cs
 * @brief      A basic ROS bridge subscriber for PointCloud2 topic types
 *
 * @author     Benjamin Williams <trewelb@gmail.com>
 * @copyright  Copyright (c) University of Lincoln 2020
*/

using System.Collections;

using System.Collections.Generic;
using UnityEngine;

using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;

using stdMsgs = RosSharp.RosBridgeClient.MessageTypes.Std;
using sensorMsgs = RosSharp.RosBridgeClient.MessageTypes.Sensor;

[System.Serializable]
public class ROSConnectionInfo
{
    [Header("[uri info]")]

    /// <summary>
    /// The IP to connect to 
    /// </summary>
    public string ip = "127.0.0.1";

    /// <summary>
    /// The protocol (web socket by default)
    /// </summary>
    public string protocol = "ws";

    /// <summary>
    /// The port (9090 by default)
    /// </summary>
    public int port = 9090;

    /// <summary>
    /// The URI to connect to 
    /// </summary>
    /// <value></value>
    public string uri
    {
        get { return $"{protocol}://{ip}:{port}"; }
    }

    [Header("[internal ros info]")]

    /// <summary>
    /// The serialiser
    /// </summary>
    public RosSocket.SerializerEnum serializer;

    /// <summary>
    /// The ros protocol 
    /// </summary>
    public Protocol rosProtocol;
}

public class ROSDepthSubscriber : MonoBehaviour
{
    /// <summary>
    /// Connection info for ROS bridge
    /// </summary>
    [Header("[connection]")]
    public ROSConnectionInfo connectionInfo;

    /// <summary>
    /// The connector
    /// </summary>
    public ROSBridgeConnector connector;

    public void Start()
    {
        //Make a new connect and connect
        connector = new ROSBridgeConnector(connectionInfo);
        connector.Connect();

        //Subscribe to an event
    }

    // private void Subscribe()

    public void OnApplicationQuit()
    {
        //Close the connection on exit
        connector.Close();
    }
}
