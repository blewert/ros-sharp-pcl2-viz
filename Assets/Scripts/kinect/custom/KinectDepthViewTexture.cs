using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class KinectDepthViewTexture : MonoBehaviour
{
    /// <summary>
    /// The sensor for the kinect
    /// </summary>
    private KinectSensor sensor;

    /// <summary>
    /// The reader for the depth frame
    /// </summary>
    private DepthFrameReader depthReader;

    /// <summary>
    /// Buffer for the depth data
    /// </summary>
    private ushort[] depthBuffer;

    // Start is called before the first frame update
    void Start()
    {

        //Get the sensor on the kinect
        this.sensor = KinectSensor.GetDefault();

        //Null? Throw a unity exception
        if(this.sensor == null)
            throw new UnityException("Couldn't find the kinect sensor.");

        //Open the sensor
        this.sensor.Open();

        //Open the reader
        this.depthReader = sensor.DepthFrameSource.OpenReader();

        //Build the buffer
        this.depthBuffer = new ushort[sensor.DepthFrameSource.FrameDescription.LengthInPixels];

        //Log out some info
        this.PrintKinectStatus();
    }

    private void PrintKinectStatus()
    {
        //Print out first line
        Debug.Log($"Kinect [{this.sensor.UniqueKinectId}] found. Open: {sensor.IsOpen}, Available: {sensor.IsAvailable}");

        //Depth frame resolution
        Debug.Log($"- Depth frame {sensor.DepthFrameSource.FrameDescription.Height} by {sensor.DepthFrameSource.FrameDescription.Width}");

        Debug.Log($"- Depth frame active: {sensor.DepthFrameSource.IsActive}");
    }

    void OnApplicationQuit()
    {
        //Close the depth reader
        if(depthReader != null)
            depthReader.Dispose();

        //Close the sensor
        if(sensor != null && sensor.IsOpen)
            sensor.Close();
    }

    private int i = 0;
    public int interval = 5;

    // Update is called once per frame
    void Update()
    {
        //Interval
        if((i++ % interval) != 0)
            return;

        //No reader? return
        if(depthReader == null)
            return;

        //Otherwise read the frame
        var frame = depthReader.AcquireLatestFrame();

        //Frame null? return
        if(frame == null)
            return;

        // var buffer = new ComputeBuffer(depthBuffer.Length, sizeof(ushort));
        // buffer.SetData(depthBuffer);

        //Otherwise copy data
        frame.CopyFrameDataToArray(depthBuffer);
        frame.Dispose();
    }
}
