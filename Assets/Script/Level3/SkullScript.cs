using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullScript : MonoBehaviour
{
    [SerializeField]
    private EnemyConfig _configData;
    [SerializeField]
    private int HP;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private SkullState _state;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private float _attackDelay;
    [SerializeField]
    private float _attackTime;
    [SerializeField]
    private float _attackStop;
    [SerializeField]
    private Transform _attackPoint;
    [SerializeField]
    private Vector2 _attackRange;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private LayerMask _playerLayer;


    private Vector2 _direction;
    [SerializeField]
    private float _walkingTime;
    [SerializeField]
    private float _stateDuration;
    [SerializeField]
    private float _idleTime;

    private float _distanceToPlayer;

    bool _isFacingRight;
    private void Start()
    {
        _direction = Vector2.left;
        _isFacingRight = false;
        _animator.SetBool("isMoving", false);
        _state = SkullState.Idle;
        _player = GameObject.Find("Player").transform;

        _speed = _configData.EnemyConfigSpeed;
        HP = _configData.EnemyConfigHP;
        _stateDuration = _configData.StateDuration;
        _attackDelay = _configData.AttackDelay;

    }
    private void Update()
    {
        if (_state == SkullState.Die) return;
        if (_state == SkullState.Hurt) return;
        if (_player.gameObject == null) Patrol();
        Patrol(); 
        //if (_state == SkullState.Move)
        //{
        //    AttackCycle();
        //}
    }

    bool attacked;
    private void AttackCycle()
    {
        _attackTime += Time.deltaTime;
        if (_attackTime <= _attackDelay)
        {
            if (!attacked)
            {
                Attack();
                _attackStop = 0f;
                attacked = true;
            }
        }
        else
        {
            attacked = false;
            _attackStop += Time.deltaTime;
            if (_attackStop >= _attackDelay)
            {
                _attackTime = 0f;
            }
        }
    }
    private void Attack()
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

    void Patrol()
    {
        _speed = _configData.EnemyConfigSpeed;
        _walkingTime += Time.deltaTime;
        if (_walkingTime >= _stateDuration)
        {
            _state = SkullState.Idle;
            _animator.SetBool("isMoving", false);
            _idleTime += Time.deltaTime;
            if (_idleTime >= _stateDuration/2) _walkingTime = 0f;
        }
        else
        {
            transform.Translate(_speed * _direction * Time.deltaTime);
            _state = SkullState.Move;
            _animator.SetBool("isMoving", true); _idleTime = 0f;
        }

    }
    private void ChasePlayer() // INCLUDING MOVING AND FLIPPING
    {
        _speed = _configData.EnemyConfigSpeed;

        _state = SkullState.Move;
        _animator.SetBool("isMoving", true);

        _direction = (_player.position - transform.position).normalized;
        _direction.y = 0f;

        if (_player.position.x <= transform.position.x) //=> Player is on the left of the skeleton
        {
            _direction = Vector2.left;
            transform.Translate(_direction * _speed * Time.deltaTime);
            if (_isFacingRight) Flip();
        }
        else
        {
            _direction = Vector2.right;

            transform.Translate(_direction * _speed * Time.deltaTime);
            if (!_isFacingRight) Flip();
        }
    }
    void Flip()
    {
        _isFacingRight = !_isFacingRight;
        var currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;

    }
    public void TakeDamage(int damage)
    {
        if (_state == SkullState.Die) return;

        HP -= damage;
        _state = SkullState.Hurt;
        _animator.SetTrigger("Hurt");
        if (HP <= 0)
        {
            _state = SkullState.Die;
            _animator.SetTrigger("Die");
        }
    }
    public void On_Hurt_End()
    {
        _state = SkullState.Idle;
        _animator.SetBool("isMoving", false);
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TurnPoint"))
        {
            _direction = (transform.position - collision.transform.position).normalized;
            _direction.y = 0f;
            if (_direction.x < 0)
            {
                _direction = Vector2.left;
               if(_isFacingRight) Flip();
            }
            else
            {
                _direction = Vector2.right;
                if(!_isFacingRight) Flip();
            }
        }
      
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_state == SkullState.Move) AttackCycle();
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null) return;
        Gizmos.DrawWireCube(_attackPoint.position, _attackRange);
    }
}
    enum SkullState
{
        Idle,
        Move,
        Hurt,
        Die,
}
