using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate512 : MonoBehaviour
{
    public GameObject samplecubePrefab;
    GameObject[] sampleCube = new GameObject[512];
    public float maxScale;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 512; i++)
        {
            GameObject instancesSampleCube = (GameObject)Instantiate(samplecubePrefab);
            instancesSampleCube.transform.position = this.transform.position;
            instancesSampleCube.transform.parent = this.transform;
            instancesSampleCube.name = "sampleCube" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0);
            instancesSampleCube.transform.position = Vector3.forward * 100;
            sampleCube[i] = instancesSampleCube;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetSamplesFromAudiClip();

    }

    private void GetSamplesFromAudiClip()
    {
        for (int i = 0; i < 512; i++)
        {
            if (sampleCube != null)
            {
                sampleCube[i].transform.localScale = new Vector3(10, (PitchDetector.samples[i] * maxScale) + 2, 10);
            }
        }        
    }
}
