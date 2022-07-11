using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    private float _movinguptime;
    private float _movingdowntime;

    private void Update()
    {
        _movinguptime += Time.deltaTime;

        if (_movinguptime <= 1f)
        {
            MovingUp();
            _movingdowntime = 0f;
        }
        else
        {
            MovingDown();
            _movingdowntime += Time.deltaTime;
            if(_movingdowntime>=1f)
            {
                _movinguptime = 0f;
            }
        }

    }
    public void MovingUp()
    {

        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }
    public void MovingDown()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime);
    }
    
}
