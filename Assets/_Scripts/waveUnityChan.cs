using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveUnityChan : MonoBehaviour {
    
    public GameObject gameDirector_forDemo;
    public GameObject unitychan;
    [SerializeField] SoundManager soundManager;

    [SerializeField] AudioClip clip1;
    [SerializeField] AudioClip clip2;
    [SerializeField] AudioClip clip3;
    [SerializeField] AudioClip clip4;
    [SerializeField] AudioClip clip5;
    [SerializeField] AudioClip clip6;

    gameDirector_forDemo instance;
    // Use this for initialization
    void Start () {
        gameDirector_forDemo = GameObject.Find("MainDirector");
        unitychan = GameObject.Find("unitychan");
        instance = gameDirector_forDemo.GetComponent<gameDirector_forDemo>();

    }

    private void PlaySfx()
    {
        soundManager.RandomizeSfx(clip1, clip2, clip3, clip4,clip5,clip6);
    }

    private void OnCollisionEnter(Collision collision)
    {
        instance.accelF = true;
        instance.time = 0; // for addforce.
        PlaySfx();
        
    }

    private void OnCollisionStay(Collision collision)
    {
        instance.accelFromStay = true;
        
    }

}
