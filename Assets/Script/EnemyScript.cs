using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField]
    private EnemyConfig _configData;
    [SerializeField]
    private int HP;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private EnemyState _state;  
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private float _stateDuration;
    [SerializeField]
    private float _attackDelay;
    [SerializeField]
    private float _idleTime;
    [SerializeField]
    private float _walkingTime;
    [SerializeField]
    private Transform _attackPoint;
    [SerializeField]
    private Vector2 _attackRange;
    [SerializeField]
    private LayerMask _playerLayer;
    [SerializeField]
    private int _damage;
    [SerializeField]
    private float _distanceToPlayer;
    [SerializeField]
    private int _attackCombo;
    [SerializeField]
    private Transform eyepoint;
    [SerializeField]
    private float sightrange;
    [SerializeField]
    private float attackDistance;

    private Vector2 _walkingDirection;
    private bool _isFacingRight;
    private void Start()
    {
        _walkingDirection.x = 1;
        _isFacingRight = true;
        _state = EnemyState.Idle; 
        _player = GameObject.Find("Player").transform;

        HP = _configData.EnemyConfigHP;
        _damage = _configData.EnemyConfigDamage;
        _speed = _configData.EnemyConfigSpeed;
        _stateDuration = _configData.StateDuration;
        _attackDelay = _configData.AttackDelay;
    }
    void FixedUpdate()
    {  
        if (_state == EnemyState.Attack) return;
        if (_state == EnemyState.Die) return;
        if (_state == EnemyState.Hurt) return;
        if (_player.gameObject == null) Patrol();

        EnemySight();
        _distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        if (_distanceToPlayer <= attackDistance && inSight) // player is close
        {
            _speed = 0f;

            _idleTime += Time.deltaTime;
            if (_idleTime <= _attackDelay)
            {
                _animator.SetBool("isWalking", false);
                _state = EnemyState.Idle;
            }
            else
            {
                _state = EnemyState.Attack;
                _animator.SetTrigger("Attack");
                //Attack();
                _idleTime = 0;
            }
        }
        else if ( inSight) // player in range
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

    private void ChasePlayer() // INCLUDING MOVING AND FLIPPING
    {
        _speed = _configData.EnemyConfigSpeed;

        _state = EnemyState.Walk;
        _animator.SetBool("isWalking", true);

        _walkingDirection = (_player.position - transform.position).normalized;
        _walkingDirection.y = 0f;

        if (_player.position.x <= transform.position.x) //=> Player is on the left of the skeleton
        {
            _walkingDirection = Vector2.left;
            transform.Translate(_walkingDirection * _speed * Time.deltaTime);
            if (_isFacingRight) Flip();
        }
        else
        {
            _walkingDirection = Vector2.right;

            transform.Translate(_walkingDirection * _speed * Time.deltaTime);
            if (!_isFacingRight) Flip();
        }
    }

    [SerializeField]
    private bool inSight; 
    float outofsighttime;
    private void EnemySight()
    {
        Debug.DrawRay(eyepoint.position, _walkingDirection * sightrange, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(eyepoint.position, _walkingDirection, sightrange, _playerLayer);
        if (hit && hit.collider.CompareTag("Player"))
        {
            inSight = true;
            outofsighttime = 0f;
        }
        else
        {
            outofsighttime += Time.deltaTime;
            if (outofsighttime >= 2f)
            {
                inSight = false;
            }
        }
    }
    void Patrol()
    {
        _speed = _configData.EnemyConfigSpeed;
        _walkingTime += Time.deltaTime;
        if (_walkingTime >= _stateDuration)
        {
            _state = EnemyState.Idle;
            _animator.SetBool("isWalking", false);
            _idleTime += Time.deltaTime;
            if (_idleTime >= _stateDuration) _walkingTime = 0f;
        }
        else
        {
            transform.Translate(_speed * _walkingDirection * Time.deltaTime);
            _state = EnemyState.Walk;
            _animator.SetBool("isWalking", true); _idleTime = 0f;
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TurnPoint"))
        {
            _walkingDirection = (transform.position - collision.transform.position).normalized;
            _walkingDirection.y = 0f;
            if (_walkingDirection.x < 0)
            {
                _walkingDirection = Vector2.left;
                Flip();
            }
            else
            {
                _walkingDirection = Vector2.right;
                Flip();
            }
        }
       
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyWall"))
        {
            inSight = false;
            _walkingDirection = (transform.position - collision.transform.position).normalized;
            _walkingDirection.y = 0f;
            if (_walkingDirection.x < 0)
            {
                _walkingDirection = Vector2.left;
                Flip();
            }
            else
            {
                _walkingDirection = Vector2.right;
                Flip();
            }
        }
    }
    void Flip()
    {
        _isFacingRight = !_isFacingRight;
        var currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
    bool takedamage;
    public void TakeDamage(int damage)
    {
        if (_state == EnemyState.Die) return;
        
        HP -= damage;
        _state = EnemyState.Hurt;
        _animator.SetTrigger("Hurt");
        takedamage = true;
        if(HP<=0)
        {
            _state = EnemyState.Die;
            _animator.SetTrigger("Die");
        }
    }
    public void On_Hurt_End()
    {
        _state = EnemyState.Idle;
        _animator.SetBool("isWalking", true);
    }
    public void Die()
    {      
        Destroy(this.gameObject);
    }
    public void On_Attack_End(int value)
    {
        _state = EnemyState.Idle;
        _animator.SetBool("isWalking", false);
        _attackCombo = value;
        _animator.SetInteger("AttackCombo", _attackCombo);
    }
private enum EnemyState
{
    None,
    Walk,
    Idle,
    Attack,
    Hurt,
    Die,
}
}
