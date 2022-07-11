using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    [SerializeField]
    private EnemyConfig _configData;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private Transform _attackPoint;
    [SerializeField]
    private Vector2 _attackRange;
    [SerializeField]
    private LayerMask _playerLayer;
    [SerializeField]
    private float _idleTime;
    [SerializeField]
    private float _attackDelay;

    // Start is called before the first frame update
    void Start()
    {
        _damage = _configData.EnemyConfigDamage;
    }

    // Update is called once per frame
    void Update()
    {
        _idleTime += Time.deltaTime;
        if (_idleTime <= _attackDelay)
        {
            return;
        }
        else
        {
            _attackPoint.gameObject.SetActive(true);

            Attack();
            _attackPoint.gameObject.SetActive(false);

            _idleTime = 0;
        }


    }
    public void Attack()
    {

        Collider2D[] enemies = Physics2D.OverlapBoxAll(_attackPoint.position, _attackRange, _playerLayer);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.gameObject.CompareTag("Player"))
            {
                enemy.gameObject.GetComponent<PlayerController>().TakeDamage(_damage);
            }
        }

    }
    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null) return;
        Gizmos.DrawWireCube(_attackPoint.position, _attackRange);
    }
}
