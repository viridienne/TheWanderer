using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonScript : MonoBehaviour
{
    [SerializeField]
    private EnemyConfig _configData;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private DemonState _state;
    [SerializeField]
    public int HP;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _stateDuration;
    [SerializeField]
    private float _idleTime;
    [SerializeField]
    private float _patrolTime;
    [SerializeField]
    private float _attackDelay;
    [SerializeField]
    private Transform _attackPoint;
    [SerializeField]
    private Vector2 _attackRange;
    [SerializeField]
    private LayerMask _playerLayer;
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _roarSFX;

    [SerializeField]
    private Transform eyepoint;
    [SerializeField]
    private float sightrange;
    [SerializeField]
    private float _distanceToPlayer;

    bool _isFacingRight;
    Vector2 _direction;
    private void Start()
    {
        _state = DemonState.Idle;
        _direction = Vector2.left;
        _isFacingRight = false;
        _player = GameObject.Find("Player").transform;

        HP = _configData.EnemyConfigHP;
        _damage = _configData.EnemyConfigDamage;
        _speed = _configData.EnemyConfigSpeed;
        _stateDuration = _configData.StateDuration;
        _attackDelay = _configData.AttackDelay;
    }
    private void Update()
    {
   
    }
    private void FixedUpdate()
    {
        if (_player == null) return;
        if (_state == DemonState.Die) return;
        if (_state == DemonState.Attack) return;
        if (_state == DemonState.Hurt) return;

        EnemySight();
        _distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        if (_distanceToPlayer <= 0.5f && inSight) // player is close
        {
            _speed = 0f;

            _idleTime += Time.deltaTime;
            if (_idleTime <= _attackDelay)
            {
                _state = DemonState.Idle;
                _animator.SetBool("isFlying", true);
            }
            else
            {
                _state = DemonState.Attack;
                _animator.SetTrigger("Attack");
                _animator.SetBool("isFlying", false);
                _idleTime = 0f;
            }
        }
        else if (inSight) // player in range
        {
            ChasePlayer();
        }
        else
        {
            if (takedamage)
            {
                Flip();
                ChasePlayer();
            }
            Patrol();
            takedamage = false;
        }
    }
    [SerializeField]
    private bool inSight;
    float outofsighttime;
    private void EnemySight()
    {
        Debug.DrawRay(eyepoint.position, _direction * sightrange, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(eyepoint.position, _direction, sightrange, _playerLayer);
        if (hit && hit.collider.CompareTag("Player"))
        {
            inSight = true;
            outofsighttime = 0f;
        }
        else
        {
            outofsighttime += Time.deltaTime;
            if (outofsighttime >= 1.5f)
            {
                inSight = false;
            }
        }
    }
    private void ChasePlayer() // INCLUDING MOVING AND FLIPPING
    {
        _speed = _configData.EnemyConfigSpeed;
        _state = DemonState.Moving;
        _animator.SetBool("isFlying", true);

        _direction = (_player.position - transform.position).normalized;
        _direction.y = 0f;

        if (_player.position.x <= transform.position.x) //=> Player is on the left of the skeleton
        {
            TurnLeft();
        }
        else
        {
            TurnRight();

        }
    }
    void TurnLeft()
    {
        _direction = Vector2.left;

        transform.Translate(_direction * _speed * Time.deltaTime);
        if (_isFacingRight) Flip();
    }
    void TurnRight()
    {
        _direction = Vector2.right;

        transform.Translate(_direction * _speed * Time.deltaTime);
        if (!_isFacingRight) Flip();
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
    void Flip()
    {
        _isFacingRight = !_isFacingRight;
        var currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    void Patrol()
    {
        _animator.SetBool("isFlying", true);
        _speed = _configData.EnemyConfigSpeed;
        _patrolTime += Time.deltaTime;
        if (_patrolTime >= _stateDuration)
        {
            _state = DemonState.Idle;
            _idleTime += Time.deltaTime;
            if (_idleTime >= _stateDuration) _patrolTime = 0f;
        }
        else
        {
            transform.Translate(_speed * _direction * Time.deltaTime);
            _state = DemonState.Moving;
            _idleTime = 0f;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null) return;
        Gizmos.DrawWireCube(_attackPoint.position, _attackRange);
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
                Flip();
            }
            else
            {
                _direction = Vector2.right;
                Flip();
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyWall"))
        {
            inSight = false;

            _direction = (transform.position - collision.transform.position).normalized;
            _direction.y = 0f;
            if (_direction.x < 0)
            {
                _direction = Vector2.left;
                Flip();
            }
            else
            {
                _direction = Vector2.right;
                Flip();
            }
        }
    }
    bool takedamage;
    public void TakeDamage(int damage)
    {
        if (_state == DemonState.Die) return;
        HP -= damage;
        takedamage = true;
        _state = DemonState.Hurt;
        _animator.SetTrigger("Hurt");
        _animator.SetBool("isFlying", false);
        if (HP <= 0)
        {
            _state = DemonState.Die;
            _animator.SetTrigger("Die");
        }
    }
    public void Die()
    {
        Destroy(this.gameObject);
    }
    public void On_Attack_End(/*int value*/)
    {
        _state = DemonState.Idle;
        _animator.SetBool("isFlying", true);

        //_attackCombo = value;
        //_animator.SetInteger("AttackCombo", _attackCombo);
    }
    public void On_Hurt_End()
    {
        _state = DemonState.Idle;
        _animator.SetBool("isFlying", true);
    }
    public void Roar()
    {
        _audioSource.PlayOneShot(_roarSFX);
    }
    enum DemonState
    {
        Idle,
        Moving,
        Attack,
        Hurt,
        Die,
    }
}
