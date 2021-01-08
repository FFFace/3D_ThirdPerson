using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New Item", menuName = "Item", order = 2)]
public class Item : ScriptableObject
{
    [SerializeField]
    private string itemName;
    [SerializeField]
    private string explain;
    [SerializeField]
    private float hpMag;
    [SerializeField]
    private float HP;
    [SerializeField]
    private float damageMag;
    [SerializeField]
    private float damage;
    [SerializeField]
    private float attackSpeedMag;
    [SerializeField]
    private float moveSpeedMag;
    [SerializeField]
    private float knockback;

    private IItemEffect effect;

    [SerializeField]
    protected Sprite itemImage;

    public float GetItemDamageMag => damageMag;
    public float GetItemDamage => damage;
    public float GetItemHPMag => hpMag;
    public float GetItemHP => HP;
    public string GetItemName => itemName;
    public string GetItemEx => explain;
    public float GetItemMoveSpeedMag => moveSpeedMag;
    public float GetItemAttackSpeedMag => attackSpeedMag;
    public Sprite GetItemSprite => itemImage;
    public IItemEffect GetItemEffect()
    {
        return effect;
    }

    public void SetItemEffect(IItemEffect _effect)
    {
        effect = _effect;
    }
}

public interface IItemEffect
{
    void Effect(Arrow _arrow);
}