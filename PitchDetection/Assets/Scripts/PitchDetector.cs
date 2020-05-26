using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

using System;

[RequireComponent (typeof (AudioSource))]
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
    public bool use_microphone;
    public AudioClip audioClip;
    public string selectedDevice;
    public string[] micDevices;
    public AudioMixerGroup mixerGroupMicrophone, mixerGroupMaster; 


    public static float[] freqBand = new float[8];
    public static float[] samples = new float[1024];

    FFTFreqBand[] samplesStored = new FFTFreqBand[10];    
    public float audioProfile;

    public enum _channel { Stereo, Left, Right };
    public _channel channel = new _channel ();

    AudioSource audioSource;
    Text pitchDisplay;
    Text avgPitchDisplay;
    Text noteDisplay;
    Text harmonicNoteDisplay;
    String note = "Note Display";

    FFTFreqBand[] savedVals = new FFTFreqBand[3];
    float dt = 0.0f;
    float dt2 = 0.0f;
    float[] freqValues = new float[3];
    float storeddB = 0.0f;
    float frequencyOutput = 0.0f;
    float freqperBand;    

    //TODO improve tolerance detection
    float tolerance = 13.0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        pitchDisplay = GameObject.Find("Pitch Display").GetComponent<Text>();
        avgPitchDisplay = GameObject.Find("Avg Pitch Display").GetComponent<Text>();
        noteDisplay = GameObject.Find("Note Display").GetComponent<Text>();
        harmonicNoteDisplay = GameObject.Find("Harmonic Note Display").GetComponent<Text>();

        freqperBand = 24000.0f / samples.Length;

        if (use_microphone)
        {
            if (Microphone.devices.Length > 0)
            {
                micDevices = Microphone.devices;                
                audioSource.outputAudioMixerGroup = mixerGroupMicrophone;
                audioSource.clip = Microphone.Start(micDevices[0], true, 600, AudioSettings.outputSampleRate);
            }
            else
                use_microphone = false;
        }
        if (!use_microphone)
        {
            audioSource.outputAudioMixerGroup = mixerGroupMaster;
            audioSource.clip = audioClip;
        }
        for (int i = 0; i < micDevices.Length;i++)
        {
            //Debug.Log(micDevices[i]);
        }
        audioSource.Play();

        
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        //MakeFrequencyBands();
        //PitchCalc();
        AvgPitchCalc();
        GetNoteName();
        //DebugAudioFile();        
    }

    private void GetNoteName()
    {
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
        String[] musicalNotes = new string[12] {"La/A", "La#", "Si/B", "Do/C", "Do#", "Re/D", "Re#", "Mi/E", "Fa/F", "Fa#", "Sol/G", "Sol#"};

        float[] samplesSavedFreq = new float[10];

        float detectFreq = 0.0f;
        for (int i = 0; i < samplesStored.Length;i++)
        {
            samplesSavedFreq[i] = samplesStored[i].Freq;
            detectFreq += samplesStored[i].Freq;
        }
        
        if (detectFreq > 0.0f)
        {
            note = null;
            //Relate pitch to musical notation with harmonics.
            for (int i = 0; i < 12; i++)
            {
                float testNoteIterator = Mathf.Pow(2.0f, i / 12.0f) * 261.6f;
                //Identify the most low freq and associate it with musical notation
                if (frequencyOutput > testNoteIterator - tolerance)
                {
                    if (frequencyOutput < testNoteIterator + tolerance)
                    {
                        //Check his harmonics
                        for (int j = 0; j < samplesSavedFreq.Length; j++)
                        {                            
                            if (j != i && samplesSavedFreq[j] > 0.0f)
                            {                                
                                note = HarmonicDetection(samplesSavedFreq[j], testNoteIterator, 2.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                note = HarmonicDetection(samplesSavedFreq[j], testNoteIterator, 3.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                note = HarmonicDetection(samplesSavedFreq[j], testNoteIterator, 4.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                note = HarmonicDetection(samplesSavedFreq[j], testNoteIterator, 5.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                note = HarmonicDetection(samplesSavedFreq[j], testNoteIterator, 6.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                note = HarmonicDetection(samplesSavedFreq[j], testNoteIterator, 7.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                note = HarmonicDetection(samplesSavedFreq[j], testNoteIterator, 8.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                                note = HarmonicDetection(samplesSavedFreq[j], testNoteIterator, 9.0f);
                                if (note != null)
                                {
                                    NotesInteractionHandler.noteID = i;
                                    break;
                                }
                            }                            
                        }
                        //note = musicalNotes[i];
                    }
                }
                if (note != null) break;                
            }
        }

        if (note != null)
        {
            harmonicNoteDisplay.text = note.ToString();
            
        }
        note = null;
        noteDisplay.text = pitchToNote(frequencyOutput);// note.ToString();        
    }

    private String HarmonicDetection(float sample, float testNoteIterator, float harmonicNumber)
    {
        float harmPitch = testNoteIterator * harmonicNumber;
        if (sample > harmPitch - freqperBand/2)
        {
            if (sample < harmPitch + freqperBand/2)
            {
                //Debug.Log("Harmonic number "+ harmonicNumber + "of " + pitchToNote(testNoteIterator)  + " Detected!");
                return pitchToNote(testNoteIterator);
            }
        }
        return null;
    }

    private String pitchToNote(float pitch)
    {
        String[] musicalNotes = new string[12] { "La/A", "La#", "Si/B", "Do/C", "Do#", "Re/D", "Re#", "Mi/E", "Fa/F", "Fa#", "Sol/G", "Sol#"};
        for (int i = 0; i < 24; i++)
        {
            float testNoteIterator = Mathf.Pow(2.0f, i / 12.0f) * 220.0f;
            //Debug.Log(frequDisplayTest);
            if (pitch > testNoteIterator - tolerance)
            {
                if (pitch < testNoteIterator + tolerance)
                {
                    if (i >= 12)
                    {
                        i = i - 12;                        
                        return musicalNotes[i];                        
                    }
                    else
                    {                        
                        return musicalNotes[i];                        
                    }
                }
            }
        }

        return null;

    }    

    private void PitchCalc()
    {
        //Mic detects sound
        bool isDetecting = false;        
        float dB = 0.0f;
        for (int i = 0; i < samples.Length;i++)
        {
            dB += samples[i];
        }
        dB /= freqperBand;
        dB *= 100;

        if (dB > 0.1)
        {
            isDetecting = true;
        }
        else
        {
            isDetecting = false;            
        }
        dt += Time.deltaTime;
        if (isDetecting)
        {
            dt = 0.0f;
        }

        if (dt < 0.5)
        {                                   
            //find the highest freqband peak
            float maxVal = -1;
            int maxIndex = -1;
            for (int i = 0; i < samples.Length; ++i)
            {
                float v = samples[i];
                if (v > maxVal)
                {
                    maxVal = v;
                    maxIndex = i;
                }
            }

            //Calculate fundamental frequency
            float freq = maxIndex * freqperBand + maxVal * freqperBand;

            //This are the frequencies of the piano
            if (freq > 27.5f && freq < 4186.0f)
            {
                pitchDisplay.text = freq.ToString() + maxIndex.ToString();
                //frequencyOutput = freq;
            }
        }
        else
        {
            pitchDisplay.text = "Pitch";
        }
               
    }


    private void AvgPitchCalc()
    {
        //Mic detects sound
        bool isDetecting = false;
        
        float dB = 0.0f;
        for (int i = 0; i < samples.Length; i++)
        {
            dB += samples[i];
        }
        dB *= 100;
        //TODO only detect one window
        //TODO 2 detect the highest window in a period of time
        if (dB > 3.0f)
        {
            //Debug.Log(dB);
            if (dB > storeddB)
            {
                
                storeddB = dB;
            }
            else
            {
                storeddB = 0.0f;
                isDetecting = true;
                
            }
            
            
        }
        else
        {
            isDetecting = false;
            NotesInteractionHandler.noteID = -1;
        }


        dt2 += Time.deltaTime;

        if (isDetecting)// && dt2 > 0.5) //seconds mic will stay shutdown after a detection
        {
            dt2 = 0.0f;
            //Debug.Log(dB + "detected");

            //Checking all the samples
            for (int i = 0; i < samples.Length; ++i)
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

                    //Old code
                    for (int j = 0; j < samplesStored.Length; j++)
                    {                        
                        //if (v > samplesStored[j].Value)
                        //{
                        //    if (j+1 < samplesStored.Length && samplesStored[j].Value != 0)
                        //    {
                        //        samplesStored[j + 1] = samplesStored[j];
                        //    }                            
                        //    samplesStored[j].Index = i;
                        //    samplesStored[j].Value = v;
                        //    samplesStored[j].Freq = i * freqperBand;                            
                        //    //sampleStoredFreq.Add(i * freqperBand);   
                        //    break;
                        //}                        
                    }                       
                }              
            }

            float finalFreq = 4186.0f;

            float[] samplesSavedFreq = new float[10];

            if (samplesStored[0].Freq > 27.5f) //we use 27.5hz since it's the lowest frequency a piano can make.
            {
                FFTFreqBand itVal = new FFTFreqBand(0.0f, 0.0f, 0.0f);
                //Apply a noise filter
                for (int i = 0; i < samplesStored.Length;i++)
                {
                    itVal = samplesStored[i];
                    for (int j = 0; j < samplesStored.Length; j++)
                    {                        
                        if (i != j)
                        {
                            //Detect if it's an harmonic
                            if (itVal.Index + 1 == samplesStored[j].Index || itVal.Index-1 == samplesStored[j].Index)
                            {                                
                                //Who has higher dB?
                                if (itVal.Value > samplesStored[j].Value)
                                {
                                    //float valueDif = itVal.Value * 100.0f - samplesStored[j].Value * 100.0f;
                                    //float removeFreq = valueDif * freqperBand/2;
                                    //itVal.Freq -= removeFreq;

                                    //float valueDif = 1.0f - (samplesStored[i].Value*100.0f);
                                    //float freqRest = valueDif * freqperBand;
                                    //itVal.Freq += freqRest;
                                    samplesStored[j] = new FFTFreqBand(0.0f,0.0f,0.0f);
                                }
                            }
                            
                        }
                        
                        
                    }
                    samplesStored[i] = itVal;
                }
                

                //Get the most lower pitch frequency as a fundamental frequency
                for (int i = 0; i < samplesStored.Length; i++)
                {
                    samplesSavedFreq[i] = samplesStored[i].Freq;
                }
                
                for (int i = 0; i < samplesSavedFreq.Length; i++)
                {
                    if (samplesSavedFreq[i] > 0.0f && samplesSavedFreq[i] < finalFreq)
                    {
                        finalFreq = samplesSavedFreq[i];
                    }
                }
                frequencyOutput = finalFreq;

                //This are the frequencies of the piano 27.5hz to 4186.0hz
                if (finalFreq > 0.0f && finalFreq < 4186.0f)
                {
                    avgPitchDisplay.text = finalFreq.ToString();
                }

                
            }
            finalFreq = 4186.0f;
            isDetecting = false;
        }
        else
        {
            //avgPitchDisplay.text = "AVGPitch";            
            samplesStored = new FFTFreqBand[10];
            
        }

    }
    void InsertArray(FFTFreqBand[] sampleArr, int size, FFTFreqBand sample)
    {        
        List<FFTFreqBand> store = new List<FFTFreqBand>();
        for (int i = 0;i <sampleArr.Length;i++)
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
                for (int j = i;j<size;j++)
                {
                    //As long as it's not empty
                    if (sampleArr[j].Value > 0.0f)
                    {
                        store.Add(sampleArr[j]);
                    }
                    else
                    {
                        break;
                    }
                    
                }
                break;
            }


        }
        int count = 0;
        foreach(FFTFreqBand ret in store)
        {
            if (count < size)
            {
                sampleArr[count] = ret;
                count++;
            }
            else
            {
                break;
            }
        }               
    }

    void OnGUI()
    {
        //String str = savedVals[0].index.ToString() + "_" + savedVals[1].index.ToString() + "_" + savedVals[2].index.ToString();
        //GUILayout.TextArea(str);
        if (GUILayout.Button("A"))
        {

        }

    }

    private void DebugAudioFile()
    {
        Debug.Log(samples.ToString());
        float samplespersecond = audioClip.samples / audioClip.length;
        Debug.Log(samplespersecond.ToString());
        Debug.Log(audioClip.samples.ToString());
    }

    private void MakeFrequencyBands()
    {
        /*
         * 22050 / 512 = 48 hertz
         * 20 - 60 
         * 60-250
         * 250-500
         * 500-2000
         * 2000-400
         * 4000-6000
         * 6000-20000
         * 
         * 0-2 = 86 hz
         * 1-4 = 172 hz
         * 2-8 = 344 hz
         * 3-16 = 688 hz
         * 4-32 = 1376 hz
         * 5-64 = 2752 hz
         * 6-128 = 5504 hz
         * 7-256 = 11008 hz
         * 
        */

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1);
                count++;
            }
            average /= count;
            freqBand[i] = average * 30;
        }

    }

    void GetSpectrumAudioSource()
    {
        //We are using hamming window because it reduces the sidelobes.
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
    }
}
