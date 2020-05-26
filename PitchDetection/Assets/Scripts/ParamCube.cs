using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamCube : MonoBehaviour
{
    public int band;
    public float startScale = 10.0f;
    public float scaleMultiplier = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(transform.localScale.x, (PitchDetector.freqBand[band] * scaleMultiplier) + startScale, transform.localScale.z);
    }
}
