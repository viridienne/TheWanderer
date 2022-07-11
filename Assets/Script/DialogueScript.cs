using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour
{
    [SerializeField]
    private Image visualCue;
    [SerializeField]
    private PlaySceneCanvas _canvas;


    bool triggerCue;
    bool inRange;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (triggerCue)
        {
            visualCue.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                _canvas.EnableDialoguePnl();
            }
        }
        else
        {
            visualCue.gameObject.SetActive(false);
            _canvas.DisableDialoguePnl();
        }

    }
    // Update is called once per frame

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            triggerCue = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            triggerCue = false;
        }
    }
}
