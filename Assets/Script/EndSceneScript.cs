using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneScript : MonoBehaviour
{
    [SerializeField]
    private PlayerController _playerscript;
    [SerializeField]
    private PlaySceneCanvas _canvas;
    [SerializeField]
    private GameObject[] enemy1;    
    [SerializeField]
    private GameObject[] enemy2;
    [SerializeField]
    private GameObject[] enemy3;
    [SerializeField]
    private GameObject[] enemy4;
    [SerializeField]
    private Scene _activeScene;

    int activescene;
    private void Start()
    {
        _activeScene = SceneManager.GetActiveScene();
        activescene = _activeScene.buildIndex;
        _playerscript = FindObjectOfType<PlayerController>();
    }
    private void Update()
    {
        enemy3 = GameObject.FindGameObjectsWithTag("Demon");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_activeScene.name == "PlayScene1")
            {
                if (_playerscript.KeyObtained >= _playerscript.NeededKey)
                {
                    GameManager.Instance.MoveToNextLevel();
                }
                else
                {
                    _canvas.MessageBoard();

                }
            }
            else
            {
                if (_playerscript.KeyObtained >= _playerscript.NeededKey && enemy3.Length == 0)                 
                {
                    GameManager.Instance.MoveToNextLevel();
                }
                else
                {
                    _canvas.MessageBoard();

                }
            }
        }

    }
}
