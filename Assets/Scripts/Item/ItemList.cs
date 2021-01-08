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
        return items[num];
    }
}

public class Spread : IItemEffect
{
    private Arrow[] arrows = new Arrow[10];

    public void Effect(Arrow _arrow)
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i] = Character.instance.ArrowDequeue();
            float num = (360 / arrows.Length) * i;
            arrows[i].transform.rotation = Quaternion.Euler(new Vector3(0, num, 0));
            arrows[i].transform.position = _arrow.transform.position + arrows[i].transform.forward * 2.5f;
            arrows[i].SetArrowDamage(_arrow.GetArrowDamage());
            arrows[i].SetSkillSpread(false);
            arrows[i].gameObject.SetActive(true);
        }
    }
}