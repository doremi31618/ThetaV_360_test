using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioReader : MonoBehaviour {
    [HideInInspector]
    public static float[] _AudioSamples = new float[512];
    AudioSource _audioSource;

	// Use this for initialization
	void Start () {
        _audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        readingSpectrumAudioSource();
	}
    void readingSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_AudioSamples, 0, FFTWindow.Blackman);
    }
}
