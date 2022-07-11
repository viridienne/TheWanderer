using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
public class CheckPointScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private bool _isFading;
    [SerializeField]
    private float _fadeTime;

    // Start is called before the first frame update
    void Start()
    {
        _text.CrossFadeAlpha(0, 0f, false);
        _isFading = false;
        _fadeTime = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (_isFading) FadeIn();
        else if (_text.color.a != 0)
        {
            _text.CrossFadeAlpha(0, 0.5f, false);
        }
    }
    void FadeIn()
    {
        _text.CrossFadeAlpha(1f, 0.5f, false);
        _fadeTime += Time.deltaTime;
        if (_fadeTime > 1.5f && _text.color.a == 1)
        {
            _isFading = false; // complete fade
            _fadeTime = 0f; // reset fade time
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isFading = true;
        }
    }
}
