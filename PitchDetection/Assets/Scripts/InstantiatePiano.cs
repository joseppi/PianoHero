using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePiano : MonoBehaviour
{
    public GameObject samplecubePrefab;
    public int PianoNumKeys = 88;
    public float maxScale = 3000000000;
    public static float[] freqBand = new float[8];
    public static float[] samples = new float[1024];

    GameObject[] sampleKey = new GameObject[512];
    // Start is called before the first frame update
    void Start()
    {
        //calc rows and cloumns        
        int ret = Mathf.RoundToInt(Mathf.Log(samples.Length));
        int name = 0;        
        for (int i = 0;i < sampleKey.Length; i++)
        {            
             GameObject instancesSampleCube = (GameObject)Instantiate(samplecubePrefab);
             instancesSampleCube.transform.localScale = new Vector3(10, 10, 10);
             instancesSampleCube.transform.position = this.transform.position + new Vector3(i, 0, 0);
             instancesSampleCube.transform.parent = this.transform;
             instancesSampleCube.name = "pianoKey" + name;
             sampleKey[i] = instancesSampleCube;
             ++name;                        
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < sampleKey.Length; i++)
        {
            if (sampleKey != null)
            {
                sampleKey[i].transform.localScale = new Vector3(10, (PitchDetector.samples[i] * maxScale) + 2, 10);
            }
        }

        int count = 0;

        //for (int i = 0; i < PianoNumKeys; i++)
        //{
        //    float average = 0;
        //    int sampleCount = (int)Mathf.Pow(2, i) * 2;
        //    if (i == 7)
        //    {
        //        sampleCount += 2;
        //    }
        //    for (int j = 0; j < sampleCount; j++)
        //    {
        //        //average += samples[count] * (count + 1);
        //        count++;
        //    }
        //    average /= count;
        //    //freqBand[i] = average * 30;
        //}
    }
}
