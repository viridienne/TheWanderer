using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScript : MonoBehaviour
{
    [SerializeField]
    private SpellDataConfig _configData;
    [SerializeField]
    private Transform _attackPoint;
    [SerializeField]
    private Vector2 _attackRange;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private LayerMask _playerLayer;

    private void Start()
    {
        _damage = _configData.SpellDamage;
    }
    public void Attack()
    {
        _attackPoint.gameObject.SetActive(true);
        Collider2D[] enemies = Physics2D.OverlapBoxAll(_attackPoint.position, _attackRange, _playerLayer);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.gameObject.CompareTag("Player"))
            {
                enemy.gameObject.GetComponent<PlayerController>().TakeDamage(_damage);
            }
        }
        _attackPoint.gameObject.SetActive(false);
    }
    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null) return;
        Gizmos.DrawWireCube(_attackPoint.position, _attackRange);
    }

    public void End()
    {
        Destroy(this.gameObject);
    }
}
