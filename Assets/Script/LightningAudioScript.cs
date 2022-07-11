using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningAudioScript : MonoBehaviour
{
    [SerializeField]
    private AudioClip _thundersfx;
    [SerializeField]
    private AudioClip _ghostbreathsfx;
    [SerializeField]
    private AudioSource _audiosource;
    public void PlayThunderSFX()
    {
        _audiosource.PlayOneShot(_thundersfx);
    }
    public void PlayScreamSFX()
    {
        _audiosource.PlayOneShot(_ghostbreathsfx);
    }
}
