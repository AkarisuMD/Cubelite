using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixerGame;
    public AudioMixer audioMixerMusique;

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown FrequencyDropdown;

    Resolution[] resolutions;
    List<Resolution> _resolutions;
    List<int> HZ;

    void Start()
    {
        resolutions = Screen.resolutions;

        List<string> options = new List<string>();
        List<string> hz = new List<string>();
        HZ = new List<int>();
        _resolutions = new List<Resolution>();

        int currentResolutionIndex = 0;
        int currentFrequencyIndex = Screen.currentResolution.refreshRate;
        int y = -1;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            string _hz = resolutions[i].refreshRate + " hz";
            
            if (!options.Contains(option))
            {
                y++;
                options.Add(option);
                _resolutions.Add(resolutions[i]);
                
            }

            if (resolutions[i].Equals(Screen.currentResolution))
                currentResolutionIndex = y;


            if (!HZ.Contains(resolutions[i].refreshRate))
            {
                HZ.Add(resolutions[i].refreshRate);
                hz.Add(_hz);
            }

            Debug.Log(option);
            
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        FrequencyDropdown.ClearOptions();
        FrequencyDropdown.AddOptions(hz);
        FrequencyDropdown.value = currentFrequencyIndex;
        FrequencyDropdown.RefreshShownValue();
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, Screen.currentResolution.refreshRate);
    }
    
    public void SetFrequency(int FrequencyIndex)
    {
        int frequency = HZ[FrequencyIndex];
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen, frequency);
    }
    public void SetVolume (float volume)
    {
        audioMixerGame.SetFloat("GameVolume", volume);
        Debug.Log(volume);
    }
    public void SetMusiqueVolume(float volume)
    {
        audioMixerMusique.SetFloat("MusiqueVolume", volume);
        Debug.Log(volume);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
