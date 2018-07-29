using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows;
using Windows.Kinect;


public class gameDirector_forDemo : MonoBehaviour
{
    public float threForSumOfMove = 3.0f;
    public float jumpStrength =10;
    public float sumOfBodyMoveAve;
    public float maxHeight;

    public float waveWidth = 2;
    public float wavePreceedingBias = 1.5f;
    public float waveChaseBias = 0.5f;

    public float amplitude = 1;
    public float timeBias = 1;
    public float distanceBias = 1;
    public float attenuation = 1;
    public float cycle = 1;

    public string nameOfGround;

    [Range(0,5)]
    public int indexGround =0;

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
    
    public Text text;
    private Material mateOfTrampo;

    //paramaters often use


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
    public bool heightChange = false;
    public bool existHuman = false;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        updateBodyData();
        sumBodyMove();
        heightTaker();
        moveGround();
        changeGroundMaterial();
        //trampolineColor();
        if (sumOfBodyMoveAve > threForSumOfMove && (accelF && time > 0.05f || accelFromStay && time > 0.05f)) addForce(sumOfBodyMoveAve);
    }

    void moveGround()
    {
        instance_terrain.GetComponent<TestTerrain>().setHeight(maxHeight, accelF);
    }

    void addForce(float forceOfValueForAdd)
    {
        if (forceOfValueForAdd < threForSumOfMove) forceOfValueForAdd = 0;
        Vector3 temp = new Vector3(0, forceOfValueForAdd * jumpStrength, 0);
        //if (rb_unityChan.velocity.y < 0) rb_unityChan.velocity = -rb_unityChan.velocity;
        rb_unityChan.AddForce(temp);
        accelF = false;
        accelFromStay = false;
    }

    void heightTaker()
    {
        heightChange = false;

        if (mainCamera.transform.position.y > maxHeight)
        {
            maxHeight = mainCamera.transform.position.y;
            heightChange = true;
        }
        //instance_terrain.GetComponent<TestTerrain>().setHeight(time, (maxHight - 30.1f) * waveStrength);
    }

    void trampolineColor()
    {
        if (time < 0.1f || accelFromStay)
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
        for(int i=0; i<21; i++)
        {
            if ((6 <= i && i <= 8) || (10 <= i && i <= 12)) continue;
            diffBetweenFrame +=  Mathf.Max( (bodyPositionBeforeOneFlame[i].y - bodyPosition[i].y),0);

        }
        if (!existHuman) diffBetweenFrame = 0;
        if(diffBetweenFrame>=0) buff_frameDiff[buffIterate] = diffBetweenFrame;
        buffIterate++;
        if (buffIterate >= 5) buffIterate = 0;
        sumOfBodyMoveAve = 0;
        for (int i=0; i < 5; i++)
        {
            sumOfBodyMoveAve += buff_frameDiff[i];
        }
        sumOfBodyMoveAve = sumOfBodyMoveAve / 5;
    }

    void updateBodyData()
    {
        existHuman = false;
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
                existHuman = true;
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

    private void changeGroundMaterial()
    {
        if(bodyPosition[8].x  < bodyPosition[7].x && bodyPosition[8].y < bodyPosition[7].y)
        {
            indexGround++;
        }
        if( bodyPosition[4].x > bodyPosition[11].x && bodyPosition[4].y < bodyPosition[11].y)
        {
            indexGround--;
        }

        switch (indexGround)
        {
            case 0: // middle
                text.text = "SOFT";
                jumpStrength = 40;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
            case 1: //high
                text.text = "HARD";
                jumpStrength = 50;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
            case 2: // super high 
                text.text = "GOD TRAMPOLINE";
                jumpStrength = 100;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
            case 3:  // middle wave 
                text.text = "MIDDLE WAVE";
                jumpStrength = 50;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
            case 4:  // super wave 
                text.text = "SUPER WAVE";
                jumpStrength = 150;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
            case 5: // super high and super wave many wave
                text.text = "SUPER  MANY WAVE";
                jumpStrength = 1000;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
            case 6: // persistin wave long time situkoi
                text.text = "WAVES";
                jumpStrength = 100;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
            case 7:     // fast wave 
                text.text = "FAST WAVE";
                jumpStrength = 100;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
            case 8:     //complex wave, many wave move fast
                text.text = "middleJump";
                jumpStrength = 100;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
            case 9:     //
                text.text = "middleJump";
                jumpStrength = 100;
                waveWidth = 2;
                wavePreceedingBias = 1.5f;
                waveChaseBias = 0.5f;
                amplitude = 1;
                timeBias = 1;
                distanceBias = 1;
                attenuation = 1;
                cycle = 1;
                break;
        }

    }

}
