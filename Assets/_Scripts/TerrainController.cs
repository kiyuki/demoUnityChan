using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{

    private Terrain terrain;
    [SerializeField]
    private Texture2D[] textures;
    public GroundManager tc;
    // Use this for initialization
    void Start()
    {
        terrain = gameObject.GetComponent<Terrain>();
        GroundManager tc = new GroundManager(33, 33, textures);
        //StartCoroutine(tc.SetRealtimeTerrainHeight(terrain));
        tc.SetRealtimeTerrainHeight(terrain);
        terrain.terrainData = tc.getTerrainData();
    }

    private void Update()
    {
        /*
        Debug.Log("terrain" + terrain);
        tc.SetRealtimeTerrainHeight(terrain);
        terrain.terrainData = tc.getTerrainData();
        */
    }
}
