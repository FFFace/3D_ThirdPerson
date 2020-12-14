using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherKick : MonoBehaviour
{
    private Character character;
    [SerializeField]
    private Collider col;

    private void Start()
    {
        col = GetComponent<Collider>();
        character = Character.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            float damage = character.GetCharacterCurrentDamage() * 1.2f;
            EventManager.instance.AttackEnemy(damage, other.transform.GetInstanceID(), true, 8);
        }
    }


    public void ColliderEnable()
    {
        col.enabled = true;
    }

    public void ColliderDisable()
    {
        col.enabled = false;
    }
}
