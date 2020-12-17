using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public delegate void HitDamage(float damage, int instanceID, bool knockBack = false, float knockBackPower = 0);
    private event HitDamage hitDamage;

    public delegate void BuffDamage(MonsterSpawn room, float time, float magnification);
    private event BuffDamage monsterBuffDamage;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AttackEnemy(float damage, int instanceID, bool knockBack = false, float knockBackPower = 0)
    {
        hitDamage(damage, instanceID, knockBack, knockBackPower);
    }

    public void MonsterBuffDamage(MonsterSpawn room, float time, float magnification)
    {
        monsterBuffDamage(room, time, magnification);
    }

    public void AddMonsterBuffDamageEvent(BuffDamage _ref)
    {
        monsterBuffDamage += _ref;
    }

    public void SubMonsterBuffDamageEvent(BuffDamage _ref)
    {
        monsterBuffDamage -= _ref;
    }

    public void AddHitEvent(HitDamage _ref)
    {
        hitDamage += _ref;
    }

    public void SubHitEvent(HitDamage _ref)
    {
        hitDamage -= _ref;
    }
}
