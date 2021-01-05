using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField]
    private List<Item> items = new List<Item>();
    [SerializeField]
    private GameObject itemSphere;

    public static ItemList instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public Item GetRandomItem()
    {
        int num = Random.Range(0, items.Count);
        if (num == 0) return null;
        return items[num];
    }

    public void CreateItemSphere(Transform _transform)
    {

    }
}
