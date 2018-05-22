using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows;
using Windows.Kinect;

public class TestKinect : MonoBehaviour
{
    private int test;
    public int _tes
    {
        set { this.test = value; }
        get { return this.test; }
    }

    private void Start()
    {

    }

    private void Update()
    {

    }
    /*
    private void OnDrawGizmos()
    {

        foreach (var data in _Data)
        {
            Gizmos.color = Color.blue;
            
            if (data == null)
            {
                continue;
            }
            Debug.Log("number i  " + i + "    trackingId : " + data.TrackingId);
            if (data.IsTracked)
            {
                foreach (KeyValuePair<JointType, Windows.Kinect.Joint> hoge in data.Joints)
                {
                    Gizmos.DrawSphere(GetVector3FromJoint(hoge.Value), 3);
                    Debug.Log("JointType : " + hoge.Key + " ,  Kinect.Joint : " + hoge.Value.Position);
                }
            }

            i++;
        }
        i = 0;
    }
    */

}
