using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBulletScript : MonoBehaviour
{
    [SerializeField]
    private SpellDataConfig _configData;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Transform _ghost;
    [SerializeField]
    private Vector2 _ghostScale;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _aliveTime;
    [SerializeField]
    private float _spellDuration;
    [SerializeField]
    private Vector2 _direction;
    // Start is called before the first frame update
    void Start()
    {
        _speed = _configData.SpellSpeed;
        _damage = _configData.SpellDamage;
        _spellDuration = _configData.SpellDuration;
    }

    // Update is called once per frame
    void Update()
    {
        _ghost = GetComponentInParent<Transform>();
        _ghostScale = _ghost.transform.localScale;
        if (_ghostScale.x == -1)
        {
            _direction = Vector2.left;
        }
        else if (_ghostScale.x == 1)
        {
            _direction = Vector2.right;
        }
        this.gameObject.transform.parent = null;
        _aliveTime += Time.deltaTime;

        transform.Translate(_direction * _speed * Time.deltaTime);
          
        if(_aliveTime>=_spellDuration)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(_damage);
        }
    }
}
