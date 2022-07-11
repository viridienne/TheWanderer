using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerScript : MonoBehaviour
{
    [SerializeField]
    private EnemyConfig _configData;
    [SerializeField]
    private int _HP;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform lightningprefab;
    [SerializeField]
    private Transform darkmagicprefab;
    [SerializeField]
    private Transform skeletonprefab;
    [SerializeField]
    private State _state;

    [SerializeField]
    private Transform spawnpos1;
    [SerializeField]
    private Transform spawnpos2;

    [SerializeField]
    private float _attackTime;
    [SerializeField]
    private float _attackDelay;
    [SerializeField]
    private bool isCreated;
    [SerializeField]
    private bool isSpawn;
    [SerializeField]
    private int specialatk=0;
    [SerializeField]
    private AudioSource _audiosource;
    [SerializeField]
    private AudioSource bgm;    
    [SerializeField]
    private AudioSource stageaudio;
    [SerializeField]
    private AudioSource _BossDeathSFX;

    [SerializeField]
    private AudioClip _castingSFX;

    Vector2 pos;
    float distance;
    bool _isFacingRight;

    // PROPERTY
    public int HP { get=>_HP;}
    //===========================

    // Start is called before the first frame update
    void Start()
    {
        _isFacingRight = true;
        _HP = _configData.EnemyConfigHP;
        _attackDelay = _configData.AttackDelay;
        specialatk = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_state == State.Die) return;
        if (_state == State.Teleport) return;
        if (_state == State.Hurt) return;
        if (_state == State.Attack) return;

        if (_HP <= 45 && specialatk < 1)
        {
            animator.SetTrigger("SpecialAttack");
            specialatk += 1;
        }

        distance = Vector2.Distance(player.position, transform.position);

        if (player.position.x <= transform.position.x) //=> Player is on the left
        {
            if (_isFacingRight) Flip();
        }
        else
        {

            if (!_isFacingRight) Flip();
        }

        if (distance <= 1) // TOO CLOSE => TELEPORT
        {
            _state = State.Teleport;
            animator.SetTrigger("Teleport");
            pos = transform.position;
            RandomPosition();
            if (Vector2.Distance(pos, player.transform.position) <= 1.5)
            {
                if (transform.position.x < 50.3) pos.x = player.transform.position.x + 1;
            }
            _state = State.Attack;
            animator.SetTrigger("Attack");
        }
        else if (distance <= 6)
        {
            _attackTime += Time.deltaTime;
            if (_attackTime >= _attackDelay)
            {
                _state = State.Attack;
                animator.SetTrigger("Attack");
                _attackTime = 0f;
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }
    void Flip()
    {
        _isFacingRight = !_isFacingRight;
        var currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    void Attack()
    {
        var pos = player.position;
        pos.y = transform.position.y;

        if (!isCreated)
        {
            var skill = Random.Range(1, 3);

            switch (skill)
            {
                case 1: Instantiate(lightningprefab, pos, lightningprefab.rotation); break;
                case 2: Instantiate(darkmagicprefab, pos, darkmagicprefab.rotation); break;
                default:break;
            }
            isCreated = true;
        }
    }
    void SpecialAttack()
    {
        if(!isSpawn)
        {
            Instantiate(skeletonprefab, spawnpos1.position, skeletonprefab.rotation);
            Instantiate(skeletonprefab, spawnpos2.position, skeletonprefab.rotation);
        }
        isSpawn = true;
    }

    int poise;
    public void TakeDamage(int damage)
    {
        if (_state==State.Die) return;
        _HP -= damage;
        poise += 10;
        if (poise >= 30)
        {
            _state = State.Hurt;
            animator.SetTrigger("Hurt");
            poise = 0;
        }

        if (_HP <= 0)
        {
            _state = State.Die;
            animator.SetTrigger("Die");
        }
    }
    public void Die()
    {
        stageaudio.Stop();
        _BossDeathSFX.Play();

        bgm.Play();
        Destroy(gameObject);
    }
    public void TeleportEnd()
    {
        transform.position = pos;
        animator.ResetTrigger("Teleport");
        animator.SetBool("isRunning", false); _state = State.Idle;

    }
    void RandomPosition()
    {
        pos.x = Random.Range((float)44.2, (float)51.3);
    }
    public void Attack_End()
    {
        isCreated = false;
        animator.SetBool("isRunning", false);
        _state = State.Idle;
    }
    public void SpecialAttackEnd()
    {
        isSpawn = false;
        animator.SetBool("isRunning", false);
        _state = State.Idle;
    }
    public void On_Hurt_End()
    {
        _state = State.Idle;
        animator.ResetTrigger("Hurt");
        animator.SetBool("isRunning", false);
    }
    public void PlayCastingSFX()
    {
        _audiosource.PlayOneShot(_castingSFX);
    }
    [ContextMenu("TestDead")]
    void TestDead()
    {
        _HP = 0;
        Die();
    }
    enum State
    {
        Idle,
        Run,
        Teleport,
        Attack,
        Hurt,
        Die
    }
}
