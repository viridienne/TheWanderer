using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStage : MonoBehaviour
{

    [SerializeField]
    private AudioSource _audioSouce1;
    [SerializeField]
    private AudioSource _audioSouce2;
    [SerializeField]
    private Canvas _demonCanvas;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _audioSouce1.Stop();
            _audioSouce2.Play();
            _demonCanvas.gameObject.SetActive(true);
        }
    }
}
