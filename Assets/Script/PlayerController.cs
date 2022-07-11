using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private PlayerConfig _configData;
    [SerializeField]
    private PlaySceneCanvas _canvasPlayScene;
    [SerializeField]
    private HPBAR _hpBarScript;
    [SerializeField]
    private StaminaBarScript _staminaScript;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private PlayerState _playerState;
    [SerializeField]
    private Rigidbody2D _rigidBody2D;
    [SerializeField]
    private Vector2 _movingDirection;
    [SerializeField]
    private VolumeController _volumeController;

  //======================================== SFX ========================================
    [SerializeField]
    private AudioClip _softHit;
    [SerializeField]
    private AudioClip _strongHit;
    [SerializeField]
    private AudioClip _swirlHit;
    [SerializeField]
    private AudioClip _fireExplode;
    [SerializeField]
    private AudioClip _igniteFire;
    [SerializeField]
    private AudioClip _dieSFX;    
    [SerializeField]
    private AudioClip _hurtSFX;
    [SerializeField]
    private AudioClip _obtainSFX;
    [SerializeField]
    private AudioSource _audioSource;
    
    //======================================== SFX ========================================

    //==================================================
    //                  CHECK GROUND
    //==================================================

    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private bool _isOnGround;
    [SerializeField]
    private Transform _checkGround;
    [SerializeField]
    private float _rayLength;
    [SerializeField]
    private LayerMask _groundMask;
    [SerializeField]
    private LayerMask _slopeMask;
    [SerializeField]
    private GameObject _stairCheck;
    [SerializeField]
    private float _stairLength;

    //==================================================
    //                      ATTACK
    //==================================================
    [SerializeField]
    private Transform _attackPoint;
    [SerializeField]
    private Vector2 _attackRange;
    [SerializeField]
    private LayerMask _enemyLayer;

    //==================================================
    //                       STAT
    //==================================================
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _playerHP;
    [SerializeField]
    private int _playerStamina;
    [SerializeField]
    private int _playerDamage;
    [SerializeField]
    private int _specialDamage;
    [SerializeField]
    private int _keyObtained;
    [SerializeField]
    private int _needKey;
    //======================================================================

    //======================================================================

    private float _recoverTime;
    private int _attackCombo;
    private float _attackTime;
    private bool _isFacingLeft;

    //*********************************PROPERTY**********************************
    //*                                                                         *   
    //*                                                                         *   
    //***************************************************************************

    public int PlayerHP
    {
        get => _playerHP;
        set
        {
            _playerHP = value;
            PlayerPrefs.SetInt("PlayerHP", _playerHP);
        }
    }
    public int PlayerStamina
    {
        get => _playerStamina;
        set
        {
            _playerStamina = value;
            PlayerPrefs.SetInt("PlayerStamina",_playerStamina);

        }
    }
    public int KeyObtained
    {
        get => _keyObtained;
        set => _keyObtained = value;
    }
    public int NeededKey
    {
        get => _needKey;
    }

    //***************************************************************************
    //*                                                                         *   
    //*                                                                         *   
    //***************************************************************************

    void Start()
    {
        _volumeController.SetUp();
        if (PlayerPrefs.HasKey("INGAME_DATA"))
        {
            var json = PlayerPrefs.GetString("INGAME_DATA");
            GameManager.Instance.ingameData = JsonUtility.FromJson<IngameData>(json);
        }
        if(GameManager.Instance.ingameData._isLevelCompleted==false && GameManager.Instance.ingameData.position!=Vector3.zero&&GameManager.Instance.ingameData._isNewGame==false)
        {
            transform.position = GameManager.Instance.ingameData.position;
            _keyObtained = GameManager.Instance.ingameData.keyobtained;
        }

        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        PlayerPrefs.SetString("ActiveScene", scene.name);
        _isFacingLeft = false;
        GameManager.Instance.ingameData._isNewGame = false;

        _playerHP = _configData.PlayerConfigHP;
        _playerStamina = _configData.PlayerConfigStamina;
        _speed = _configData.PlayerConfigSpeed;
        _playerDamage = _configData.PlayerConfigDamage;
        _specialDamage = _configData.PlayerConfigSpecialDamage;
        jumpForce = _configData.PlayerConfigJumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        //=========================================JUMPING AND FALLING=================================
      
        var velocity = _rigidBody2D.velocity;
        velocity.x = 0f;
        _rigidBody2D.velocity = velocity;

        CheckGround();
        CheckStair();
        CalculateState();

        if (_playerState == PlayerState.Attack) return;
        if (_playerState == PlayerState.Die) return;
        if (_playerState == PlayerState.Hurt) return; 

        if (Input.GetKeyDown(KeyCode.Escape)) BringUpSetting();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _playerState = PlayerState.Jump;

            if (_isOnGround && _rigidBody2D.velocity.y >= 0|| isOnStair)
            {
                _animator.SetTrigger("Jump");
                _rigidBody2D.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
                _isOnGround = false;
            }
        }

        if (!isOnStair && !_isOnGround)
        {
           if( _rigidBody2D.velocity.y < 0) _animator.SetBool("isFalling", true);
        }
        //else if (isOnStair) _animator.SetBool("isFalling", false);
        else _animator.SetBool("isFalling", false);

        //============================================ATTACKING=======================================

        if (Input.GetKeyDown(KeyCode.Z)&&_playerStamina>=10)
        {
            _attackTime = 0f;
            LooseStamina("Normal");
            _playerState = PlayerState.Attack;
            _animator.SetBool("isRunning", false);
            _animator.SetTrigger("Attack_1");
            //Attack(); IN ANIMATION
        }
        else if (Input.GetKeyDown(KeyCode.X)&&_playerStamina>=30)
        {
            _attackTime = 0f; 
            LooseStamina("Special");
            _playerState = PlayerState.Attack;
            _animator.SetBool("isRunning", false);
            _animator.SetTrigger("AttackSpecial");
        }

        //=============================================MOVEMENT=========================================

        if (Input.GetKey(KeyCode.LeftArrow)&&_playerState!=PlayerState.Attack)
        {
            if (isOnStair)
            {

                _movingDirection = -stairObj.transform.right;
            }
            else
            {
                _movingDirection = Vector2.left;
            }

            Movement();
            if (!_isFacingLeft) Flip();
        }
        else if (Input.GetKey(KeyCode.RightArrow)&&_playerState != PlayerState.Attack)
        {
            if (isOnStair)
            {
                _movingDirection = stairObj.transform.right;
            }
            else
            {
                _movingDirection = Vector2.right;
            }
            Movement();
            if (_isFacingLeft)
            {
                Flip();
            }
        }

        //================================================ETC==============================================

        else
        {
            if (_playerState == PlayerState.Attack || _playerState == PlayerState.Jump && _isOnGround == false) return;
            else
            {
                _playerState = PlayerState.Idle;
                _animator.SetBool("isRunning", false);
            }
        }

    }

    //===========================================================================================================
    //                                              MAJOR FUNCTIONS
    //===========================================================================================================
    void Movement()
    {
        transform.Translate(_movingDirection * _speed * Time.deltaTime);

        _playerState = PlayerState.Run;
        _animator.SetBool("isRunning", true);
    }

    void CalculateState()
    {
        if (_playerStamina < _configData.PlayerConfigStamina && _playerState!=PlayerState.Attack)
        {
            _recoverTime += Time.deltaTime;
            if (_recoverTime >= 1f)
            {
                _playerStamina += 20;
                _recoverTime = 0f;
            }
            _staminaScript.UpdateGraphic();
        }
 
        _attackTime += Time.deltaTime;

        if (_attackTime >= 2f)
        {
            _attackCombo = 0;
            _animator.SetInteger("AttackCombo", _attackCombo);
            _attackTime = 0f;
        }
    }
    void BringUpSetting()
    {
        _canvasPlayScene.EnableSettingPanel();
        Time.timeScale = 0f;
    }
    void Flip()
    {
        _isFacingLeft = !_isFacingLeft;
        var _currentScale = transform.localScale;
        _currentScale.x *= -1;
        transform.localScale = _currentScale;
    }

    GameObject stairObj;
    [SerializeField]
    private bool isOnStair;

    void CheckGround()
    {
        Debug.DrawRay(_checkGround.position, Vector2.down*_rayLength,Color.red);
        var hit = Physics2D.Raycast(_checkGround.position, Vector2.down, _rayLength,_groundMask);
        if (hit.collider && hit.collider.CompareTag("Ground"))
        {
            _isOnGround = true;
        }
        else
        {
            _isOnGround = false;
        }
    }
    void CheckStair()
    {
        Debug.DrawRay(_stairCheck.transform.position, Vector2.down * _stairLength, Color.green);

        var hit = Physics2D.Raycast(_stairCheck.transform.position, Vector2.down, _stairLength,_slopeMask);
        if (hit.collider && hit.collider.CompareTag("Stair"))
        {
            isOnStair = true;
            _rigidBody2D.gravityScale = 0f;
            stairObj = hit.collider.gameObject;
        }
        else
        {
            _rigidBody2D.gravityScale = 1f;
            isOnStair = false;
            stairObj = null;
        }
    }

    public void Attack()
    {
        _attackPoint.gameObject.SetActive(true);

        Collider2D[] enemies = Physics2D.OverlapBoxAll(_attackPoint.position, _attackRange, _enemyLayer);
        foreach(Collider2D enemy in enemies)
        {
            if (enemy.gameObject.CompareTag("Enemy"))
            {
                enemy.gameObject.GetComponent<EnemyScript>().TakeDamage(_playerDamage);
            }
            else if (enemy.gameObject.CompareTag("Skeleton"))
            {
                enemy.gameObject.GetComponent<RiseSkeleton>().TakeDamage(_playerDamage);
            }
            else if (enemy.gameObject.CompareTag("Ghost"))
            {
                enemy.gameObject.GetComponent<GhostScript>().TakeDamage(_playerDamage);
            }
            else if (enemy.gameObject.CompareTag("Demon"))
            {
                enemy.gameObject.GetComponent<DemonScript>().TakeDamage(_playerDamage);
            }
            else if (enemy.gameObject.CompareTag("Skull"))
            {
                enemy.gameObject.GetComponent<SkullScript>().TakeDamage(_playerDamage);
            }
            else if (enemy.gameObject.CompareTag("Boss"))
            { 
                enemy.gameObject.GetComponent<NecromancerScript>().TakeDamage(_playerDamage);
            }
        }
        _attackPoint.gameObject.SetActive(false);

    }
    public void SpecialAttack()
    {
        _attackPoint.gameObject.SetActive(true);

        Collider2D[] enemies = Physics2D.OverlapBoxAll(_attackPoint.position, _attackRange, _enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.gameObject.CompareTag("Enemy"))
            {
                enemy.gameObject.GetComponent<EnemyScript>().TakeDamage(_specialDamage);
            }
            else if (enemy.gameObject.CompareTag("Skeleton"))
            {
                enemy.gameObject.GetComponent<RiseSkeleton>().TakeDamage(_specialDamage);
            }
            else if (enemy.gameObject.CompareTag("Ghost"))
            {
                enemy.gameObject.GetComponent<GhostScript>().TakeDamage(_specialDamage);
            }
            else if (enemy.gameObject.CompareTag("Demon"))
            {
                enemy.gameObject.GetComponent<DemonScript>().TakeDamage(_specialDamage);
            }
            else if (enemy.gameObject.CompareTag("Skull"))
            {
                enemy.gameObject.GetComponent<SkullScript>().TakeDamage(_specialDamage);
            }
            else if (enemy.gameObject.CompareTag("Boss"))
            {
                enemy.gameObject.GetComponent<NecromancerScript>().TakeDamage(_specialDamage);
            }
        }
    
        _attackPoint.gameObject.SetActive(false);
    }

    public void LooseStamina(string atktype)
    {
        if(atktype=="Normal") _playerStamina -= 25;
        else _playerStamina -= 35;

        _staminaScript.UpdateGraphic();
    }
    public void TakeDamage(int damage)
    {
        if (_playerState == PlayerState.Die) return;
        _playerHP -= damage;
        _hpBarScript.UpdateGraphic();
        _playerState = PlayerState.Hurt;
        _animator.SetTrigger("Hurt");
        if(_playerHP<=0)
        {
            _playerState = PlayerState.Die;
            _animator.SetTrigger("Die");
        }
    }
    void Die()
    {
        _canvasPlayScene.GameOver();
    }

    //===========================================================================================================
    //                                              ANIMATION
    //===========================================================================================================
    public void On_Hurt_Anim_End()
    {
        _playerState = PlayerState.Idle;
        _animator.SetBool("isRunning", false);
    }
    public void On_Attack_End(int value)
    {
        _playerState = PlayerState.Idle;
        _animator.SetBool("isRunning", false);
        _attackCombo = value;
        _animator.SetInteger("AttackCombo", _attackCombo);
    }    
    public void On_Special_Attack_End()
    {
        _playerState = PlayerState.Idle;
        _animator.SetBool("isRunning", false);
    }

    /// <summary>
    /// Item etc
    /// </summary>
    public void EatPotion()
    {
        _playerHP += 30;
        if (_playerHP > _configData.PlayerConfigHP) _playerHP = _configData.PlayerConfigHP;
        _hpBarScript.UpdateGraphic();
    }
    public void ObtainKey()
    {
        _keyObtained += 1;
    }



    //===========================================================================================================
    //                                                UNITY
    //===========================================================================================================


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Potion"))
        {
            EatPotion();
            _audioSource.PlayOneShot(_obtainSFX);
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.CompareTag("Key"))
        {
            ObtainKey();
            _audioSource.PlayOneShot(_obtainSFX);
            collision.gameObject.SetActive(false);
            var item = collision.gameObject.GetComponent<ItemController>();
            if (!GameManager.Instance.ingameData.itemID.Contains(item.itemID))
                GameManager.Instance.ingameData.itemID.Add(item.itemID);
        }
        else if(collision.gameObject.CompareTag("CheckPoint"))
        {
            Debug.Log("saved");
            GameManager.Instance.ingameData._isLevelCompleted = false;
            GameManager.Instance.ingameData.position = collision.gameObject.transform.position;
            GameManager.Instance.ingameData.keyobtained = _keyObtained;
            string json = JsonUtility.ToJson(GameManager.Instance.ingameData);
            PlayerPrefs.SetString("INGAME_DATA", json);
        }    
    }
    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null) return;
        Gizmos.DrawWireCube(_attackPoint.position, _attackRange);
    }


    /// <summary>
    /// SFX AREA
    /// </summary>
    public void SoftHit()
    {
        _audioSource.PlayOneShot(_softHit);
    }
    public void StrongHit()
    {
        _audioSource.PlayOneShot(_strongHit);
    }   public void SwirlHit()
    {
        _audioSource.PlayOneShot(_swirlHit);
    }
    public void FireExlode()
    {
        _audioSource.PlayOneShot(_fireExplode) ;
    }
    public void IgniteFire()
    {
        _audioSource.PlayOneShot(_igniteFire);
    }
    public void DieSFX()
    {
        _audioSource.PlayOneShot(_dieSFX);
    }
    public void HurtSFX()
    {
        _audioSource.PlayOneShot(_hurtSFX);

    }
}
public enum PlayerState
{
    None,
    Idle,
    Run,
    Jump,
    Attack,
    Hurt,
    Die,
}
