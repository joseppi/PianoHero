using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFTFreqBand : MonoBehaviour
{
    public float Value;
    public float Index;
    public float Freq;


    public FFTFreqBand(float value, float index, float freq)
    {
        this.Value = value;
        this.Index = index;
        this.Freq = freq;
    }
    void Init(int size)
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        Value = 0.0f;
        Index = 0.0f;
        Freq = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
