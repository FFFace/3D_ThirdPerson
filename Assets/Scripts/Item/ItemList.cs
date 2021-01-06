using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField]
    private List<Item> items = new List<Item>();
    [SerializeField]
    private GameObject itemSphere;

    private Queue<ItemSphere> itemSpherePool = new Queue<ItemSphere>();

    public static ItemList instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void ItemSphereEnqueue(ItemSphere _item)
    {
        itemSpherePool.Enqueue(_item);
    }

    private ItemSphere ItemSphereDequeue()
    {
        ItemSphere item;
        if (itemSpherePool.Count > 0)
            item = itemSpherePool.Dequeue();

        else
        {
            item = Instantiate(itemSphere, transform.position, Quaternion.identity).GetComponent<ItemSphere>();
            item.gameObject.SetActive(false);
        }

        return item;
    }

    public Item GetRandomItem(Vector3 _pos)
    {
        ItemSphere item = ItemSphereDequeue();
        item.transform.position = _pos;
        item.gameObject.SetActive(true);

        int num = Random.Range(0, items.Count);
        if (num == 0) return null;
        return items[num];
    }
}
