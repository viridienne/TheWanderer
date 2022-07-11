using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SpellDataConfig",menuName ="Enemy/Spell Config")]
public class SpellDataConfig : ScriptableObject
{
    public int SpellDamage;
    public float SpellSpeed;
    public float SpellDuration;
}
