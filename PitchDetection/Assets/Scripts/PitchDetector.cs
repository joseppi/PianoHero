using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

using System;

[RequireComponent(typeof(AudioSource))]
public class PitchDetector : MonoBehaviour
{
    struct FFTFreqBand
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
    }

    // Microphone input
    public AudioMixerGroup mixerGroupMicrophone, mixerGroupMaster;
    TMPro.TMP_Dropdown displayInputDevices;
    List<string> inputDevices = new List<string>();
    AudioSource audioSource;
    bool use_microphone;
    AudioClip audioClip;
    string selectedDevice;
    string[] micDevices;
    public float decibelDetectionClosingValue = 25.0f;
    public float detectionOpeningValue = 0.005f;

    //Storing Samples    
    public static float[] samples = new float[2048];
    public static float[] samplesdB = new float[2048];
    public static float[] samplesVal = new float[2048];    
    public static float[,] samplesValStored = new float[512,3];
    float[] storedMaxVal = new float[3];
    float[] samplesSavedFreq = new float[10];
    FFTFreqBand[] samplesStored = new FFTFreqBand[10];

    //UI Display
    Text pitchDisplay;
    Text avgPitchDisplay;
    Text noteDisplay;
    Text harmonicNoteDisplay;
    String note = "Note Display";
        
    float storeddB = 0.0f;
    float storeVal = 0.0f;
    float fundamentalFrequencyOutput = 0.0f;
    float freqperBand;

    private float rmsValue = 0.0f;
    private float dbValue = 0.0f;
    private float refValue;

    // Start is called before the first frame update
    void Start()
    {
        refValue = 0.001f;
        audioSource = GetComponent<AudioSource>();
        freqperBand = 24000.0f / samples.Length;




        //displayInputDevices.AddOptions(inputDevices);

    }

    public void StartRecording()
    {       
        if (Microphone.devices.Length > 0)
        {
            micDevices = Microphone.devices;
            audioSource.outputAudioMixerGroup = mixerGroupMicrophone;
            audioSource.clip = Microphone.Start(micDevices[0], false, 60, AudioSettings.outputSampleRate);
        }

        if (Microphone.IsRecording(micDevices[0]))
        {
            while (!(Microphone.GetPosition(micDevices[0]) > 0)) { }
            audioSource.Play();
        }

        for (int i = 0; i < micDevices.Length; i++)
        {
            inputDevices.Add(micDevices[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        AvgPitchCalc();
        GetNoteName();
    }

    void GetSpectrumAudioSource()
    {
        //We are using hamming window because it reduces the sidelobes.
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
    }
    private bool ValueDetection(float value)
    {
        int i = 0;
        float maxVal = 0;
        
        for (i = 0; i < (samples.Length / 4); i++)
        {
            //Get Highest Value
            if (samples[i] > maxVal)
            {
                maxVal = samples[i];

            }
        }
        
        //Debug.Log(maxVal);
        if (maxVal < 0.00001) maxVal = 0; // clamp it to 0 val
        if (maxVal > value)
        {
            if (storedMaxVal[0] > maxVal && storedMaxVal[1] > maxVal && storedMaxVal[2] > maxVal)
            {
                //Debug.Log(maxVal + " ------------------------------------------------------Accepted Opening");
                for (int y = 0; y < 512;y++)
                {
                    samples[y] = samplesValStored[y,0];
                }
                storedMaxVal = new float[3];

                return true;                                                   
            }
            else
            {
                for (int it = 0; it < storedMaxVal.Length; it++)
                {
                    if (storedMaxVal[it] < maxVal)
                    {
                        storedMaxVal[it] = maxVal;
                        for (i = 0; i < (samples.Length / 4); i++)
                        {
                            samplesValStored[i, it] = samples[i];
                        }
                        break;
                    }
                }
                storeVal = maxVal;
                return false;
            }
        }
        return false;
    }
    private bool ValueDetectionOpening(float value)
    {
        int i = 0;
        float maxVal = 0;
        for (i = 0; i < (samples.Length / 4); i++)
        {
            //Get Highest Value
            if (samples[i] > maxVal)
            {
                maxVal = samples[i];
            }
        }
        //Debug.Log(maxVal);
        if (maxVal < 0.00001) maxVal = 0; // clamp it to 0 val
        if (maxVal > value)
        {
            if (storeVal > maxVal)
            {
                //Debug.Log(maxVal + " ------------------------------------------------------Accepted Opening");                
                storeVal = 0.0f;
                return true;
            }
            else
            {
                storeVal = maxVal;
                return false;
            }
        }
        return false;
    }

    private bool ValueDetectionClosing(float value)
    {
        int i = 0;
        float maxVal = 0;
        for (i = 0; i < (samples.Length / 4); i++)
        {
            //Get Highest Value
            if (samples[i] > maxVal)
            {
                maxVal = samples[i];
            }
        }

        if (maxVal < 0.00001) maxVal = 0; // clamp it to 0 val
        if (maxVal > value)
        {
            if (storeVal > maxVal)
            {
                Debug.Log(maxVal + " Accepted Opening");
                storeVal = 0.0f;
                return true;
            }
            else
            {
                storeVal = maxVal;
                return false;
            }
        }
        return false;
    }

    //Mic Detection is in charge of start/stop the pitch algorithm
    private bool DecibelDetectionOpening(float openingdBs)
    {
        audioSource.GetOutputData(samplesdB, 0); // fill array with samples
        int i = 0;
        float sum = 0;
        for (i = 0; i < samplesdB.Length; i++)
        {
            sum += samplesdB[i] * samplesdB[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / samplesdB.Length); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
        if (dbValue > openingdBs)
        {
            if (storeddB > dbValue)
            {
                //Debug.Log(dbValue + " Accepted Opening");
                storeddB = 0.0f;
                return true;
            }
            else
            {
                storeddB = dbValue;
                return false;
            }
        }
        return false;
    }

    //Mic Detection is in charge of start/stop the pitch algorithm
    private bool DecibelDetectionClosing(float closingdBs)
    {
        //Debug.Log("Current dB value " + dbValue);
        audioSource.GetOutputData(samplesdB, 0); // fill array with samples
        int i = 0;
        float sum = 0;
        for (i = 0; i < samplesdB.Length; i++)
        {
            sum += samplesdB[i] * samplesdB[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / samplesdB.Length); // rms = square root of average
        dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
        if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
        if (dbValue < closingdBs)
        {
            if (storeddB < dbValue)
            {
                //Debug.Log(dbValue + " Accepted Closing");
                storeddB = 0.0f;
                NotesInteractionHandler.noteID = -1;
                return true;
            }
            else
            {
                storeddB = dbValue;
                return false;
            }
        }


        return false;
    }

    void InsertArray(FFTFreqBand[] sampleArr, int size, FFTFreqBand sample)
    {
        List<FFTFreqBand> store = new List<FFTFreqBand>();
        for (int i = 0; i < sampleArr.Length; i++)
        {
            //Is the new value higher than the value from the list?
            if (sample.Value < sampleArr[i].Value)
            {
                //Save the value from the original list
                store.Add(sampleArr[i]);
            }
            else
            {
                //Save the new value
                store.Add(sample);
                //Save the remaining part of the list
                for (int j = i; j < size; j++)
                {
                    //As long as it's not empty
                    if (sampleArr[j].Value > 0.0f) { store.Add(sampleArr[j]); }
                    else { break; }
                }
                break;
            }
        }
        int count = 0;
        foreach (FFTFreqBand ret in store)
        {
            if (count < size)
            {
                sampleArr[count] = ret;
                count++;
            }
            else
                break;
        }
    }

    private void AvgPitchCalc()
    {
        DecibelDetectionClosing(decibelDetectionClosingValue);        
        if (ValueDetection(detectionOpeningValue))
        {
            //Checking all the samples
            for (int i = 0; i < (samples.Length / 4); ++i)
            {
                //Get the value of a sample
                float v = samples[i];

                //high-pass filter
                if (v > 0.001f)
                {
                    //Iterate though saved values
                    float itFreq = i * freqperBand;
                    FFTFreqBand itV = new FFTFreqBand(v, i, itFreq);

                    InsertArray(samplesStored, samplesStored.Length, itV);
                }
            }

            if (samplesStored[0].Freq > 27.5f) //we use 27.5hz since it's the lowest frequency a piano can make.
            {
                FFTFreqBand itVal = new FFTFreqBand(0.0f, 0.0f, 0.0f);

                //Delete the frequency samples that their band indexs are adjecent based on their lowest value                
                for (int i = 0; i < samplesStored.Length; i++)
                {
                    itVal = samplesStored[i];
                    for (int j = 0; j < samplesStored.Length; j++)
                    {
                        //Don't compare the iterated frequency with itself.
                        if (i != j)
                        {
                            //Look for adjecent frequency bands
                            if (itVal.Index + 1 == samplesStored[j].Index || itVal.Index - 1 == samplesStored[j].Index)
                            {
                                //Delete the band with less value than the other.
                                if (itVal.Value > samplesStored[j].Value)
                                {
                                    samplesStored[j] = new FFTFreqBand(0.0f, 0.0f, 0.0f);
                                }
                            }

                        }
                    }
                    samplesStored[i] = itVal;
                }

                //Save the frequencies in another list for optimized calculations
                for (int i = 0; i < samplesStored.Length; i++)
                {
                    samplesSavedFreq[i] = samplesStored[i].Freq;
                }

                fundamentalFrequencyOutput = 4186.0f;
                FFTFreqBand fundamentalSample = new FFTFreqBand(0.0f, 0.0f, 0.0f);
                for (int i = 0; i < samplesSavedFreq.Length; i++)
                {
                    //Get the lowest frequency detected and save it.
                    if (samplesSavedFreq[i] > 0.0f && samplesSavedFreq[i] < fundamentalFrequencyOutput)
                    {
                        fundamentalFrequencyOutput = samplesSavedFreq[i];
                    }
                }
            }
        }
        else
        {
            //avgPitchDisplay.text = "AVGPitch";            
            samplesStored = new FFTFreqBand[10];
        }
    }

    IEnumerator OpenMicDelay(float time)
    {
        yield return new WaitForSeconds(time);
    }

    private void GetNoteName()
    {
        //This are the frequencies of the piano 27.5hz to 4186.0hz
        //We set it to 246 since it's the lowest frequency we are looking 
        if (fundamentalFrequencyOutput > 246.5f)
        {
            note = null;
            //Relate pitch to musical notation with harmonics.
            for (int i = 0; i < 24; i++) //Check from Do 261.6freq to Do 522freq
            {
                //Get the pitch of the note we are comparing
                float testPitchIterator = Mathf.Pow(2.0f, i / 12.0f) * 261.6f;

                //Get variable tolerance
                //tolerance[0] is the difference of testPitch and testPitch-1
                //tolernace[1] is the difference of testPitch+1 and testPitch
                float[] tolerance = GetTolerance(i);

                //Identify the most low freq and look for his musical notation.
                if (fundamentalFrequencyOutput > testPitchIterator - tolerance[0])
                {
                    if (fundamentalFrequencyOutput < testPitchIterator + tolerance[1])
                    {
                        //Check his harmonics
                        for (int j = 0; j < samplesSavedFreq.Length; j++)
                        {
                            if (samplesSavedFreq[j] > 0.0f)
                            {
                                note = HarmonicDetection(samplesSavedFreq[j], testPitchIterator, tolerance, 2.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                note = HarmonicDetection(samplesSavedFreq[j], testPitchIterator, tolerance, 3.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                note = HarmonicDetection(samplesSavedFreq[j], testPitchIterator, tolerance, 4.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (note != null) break;
            }
        }

        note = null;
        fundamentalFrequencyOutput = 0;
    }

    private float[] GetTolerance(int index)
    {
        float[] ret = { 0.0f, 0.0f };
        float testPitchIterator = Mathf.Pow(2.0f, index / 12.0f) * 261.6f;
        float testNoteIteratorBelow = Mathf.Pow(2.0f, (index - 1) / 12.0f) * 261.6f;
        float testNoteIteratorAbove = Mathf.Pow(2.0f, (index + 1) / 12.0f) * 261.6f;
        ret[0] = testPitchIterator - testNoteIteratorBelow; //Minus Tolerance
        ret[1] = testNoteIteratorAbove - testPitchIterator; //Plus Tolerance


        return ret;
    }

    private String HarmonicDetection(float sample, float testNoteIterator, float[] tolerance, float harmonicNumber)
    {
        float harmPitch = testNoteIterator * harmonicNumber;
        if (sample > harmPitch - tolerance[0])
        {
            if (sample < harmPitch + tolerance[1])
            {
                //Debug.Log("Harmonic number "+ harmonicNumber + "of " + pitchToNote(testNoteIterator)  + " Detected!");
                return pitchToNote(testNoteIterator);
            }
        }
        return null;
    }

    private String pitchToNote(float pitch)
    {
        String[] musicalNotes = new string[24] { "Do/C", "Do#", "Re/D", "Re#", "Mi/E", "Fa/F", "Fa#", "Sol/G", "Sol#", "La/A", "La#", "Si/B", "Do/C", "Do#", "Re/D", "Re#", "Mi/E", "Fa/F", "Fa#", "Sol/G", "Sol#", "La/A", "La#", "Si/B" };
        for (int i = 0; i < 24; i++)
        {
            float testNoteIterator = Mathf.Pow(2.0f, i / 12.0f) * 261.6f;
            float[] tolerance = GetTolerance(i);
            if (pitch > testNoteIterator - tolerance[0])
            {
                if (pitch < testNoteIterator + tolerance[1])
                {
                    return musicalNotes[i];
                }
            }
        }
        return null;
    }

    private void DebugAudioFile()
    {
        Debug.Log(samples.ToString());
        float samplespersecond = audioClip.samples / audioClip.length;
        Debug.Log(samplespersecond.ToString());
        Debug.Log(audioClip.samples.ToString());
    }

    public void DropDownValueChanged(int value)
    {
        Microphone.End(selectedDevice);
        audioSource.Stop();
        audioSource.clip = null;


        audioSource.clip = Microphone.Start(micDevices[value], true, 600, AudioSettings.outputSampleRate);
        selectedDevice = micDevices[value];
        audioSource.Play();
    }


    /*
 * La 220
 * 13
 * La# 233
 * 13
 * Si 246
 * 15
 * Do 261 (Middle C)
 * 16
 * Do# 277 
 * 16
 * Re 293
 * 18
 * Re# 311
 * 18
 * Mi 329
 * 20
 * Fa 349
 * 20
 * Fa# 369
 * 22
 * Sol 391
 * 24
 * Sol# 415
 * 25
 * La 440 (Base Frequency)
 * 26
 * La# 466
 * 27
 * Si 493
 * 30
 * Do 523
 * 31
 * Do# 554
 * 33
 * Re 587
 * 35
 * Re# 622
 * 37
 * Mi 659
 * 39
 * Fa 698
 * 41
 * Fa# 739
 * 44
 * Sol 783 
 * 47
 * Sol# 830
 * 50
 * La 880
 * 52
 * La# 932
 * 55
 * Si 987
 * 
 * 
 * 
 */


}