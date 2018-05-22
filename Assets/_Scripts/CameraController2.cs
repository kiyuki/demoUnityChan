using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController2 : MonoBehaviour {
    public GameObject mainCamera;
    public float radius;
    public float theata;
    public float speed;

    private Rigidbody _rigidbody;

    public enum KindOfFall { sin, gravity };

    public KindOfFall kindOfFall = new KindOfFall();
	// Use this for initialization
	void Start () {
        _rigidbody = this.GetComponent<Rigidbody>();
        mainCamera = GameObject.FindWithTag("MainCamera");
        radius = 30;
        speed = 1.5f;
        mainCamera.transform.localEulerAngles = new Vector3(90f, 0, 0);
        mainCamera.transform.localPosition = new Vector3(250, 200, 250);
	}
	
	// Update is called once per frame
	void Update () {
        if(kindOfFall == KindOfFall.gravity)
        {
            if(mainCamera.transform.localPosition.y  <= 4.5)
            {
                _rigidbody.velocity = -_rigidbody.velocity;
            }
            //mainCamera.transform.s
        }else if(kindOfFall == KindOfFall.sin)
        {
            mainCamera.transform.localPosition = new Vector3(0, 1 + radius + radius * Mathf.Sin(Time.time * speed), 20);
        }
	}
}
