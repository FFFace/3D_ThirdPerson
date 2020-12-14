using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemAbility {OFFENSIVE, MOBILITY}
    public ItemAbility itemAbility;


    protected void ItemApply(ref CharacterState state )
    {
        
    }
}
