using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBossZone : MonoBehaviour
{
    [SerializeField]
    private BossCanvas _canvas;
    [SerializeField]
    private AudioSource _audio;
    [SerializeField]
    private AudioSource _bgm;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _canvas.gameObject.SetActive(true);
            _audio.Play();
            _bgm.Stop();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
