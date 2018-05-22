using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject mainCamera;
    public enum cameraPosi {upper, beside };
    public cameraPosi cameraposi;
	// Use this for initialization
	void Start () {
        mainCamera = GameObject.FindWithTag("MainCamera");
        
	}
	
	// Update is called once per frame
	void Update () {
        if(cameraposi == cameraPosi.beside)
        {
            mainCamera.transform.position = new Vector3(125, 200, 125);
            mainCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
        }else if(cameraposi == cameraPosi.upper)
        {
            mainCamera.transform.position = new Vector3(125, 125, 125);
            mainCamera.transform.localEulerAngles = new Vector3(90, 0, 0);

        }
	}
}
