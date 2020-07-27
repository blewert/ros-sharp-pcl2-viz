# What is this?
This is a small Unity project which visualises live PointCloud2 data sent 
from ROS. Communication is done via the 
[ros-sharp](https://github.com/siemens/ros-sharp/) library which talks to 
ROS via ROSBridge. Rendering is done by converting byte array data to 
per-vertex data (position, colour and initially, indices) via a GPGPU 
method (HLSL compute shaders). This repo takes inspiration (and some 
important snippets of code) from [inmo-jang's PointCloud streaming 
repository](https://github.com/inmo-jang/unity_assets/tree/master/PointCloudStreaming). 
There are also some files for communicating with the Kinect, which we are 
currently investigating.

![example-image](https://i.imgur.com/IrDAytm.png)

## Quick-start
1. Download a copy of this repository, and open it up as a Unity project.
2. Create a new scene
3. In that new scene, create a new empty game object, and attach a `RosConnector` script to it. Fill in the relevant details in the inspector. Add a `Mesh Renderer` component to this object.
4. There are two scripts you need to attach after this step:
   1. A subscriber to read the data from messages sent from ROS.
   2. A visualiser which takes that data, transforms it, and renders it to the screen.
5. To do this, click on the object in the hierarchy which contains the ROSConnector. Then, navigate (using the top menu) to `Component > UoL > ROS > PointCloud2 Subscriber` and click it to attach the subscriber.
   1. Fill in the relevant topic. Ignore the `timestep` variable.
6. Do the same, but for `Component > UoL > ROS > PointCloud2 Visualiser (direct-to-shader)`.
   1. Fill in the "Compute Shader" field by choosing the `PointCloudMeshVertices` compute shader.
   2. Fill in the "Render Shader" field by choosing the `PointCloudBufferRenderer` shader.
   3. Leave the "Render Area" variable (it should be `(1, 0.2, 1)` by default).
7. Your inspector should look like this:

![inspector in unity](https://i.imgur.com/volzuFq.png)


## The future
In the future, this README will be updated with further instructions of how 
to use this repository. For now, it only exists as a testbed for 
prototyping PCL2 visualisation solutions.
