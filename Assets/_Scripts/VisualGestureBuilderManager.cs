using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;

public class VisualGestureBuilderManager : MonoBehaviour
{
    public string databaseName = "squatForTrampoline.gbd";
    public BodySourceManager bodyManager;
    VisualGestureBuilderFrameSource gestureSource;
    VisualGestureBuilderFrameReader gestureReader;
    KinectSensor sensor = null;
    public Dictionary<Gesture, DiscreteGestureResult> DiscreteGestureResults;

    void Start()
    {
        sensor = KinectSensor.GetDefault();
        if (sensor != null)
        {
            if (!sensor.IsOpen)
            {
                Debug.Log("is OPEN");
                sensor.Open();
            }
        }
        gestureSource = VisualGestureBuilderFrameSource.Create(sensor, 0);
        gestureReader = gestureSource.OpenReader();
        if (gestureReader != null)
        {
            gestureReader.IsPaused = true;
            gestureReader.FrameArrived += GestureFrameArrived;
            Debug.Log("IS PAUSED");
        }

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, databaseName);
        using (VisualGestureBuilderDatabase database = VisualGestureBuilderDatabase.Create(path))
        {
            foreach (Gesture gesture in database.AvailableGestures)
            {
                gestureSource.AddGesture(gesture);
                Debug.Log(gesture.Name);
            }
        }
    }

    void Update()
    {
        if (!gestureSource.IsTrackingIdValid)
        {
            FindValidBody();
        }
    }

    void FindValidBody()
    {
        if (bodyManager != null)
        {
            Body[] bodies = bodyManager.GetData();
            if (bodies != null)
            {
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        SetBody(body.TrackingId);
                        break;
                    }
                }
            }
        }

    }

    public void SetBody(ulong id)
    {
        if (id > 0)
        {
            gestureSource.TrackingId = id;
            gestureReader.IsPaused = false;
        }
        else
        {
            gestureSource.TrackingId = 0;
            gestureReader.IsPaused = true;
        }
    }

    void GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
    {
        using (var frame = e.FrameReference.AcquireFrame())
        {
            if (frame != null)
            {
                DiscreteGestureResults = frame.DiscreteGestureResults;
                Debug.Log("FRAME ARRIVED");

                if (DiscreteGestureResults != null)
                {
                    foreach (KeyValuePair<Gesture, DiscreteGestureResult> pair in DiscreteGestureResults)
                    {
                        Debug.Log(pair.Key.Name);
                        Debug.Log("detected: " + pair.Value.Detected + ", confidence: " + pair.Value.Confidence);
                    }
                }
            }
        }
    }
}