﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField]
    private MonsterSpawn spawn;
    [SerializeField]
    private int MonsterNum;

    private bool isOpen;

    //private void Start()
    //{
    //    spawn.ChestOpen(MonsterNum);
    //}

    private void AddItem()
    {
        Item item = ItemList.instance.GetRandomItem(transform.position);
        if (item == null) return;
        Character.instance.AddItem(item);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (!isOpen && Input.GetKeyDown(KeyCode.E))
            {
                spawn.ChestOpen(MonsterNum);
                AddItem();
            }
        }
    }

    private IEnumerator IEnumItemDrop()
    {
        yield return new WaitForSeconds(1.0f);

    }
}