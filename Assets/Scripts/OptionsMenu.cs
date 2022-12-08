using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public AudioMixer audioMixer;
    public Slider slider;
    
    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volume", 0.75f);
    }

    public void SetVolume()
    {
        float sliderValue = slider.value;
        audioMixer.SetFloat("volume", Mathf.Log10 (sliderValue) * 20);
        PlayerPrefs.SetFloat("volume", sliderValue);
    }
}
