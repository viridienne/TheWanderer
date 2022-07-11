using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform _necromancer;
    [SerializeField]
    private PlaySceneCanvas _canvas;
    [SerializeField]
    private AudioSource _audiosource;
    [SerializeField]
    private AudioClip _churchsound;
    [SerializeField]
    private AudioSource _bgm;

    private void Start()
    {
        _necromancer = GameObject.Find("Necromancer").gameObject.transform;
    }
    private void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (_necromancer != null) _canvas.NeedToSlayMessage();
            else if(_necromancer==null)
            {
                _bgm.Pause();
                _audiosource.PlayOneShot(_churchsound);
                GameManager.Instance.ingameData._isLevelCompleted = true;
                _canvas.EndgamePnlEnable();
            }
        }
      
    }
}
