using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField]
    private MonsterSpawn spawn;
    [SerializeField]
    private int skeletonNum;
    [SerializeField]
    private int warrokNum;

    private bool isOpen;

    //private void Start()
    //{
    //    spawn.ChestOpen(MonsterNum);
    //}

    private void AddItem()
    {
        ItemList.instance.RespawnSphere(transform.position, new Color(1, 0.6640f, 0, 1));
        //if (item == null) return;
        //Character.instance.AddItem(item);
        //UIManager.instance.PlayerGetItem(item);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (!isOpen && Input.GetKeyDown(KeyCode.E))
            {
                spawn.ChestCount();
                spawn.ChestOpen<Skeleton>(skeletonNum);
                spawn.ChestOpen<Warrok>(warrokNum);
                AddItem();
                isOpen = true;
                StartCoroutine(IEnumChestOpen());
            }
        }
    }

    private IEnumerator IEnumChestOpen()
    {
        yield return new WaitForSeconds(1.0f);
        Material[] materials = GetComponent<Renderer>().materials;
        float time = 0;
        while (time <= 3)
        {
            for (int i = 0; i < materials.Length; i++)
                materials[i].color = Color.Lerp(materials[i].color, new Color(1, 1, 1, 0), 2 * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}