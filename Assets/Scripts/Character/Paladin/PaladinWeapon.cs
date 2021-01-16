﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaladinWeapon : MonoBehaviour
{
    private Paladin paladin;
    private Collider col;
    private float damage;
    [SerializeField]
    private TrailRenderer trail;

    private void Start()
    {
        paladin = Character.instance as Paladin;
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            EventManager.instance.AttackEnemy(damage, other.transform.GetInstanceID());
        }
    }

    public void Attack()
    {
        col.enabled = true;
        trail.emitting = true;
    }

    public void AttackEnd()
    {
        col.enabled = false;
        trail.emitting = false;
    }

    public void SetDamage(float _damage)
    {
        damage = _damage;
    }
}
