using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Player/Player Data")]

public class PlayerConfig : ScriptableObject
{
    public float PlayerConfigSpeed;
    public int PlayerConfigHP;
    public int PlayerConfigStamina;
    public int PlayerConfigDamage;
    public int PlayerConfigSpecialDamage;
    public float PlayerConfigJumpForce;
}
