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
## The future
In the future, this README will be updated with further instructions of how 
to use this repository. For now, it only exists as a testbed for 
prototyping PCL2 visualisation solutions.
