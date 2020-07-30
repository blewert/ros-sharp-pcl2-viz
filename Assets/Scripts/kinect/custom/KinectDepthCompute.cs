using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class KinectDepthCompute : MonoBehaviour
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

    /// <summary>
    /// Kernel id
    /// </summary>
    public int kernelID;

    /// <summary>
    /// The compute shader
    /// </summary>
    public ComputeShader computeShader;

    /// <summary>
    /// The compute buffer
    /// </summary>
    public ComputeBuffer depthComputeBuffer;

    /// <summary>
    /// The out buffer
    /// </summary>
    public ComputeBuffer outBuffer;

    private Vector3[] outBufferData;

    public int maxBufferSize = 512 * 512;
    public int numThreads = 64;

    public Shader renderShader;
    private Material mat;

    public Vector3 renderArea = Vector3.one;

    // Start is called before the first frame update
    void Start()
    {
        //alloc max buf size for float3
        outBufferData = new Vector3[maxBufferSize];

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

        //Find the kernel, make the buffer
        kernelID = computeShader.FindKernel("CSMain");
        depthComputeBuffer = new ComputeBuffer(maxBufferSize / 2, sizeof(int), ComputeBufferType.Raw);
        outBuffer = new ComputeBuffer(maxBufferSize, sizeof(float) * 3);
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

        //Close buffers
        outBuffer.Dispose();
        depthComputeBuffer.Dispose();
    }

    private int i = 0;
    public int interval = 5;

    private void DispatchComputeShaders()
    {
        //Set buffers for compute shader
        computeShader.SetBuffer(kernelID, "depthBufferInt", depthComputeBuffer);
        computeShader.SetBuffer(kernelID, "outBuffer", outBuffer);
        computeShader.Dispatch(kernelID, (maxBufferSize / numThreads) / 2, 1, 1);

        //Get the data
        outBuffer.GetData(outBufferData);

        //Print out the first 10 bc why not
        for (int i = 0; i < 10; i++)
            Debug.Log($"{outBufferData[i].x}, {outBufferData[i].y}, {outBufferData[i].z}");
    }

    private void RenderPoints()
    {
        //Make a new material if needed
        if (mat == null)
        {
            mat = new Material(renderShader);
            mat.hideFlags = HideFlags.DontSave;
        }

        //Set material up for rendering
        mat.SetPass(0);

        //Set up render material before render
        mat.SetPass(0);
        mat.SetColor("_Tint", Color.white);
        mat.SetMatrix("_Transform", transform.localToWorldMatrix);
        mat.SetBuffer("_Vertices", outBuffer);
        mat.SetBuffer("_Colors", outBuffer);
        mat.SetVector("_PointMultMagnitude", renderArea);

        #if UNITY_2019_1_OR_NEWER
        Graphics.DrawProceduralNow(MeshTopology.Points, outBuffer.count, 1);
        #else
        Graphics.DrawProcedural(MeshTopology.Points, vertexBuffer.count, 1);
        #endif
    }

    private void OnRenderObject()
    {
        //Return if null
        if (outBuffer == null)
            return;

        //Check the camera: Is it the editor?
        var camera = Camera.current;
        if ((camera.cullingMask & (1 << gameObject.layer)) == 0) return;
        if (camera.name == "Preview Scene Camera") return;

        //Render points
        this.RenderPoints();
    }

    // Update is called once per frame
    void Update()
    { 
        //Interval
        //if ((i++ % interval) != 0)
            //return;

        //No reader? return
        if(depthReader == null)
            return;

        //Otherwise read the frame
        var frame = depthReader.AcquireLatestFrame();

        //Frame null? return
        if(frame == null)
            return;

        //Otherwise copy data
        frame.CopyFrameDataToArray(depthBuffer);
        frame.Dispose();

        //Set buffer data
        depthComputeBuffer.SetData(depthBuffer);

        //Dispatch compute shaders
        this.DispatchComputeShaders();
    }
}
