using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [SerializeField]
    private Slider _volumeSFX;
    [SerializeField]
    private Slider _volumeBGM;
    [SerializeField]
    private string _BGMParamName;
    [SerializeField]
    private string _SFXParamName;
    [SerializeField]
    private AudioMixerGroup _audioMixer;

    private void Start()
    {
        SetUp();
    }
    private void Update()
    {
        OnBGMSliderValueChanged();
        OnSFXSliderValueChanged();
    }
    public void SetUp()
    {
        _volumeBGM.value = PlayerPrefs.GetFloat("VolumeBGM");
        _audioMixer.audioMixer.SetFloat(_BGMParamName, _volumeBGM.value);
        _volumeSFX.value = PlayerPrefs.GetFloat("VolumeSFX");
        _audioMixer.audioMixer.SetFloat(_SFXParamName, _volumeSFX.value);

    }
    public void OnBGMSliderValueChanged()
    {
        _audioMixer.audioMixer.SetFloat(_BGMParamName, _volumeBGM.value);
        PlayerPrefs.SetFloat("VolumeBGM", _volumeBGM.value);

    }
    public void OnSFXSliderValueChanged()
    {
        _audioMixer.audioMixer.SetFloat(_SFXParamName, _volumeSFX.value);
        PlayerPrefs.SetFloat("VolumeSFX", _volumeSFX.value);
    }
}
