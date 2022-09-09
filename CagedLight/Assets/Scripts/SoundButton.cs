using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButton : MonoBehaviour
{
    private bool isSoundOn = true;
    [SerializeField] private GameObject soundOn;
    [SerializeField] private GameObject soundOff;

    private void Start()
    {
        if (PlayerPrefs.HasKey("sound"))
        {
            isSoundOn = Convert.ToBoolean(PlayerPrefs.GetInt("sound"));
        }
        else // there's no sound settings in PlayerPrefs which means it's the first time user opened the game
        {
            // we initiate PlayerPrefs and set sound settings to 1
            PlayerPrefs.SetInt("sound", 1);
            isSoundOn = true;
        }
        Debug.Log(isSoundOn);
    }

    private void Update() {
        if (isSoundOn)
        {
            soundOn.SetActive(true);
            soundOff.SetActive(false);
        }
        else
        {
            soundOn.SetActive(false);
            soundOff.SetActive(true);
        }
    }

    public void ToogleSound()
    {
        if (isSoundOn)
        {
            isSoundOn = false; // turn off the sound            
        }
        else
        {
            isSoundOn = true; // turn on the sound            
        }
        
        PlayerPrefs.SetInt("sound", Convert.ToInt32(isSoundOn));
    }
}
