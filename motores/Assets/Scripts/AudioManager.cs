using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public AudioSource musicAudio;
    public AudioSource FXAudio;
    public Slider sliderVolume;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sliderVolume.onValueChanged.AddListener(ChangeVolumeMusic);
    }

    public static AudioManager Get()
    {
        return instance;
    }

    void ChangeVolumeMusic(float volumen)
    {
        Debug.Log(volumen);
        musicAudio.volume = volumen;
    }
}
