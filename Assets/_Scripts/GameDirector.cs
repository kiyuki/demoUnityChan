using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows;
using Windows.Kinect;


public class GameDirector : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject cameraFront;
    public GameObject cameraRight;
    public GameObject instance_terrain;

    public Rigidbody _rigidbody;
    public GameObject trampoline;

    private GameObject unitychan;

    private BodySourceManager _bodyManager;


    private Body[] _Data = null;

    public Vector3[] bodyPosition;
    public Vector3[] bodyPositionOneFlameBefore;

    private Vector3[] headposi;

    enum bodyUse
    {
        bodyHeadPosition,
        bodySpineBasePosition,
        bodyAnkleRightPosi,
        bodyAnkleLeftPosi
    }

    public float radius;
    public float theata;
    public float speed;

    private Material trampolineMate;

    // Use this for initialization
    private float timeForDivide;
    
    public float[] moveValue;
    public float sumOfMove;
    int moveValueCounter;

    public bool accelF;
    public bool addforce;
    public float jumpBias;

    void Start()
    {
        moveValue = new float[10];
        bodyPosition = new Vector3[4];
        headposi = new Vector3[5];
        bodyPositionOneFlameBefore = new Vector3[4];
        jumpBias = 10f;
        //moveValueOneStepBefore = new float[15];
        moveValueCounter = 0;
        instance_terrain = GameObject.Find("terrain");
        mainCamera = GameObject.FindWithTag("MainCamera");
        trampoline = GameObject.Find("trampoline");
        unitychan = GameObject.Find("unitychan");
        GameObject light = GameObject.Find("Directional Light");
        cameraRight = GameObject.Find("CameraFront");
        cameraFront = GameObject.Find("CameraRight");
        light.GetComponent<Light>().color = Color.white;
        light.transform.localPosition = new Vector3(125, 100, 125);
        light.transform.localEulerAngles = new Vector3(90, 0, 0);
        
        trampolineMate = trampoline.GetComponent<Renderer>().material;
        trampolineMate.color = Color.blue;

        _rigidbody = unitychan.GetComponent<Rigidbody>();

        _bodyManager = this.GetComponent<BodySourceManager>();
        radius = 30;
        speed = 1.5f;
        mainCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
        mainCamera.transform.localPosition = new Vector3(0, 1.38f, 0.45f);
        cameraFront.transform.localPosition = new Vector3(0, 1.38f, 0);
        cameraRight.transform.localEulerAngles = new Vector3(0, 90f, 0);
        cameraRight.transform.localPosition = new Vector3(0, 1.38f, 0);

        unitychan.transform.localPosition = new Vector3(250, 33f, 250);
        
    }

    // Update is called once per frame
    void Update()
    {
        timeForDivide += Time.deltaTime;
        updateBodyData();
        hightController(timeForDivide);
        positionController();
    }

    void updateBodyData()
    {

        int i = 0;
        _Data = _bodyManager.GetData();
        if (_Data == null)
        {
            return;
        }

        foreach (var data in _Data)
        {

            if (data == null)
            {
                continue;
            }
            //Debug.Log("number i  " + i + "    trackingId : " + data.TrackingId);
            if (data.IsTracked)
            {
                bodyPositionOneFlameBefore[(int)bodyUse.bodyAnkleLeftPosi] = bodyPosition[(int)bodyUse.bodyAnkleLeftPosi];
                bodyPositionOneFlameBefore[(int)bodyUse.bodyAnkleRightPosi] = bodyPosition[(int)bodyUse.bodyAnkleRightPosi];
                bodyPositionOneFlameBefore[(int)bodyUse.bodySpineBasePosition] = bodyPosition[(int)bodyUse.bodySpineBasePosition];
                bodyPositionOneFlameBefore[(int)bodyUse.bodyHeadPosition] = bodyPosition[(int)bodyUse.bodyHeadPosition];

                bodyPosition[(int)bodyUse.bodyAnkleLeftPosi] = GetVector3FromJoint(data.Joints[JointType.AnkleLeft]);
                bodyPosition[(int)bodyUse.bodyAnkleRightPosi] = GetVector3FromJoint(data.Joints[JointType.AnkleRight]);
                bodyPosition[(int)bodyUse.bodySpineBasePosition] = GetVector3FromJoint(data.Joints[JointType.SpineBase]);
                bodyPosition[(int)bodyUse.bodyHeadPosition] = GetVector3FromJoint(data.Joints[JointType.Head]);

                foreach (KeyValuePair<JointType, Windows.Kinect.Joint> hoge in data.Joints)
                {
                    //Debug.Log("JointType : " + hoge.Key + " ,  Kinect.Joint : " + hoge.Value.Position);
                }
            }

            i++;
        }
    }



    void hightController(float time)
    {
        if (moveValueCounter > 9)
        {
            moveValueCounter = 0;
        }

        /*
        if(_rigidbody.velocity.magnitude == 0)
        {

        }*/

        if (unitychan.transform.localPosition.y >= 29 && unitychan.transform.localPosition.y <= 32)
        {
            moveValue[moveValueCounter] = Mathf.Max(0, bodyPosition[(int)bodyUse.bodySpineBasePosition].y - bodyPositionOneFlameBefore[(int)bodyUse.bodySpineBasePosition].y);
            moveValueCounter++;
            foreach (var x in moveValue)
            {
                sumOfMove += x;

            }
            //Debug.Log("this is counter : " + moveValueCounter);
            trampolineMate.color = Color.yellow;
        }
        else
        {
            trampolineMate.color = Color.blue;
        }

        if (unitychan.transform.localPosition.y > Mathf.Min(30 + (unitychan.GetComponent<waveUnityChan>().maxHight-30)/10, 32.0f) )
        {


            //_rigidbody.velocity = new Vector3(0, Mathf.Abs(_rigidbody.velocity.y), 0);
            if (accelF && sumOfMove > 1)
            {
                _rigidbody.AddForce(new Vector3(0, sumOfMove*jumpBias, 0));
                timeForDivide = 0;
                sumOfMove = 0;
                unitychan.GetComponent<waveUnityChan>().maxHight = 0;
                accelF = false;
             }
        }
    }
    


    void positionController()
    {

        if (_rigidbody.velocity.magnitude == 0)
        {

            Vector3 tempPosi = bodyPosition[(int)bodyUse.bodySpineBasePosition] * 10;
            unitychan.transform.localPosition = new Vector3(tempPosi.x + 250, unitychan.transform.localPosition.y, tempPosi.z + 250);
            unitychan.transform.localEulerAngles = new Vector3(0, 0, 0);
            instance_terrain.transform.localPosition = new Vector3(tempPosi.x, 0f, tempPosi.z);
        }
        else
        {

            Vector3 tempPosi = bodyPosition[(int)bodyUse.bodySpineBasePosition] * 10;
            unitychan.transform.localPosition = new Vector3(tempPosi.x + 250, unitychan.transform.localPosition.y, tempPosi.z + 250);
            unitychan.transform.localEulerAngles = new Vector3(0, 0, 0);
            /*
            instance_terrain.transform.localPosition +=
                new Vector3(tempPosi.x - instance_terrain.transform.localPosition.x, 0f, tempPosi.z - instance_terrain.transform.localPosition.z) / 10;
                */

            instance_terrain.transform.localPosition = new Vector3(tempPosi.x, 0f, tempPosi.z);
        }
    }
    private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
