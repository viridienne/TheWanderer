using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiseSkeleton : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private EnemyConfig _configData;

    private Vector2 _walkingDirection;
    private bool _isFacingRight;
    [SerializeField]
    private float _distanceToPlayer;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Transform eyepoint;
    [SerializeField]
    private float sightrange;
    [SerializeField]
    private LayerMask _playerLayer;

    [SerializeField]
    private int HP;
        

    [SerializeField]
    private Transform _player;
    [SerializeField]
    private EnemyState _state;
    [SerializeField]
    private AudioSource _audiosouce;
    [SerializeField]
    private AudioClip _riseSFX;
    [SerializeField]
    private float _walkingTime;
    [SerializeField]
    private float _stateDuration;
    [SerializeField]
    private float _idleTime;

    // Start is called before the first frame update
    void Start()
    {
        _walkingDirection.x = -1f;
        _isFacingRight = false;
        _animator.SetBool("isWalking", false);
        _state = EnemyState.Rise;
        _player = GameObject.Find("Player").transform;

        HP = _configData.EnemyConfigHP;
        _speed = _configData.EnemyConfigSpeed;
        _stateDuration = _configData.StateDuration;
    }

    // Update is called once per frame
    void Update()
    {
        EnemySight();
        if (_state == EnemyState.Die) return;
        if (_state == EnemyState.Hurt) return;
        if (_state == EnemyState.Rise) return;
        if (_player.gameObject == null) Patrol();

        _distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        if (_distanceToPlayer <= 0.3 && inSight) // player is close
        {
            _speed = 0f;

            _animator.SetBool("isWalking", false);
            _state = EnemyState.Idle;

        }
        else if (inSight)  // player in range
        {
            LV2ChasePlayer();
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
    void Flip()
    {
        _isFacingRight = !_isFacingRight;
        var currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
    private void LV2ChasePlayer() // INCLUDING MOVING AND FLIPPING
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
            _animator.SetBool("isWalking", true);
            _idleTime = 0f;
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
        if(collision.gameObject.CompareTag("EnemyWall"))
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
    public void Rise_End()
    {
        _state = EnemyState.Idle;
        _animator.SetBool("isWalking", false);

    }
    bool takedamage;
    public void TakeDamage(int damage)
    {
        if (_state==EnemyState.Die) return;

        HP -= damage;
        takedamage = true;
        _state = EnemyState.Hurt;
        _animator.SetTrigger("Hurt");

        if (HP <= 0)
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
    public void PlayRiseSFX()
    {
        _audiosouce.PlayOneShot(_riseSFX);
    }

enum EnemyState
{
    Rise,
    Walk,
    Idle,
    Hurt,
    Die, 
}
}
