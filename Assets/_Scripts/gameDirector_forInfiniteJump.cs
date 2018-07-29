using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows;
using Windows.Kinect;

public class gameDirector_forInfiniteJump : MonoBehaviour
{
    //must for project //frame kara nannkatoreru
    private BodySourceManager _bodyManager;
    public GameObject instance_terrain;
    public GameObject unityChan;
    private Rigidbody rb_unityChan;
    public GameObject trampoline;
    private GameObject mainCamera;
    private GameObject cameraFront;
    private GameObject cameraRight;
    private GameObject light;
    private Material mateOfTrampo;

    //paramaters often use
    public float jumpStrength = 5;
    public float maxHeight;

    public float sumOfBodyMoveAve;

    private Body[] _Data = null;
    public Vector3[] bodyPosition;
    public Vector3[] bodyPositionBeforeOneFlame;
    public float[] buff_frameDiff;

    //var sometime use
    private Material trampolineMate;
    public float time;
    private float timeForGro;
    public bool accelF;
    public bool accelFromStay;

    public float threForSumOfMove;
    int buffIterate;

    void Start()
    {
        //get Object        
        instance_terrain = GameObject.Find("terrain");
        unityChan = GameObject.Find("unitychan");
        mainCamera = GameObject.FindWithTag("MainCamera");
        trampoline = GameObject.Find("trampoline");
        light = GameObject.Find("Directional Light");
        cameraRight = GameObject.Find("CameraFront");
        cameraFront = GameObject.Find("CameraRight");
        rb_unityChan = unityChan.GetComponent<Rigidbody>();
        _bodyManager = this.GetComponent<BodySourceManager>();

        //set initial value and initialize
        bodyPosition = new Vector3[25];
        bodyPositionBeforeOneFlame = new Vector3[25];
        buff_frameDiff = new float[10];
        light.GetComponent<Light>().color = Color.white;
        light.transform.localPosition = new Vector3(125, 100, 125);
        light.transform.localEulerAngles = new Vector3(90, 0, 0); mainCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
        mainCamera.transform.localPosition = new Vector3(0, -0.5f, 0);
        cameraFront.transform.localPosition = new Vector3(0, 1.38f, 0);
        cameraRight.transform.localEulerAngles = new Vector3(0, 90f, 0);
        cameraRight.transform.localPosition = new Vector3(0, 1.38f, 0);
        mateOfTrampo = trampoline.GetComponent<Renderer>().material;
        mateOfTrampo.color = Color.blue;

        threForSumOfMove = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        updateBodyData();
        sumBodyMove();
        heightTaker();
        moveGround();
        trampolineColor();
        if (sumOfBodyMoveAve > threForSumOfMove ) addForce(sumOfBodyMoveAve);
    }

    void moveGround()
    {
        instance_terrain.GetComponent<TestTerrain>().setHeight(maxHeight, accelF);
    }

    void addForce(float forceOfValueForAdd)
    {
        if (forceOfValueForAdd < threForSumOfMove) forceOfValueForAdd = 0;
        Vector3 temp = new Vector3(0, forceOfValueForAdd * jumpStrength, 0);
        if (rb_unityChan.velocity.y < 0) rb_unityChan.velocity = -rb_unityChan.velocity;
        rb_unityChan.AddForce(temp);
        accelF = false;
        accelFromStay = false;
    }

    void heightTaker()
    {
        time += Time.deltaTime;
        if (mainCamera.transform.position.y > maxHeight)
        {
            maxHeight = mainCamera.transform.position.y - 29;
        }
        //instance_terrain.GetComponent<TestTerrain>().setHeight(time, (maxHight - 30.1f) * waveStrength);
    }

    void trampolineColor()
    {
        if (accelF || accelFromStay)
        {
            mateOfTrampo.color = Color.blue;
        }
        else
        {
            mateOfTrampo.color = Color.white;
        }
    }
    void sumBodyMove()
    {

        float diffBetweenFrame = 0;
        for (int i = 0; i < 21; i++)
        {
            if ((6 <= i && i <= 8) || (10 <= i && i <= 12)) continue;
            diffBetweenFrame += bodyPositionBeforeOneFlame[i].y - bodyPosition[i].y;

        }
        if (diffBetweenFrame > 0) buff_frameDiff[buffIterate] = diffBetweenFrame;
        buffIterate++;
        if (buffIterate >= 10) buffIterate = 0;
        sumOfBodyMoveAve = 0;
        for (int i = 0; i < 10; i++)
        {
            sumOfBodyMoveAve += buff_frameDiff[i];
        }
        sumOfBodyMoveAve = sumOfBodyMoveAve / 10;
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
                foreach (KeyValuePair<JointType, Windows.Kinect.Joint> hoge in data.Joints)
                {
                    bodyPositionBeforeOneFlame[(int)hoge.Key] = bodyPosition[(int)hoge.Key];

                    bodyPosition[(int)hoge.Key] = GetVector3FromJoint(hoge.Value);
                    //Debug.Log(bodyPosition[(int)hoge.Key]);
                }

            }

            i++;
        }
    }
    private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }



}

