using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveUnityChan : MonoBehaviour {

    public GameObject instance_terrain;
    public GameObject mainDirector;
    public GameObject unitychan;
    public float time;
    public float maxHight;
    public float strength;
	// Use this for initialization
	void Start () {
        strength = 1;
        instance_terrain = GameObject.Find("terrain");
        mainDirector = GameObject.Find("MainDirector");
        unitychan = GameObject.Find("unitychan");
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        if( this.transform.localPosition.y > maxHight)
        {
            maxHight = this.transform.localPosition.y;
        }
        instance_terrain.GetComponent<TestTerrain>().setHeight(time, (maxHight-30.1f)*strength);
    }

    private void OnCollisionEnter(Collision collision)
    {
        time = 0;
        mainDirector.GetComponent<GameDirector>().accelF = true;

        Debug.Log("OK");
        
    }

    private void OnCollisionStay(Collision collision)
    {
        mainDirector.GetComponent<GameDirector>().accelF = true;
        maxHight = 0;
        Debug.Log("OK2");
    }

}
