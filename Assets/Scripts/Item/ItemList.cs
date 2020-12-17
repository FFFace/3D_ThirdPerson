using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField]
    private List<Item> items = new List<Item>();

    public static ItemList instance;

    private void Awake()
    {
        if (instance == null) instance = null;
        else Destroy(gameObject);
    }

    public Item GetRandomItem()
    {
        int num = Random.Range(0, items.Count);

        return items[num];
    }
}
