using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private float _speed;
    private float _movinguptime;
    private float _movingdowntime;
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private Vector2 _flyingDirection;
    [SerializeField]
    private Transform _bulletPrefab;
    [SerializeField]
    private Transform _attackPoint;
    [SerializeField]
    private float _flyTime;
    [SerializeField]
    private float _shootDelay;
    [SerializeField]
    private float _idleTime;
    [SerializeField]
    private GhostState _state;
    [SerializeField]
    private int HP;
    [SerializeField]
    private Transform eyepoint;
    [SerializeField]
    private float sightrange;
    [SerializeField]
    private LayerMask _playerLayer;

    private bool _isFacingRight;

    private void Start()
    {
        _isFacingRight = false;
        _state = GhostState.Fly; 
        _player = GameObject.Find("Player").transform;

    }
    private void Update()
    {

        if (_state == GhostState.Die) return;
        if (_state == GhostState.Hurt) return;
        EnemySight();
        var _distanceToPlayer = Vector2.Distance(transform.position, _player.position);

        if (inSight) // player is close
        {
            if (transform.position.x > _player.position.x) // ghost is on the right of player => must be facing left
            {
                if (_isFacingRight) Flip();
            }
            else
            {
                if (!_isFacingRight) Flip();
            }

            _animator.SetBool("isGlowing", true);
            _idleTime += Time.deltaTime;
            if (_idleTime <= 0.1f)
            {
                Instantiate(_bulletPrefab, _attackPoint.position, transform.rotation,transform);
                _flyTime = 0f;
            }
            else
            {
                _flyTime += Time.deltaTime;
                if(_flyTime<=_shootDelay)
                {
                    Flying();
                }
                else _idleTime = 0f;
            }
        }
        else
        {
            _animator.SetBool("isGlowing", false);
            if (takedamage) Flip();
            Flying();
            takedamage = false;
        }
    }
    [SerializeField]
    private bool inSight;
    float outofsighttime;
    Vector2 direction;
    private void EnemySight()
    {
        if (_isFacingRight)
        {
            direction = Vector2.right;
        }
        else direction = Vector2.left;
        Debug.DrawRay(eyepoint.position, direction * sightrange, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(eyepoint.position, direction, sightrange, _playerLayer);
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
    bool takedamage;
    public void TakeDamage(int damage)
    {
        if (_state == GhostState.Die) return;
        HP -= damage;
        _state = GhostState.Hurt;
        _animator.SetTrigger("Hurt");
        takedamage = true;
        if (HP <= 0)
        {
            _state = GhostState.Die; _animator.SetTrigger("Die");
        }
    }
    void Die()
    {

        Destroy(this.gameObject);
    }
    void Flip()
    {
        _isFacingRight = !_isFacingRight;
        var currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
    public void Flying()
    {
        _movinguptime += Time.deltaTime;

        if (_movinguptime <= 1.5f)
        {
            MovingUp();
            _movingdowntime = 0f;
        }
        else
        {
            MovingDown();
            _movingdowntime += Time.deltaTime;
            if (_movingdowntime >= 1.5f)
            {
                _movinguptime = 0f;
            }
        }
    }
    public void MovingUp()
    {
        _flyingDirection = Vector2.up;
        transform.Translate(_flyingDirection * _speed * Time.deltaTime);
    }
    public void MovingDown()
    {
        _flyingDirection = Vector2.down;

        transform.Translate(_flyingDirection * _speed * Time.deltaTime);
    }
    public void On_Hurt_End()
    {
        _state = GhostState.Fly;
        _animator.SetBool("isGlowing", true);
    }
    enum GhostState
    {
        Fly,
        Glow,
        Hurt,
        Die,
    }
}
