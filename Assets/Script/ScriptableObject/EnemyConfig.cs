using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyConfig",menuName ="Enemy/Enemy Data")]

public class EnemyConfig : ScriptableObject
{
    public float EnemyConfigSpeed;
    public int EnemyConfigDamage;
    public int EnemyConfigHP;
    public float StateDuration;
    public float AttackDelay;
}
