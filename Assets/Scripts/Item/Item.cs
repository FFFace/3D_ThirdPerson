using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Item : MonoBehaviour
{
    protected ItemState itemState;
    protected IItemEffect effect;

    [SerializeField]
    protected Sprite itemImage;

    protected virtual void Start()
    {
        InitDate();   
    }

    protected virtual void InitDate()
    {

    }

    public float GetItemDamageMag()
    {
        return itemState.damageMag;
    }

    public float GetItemDamage()
    {
        return itemState.damage;
    }

    public float GetItemHPMag()
    {
        return itemState.hpMag;
    }

    public float GetItemHP()
    {
        return itemState.HP;
    }

    public string GetItemString()
    {
        return itemState.name;
    }

    public float GetItemMoveSpeedMag()
    {
        return itemState.moveSpeedMag;
    }

    public float GetItemAttackSpeedMag()
    {
        return itemState.attackSpeedMag;
    }

    public IItemEffect GetItemEffect()
    {
        return effect;
    }
}

public class ItemState
{
    public string name;
    public float hpMag;
    public float HP;
    public float damageMag;
    public float damage;
    public float attackSpeedMag;
    public float moveSpeedMag;
    public float knockback;
}

public interface IItemEffect
{
    void Effect(Arrow _arrow);
}