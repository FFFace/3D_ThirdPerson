using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField]
    private bool[,] map;

    [SerializeField]
    private int tileXNum;
    [SerializeField]
    private int tileZNum;

    [SerializeField]
    private bool isWarrok;
    private bool isBattle;

    private List<Monster> monsters = new List<Monster>();
    //private bool isActive = true;

    private void Start()
    {
        MapSetting();
        //StartCoroutine(UpdateMonsterState());
    }

    //private IEnumerator UpdateMonsterState()
    //{
    //    while (true)
    //    {
    //        float distance = Vector3.Distance(transform.position, Character.instance.transform.position);
    //        bool isOn = distance < 50.0f ? true : false;

    //        //Debug.Log(distance.ToString() + ", " + transform.name);

    //        if (isActive != isOn)
    //        {
    //            //Debug.Log(isOn);
    //            if (isOn)
    //            {
    //                foreach (var monster in monsters)
    //                {
    //                    Monster.MonsterAction state = monster.GetMonsterState();
    //                    if(state == Monster.MonsterAction.NULL)
    //                        monster.SetMonsterState(Monster.MonsterAction.STAY);
    //                    isActive = isOn;
    //                }
    //            }

    //            else
    //            {
    //                foreach (var monster in monsters)
    //                {
    //                    Monster.MonsterAction state = monster.GetMonsterState();
    //                    if(state == Monster.MonsterAction.STAY)
    //                        monster.SetMonsterState(Monster.MonsterAction.NULL);
    //                    isActive = isOn;
    //                }
    //            }
    //        }

    //        yield return new WaitForSeconds(1.0f);
    //    }
    //}

    /// <summary>
    /// 가상의 타일을 만들어 해당 타일위치에 소환이 가능한 상태인지 판별
    /// </summary>
    private void MapSetting()
    {
        float tileX = transform.lossyScale.x / tileXNum;
        float tileZ = transform.lossyScale.z / tileZNum;

        float x = transform.position.x - transform.lossyScale.x * 0.5f + tileX * 0.5f;
        float z = transform.position.z - transform.lossyScale.z * 0.5f + tileZ * 0.5f;

        map = new bool[tileXNum, tileZNum];

        LayerMask layer = 1 << LayerMask.NameToLayer("Wall");

        for (int i = 0; i < tileXNum; i++)
        {
            for (int j = 0; j < tileZNum; j++)
            {
                Vector3 pos = new Vector3(x + i * tileX, 0.5f, z + j * tileZ);
                map[i, j] = Physics.OverlapBox(pos, new Vector3(tileX, 0, tileZ), Quaternion.identity, layer).Length > 0 ? false : true;

                //if (map[i, j])
                //{
                //    GameObject a = new GameObject();
                //    a.transform.position = pos;
                //    a.AddComponent<BoxCollider>();
                //    a.GetComponent<BoxCollider>().size = new Vector3(tileX, 0, tileZ);
                //}
            }
        }
    }

    public void ChestOpen(int summonMonsterNum)
    {
        SummonMonsters(summonMonsterNum);
    }

    private void SummonMonsters(int summonMonsterNum)
    {
        int num = Random.Range(Mathf.CeilToInt(summonMonsterNum / 2), summonMonsterNum + 1);

        for(int i=0; i<num; i++)
        {
            Vector3 pos = GetMoveTile();

            //Monster monster = MonsterPooling.instance.SkeletonDequeue();
            Monster monster = MonsterPooling.instance.MonsterDequeue<Skeleton>();
            monster.transform.position = pos;
            monster.SetMonsterRoom(this);
            monster.gameObject.SetActive(true);
            monsters.Add(monster);
            monster.SetSummonBoss(false);
        }

        if (isWarrok)
        {
            Vector3 pos = GetMoveTile();

            Monster monster = MonsterPooling.instance.MonsterDequeue<Warrok>();
            monster.transform.position = pos;
            monster.SetMonsterRoom(this);
            monster.SetSummonBoss(false);
            monster.gameObject.SetActive(true);
            monsters.Add(monster);
            isWarrok = false;
        }
    }

    public Vector3 GetMoveTile()
    {
        int x = Random.Range(0, tileXNum);
        int z = Random.Range(0, tileZNum);

        //Debug.Log("시작 좌표 : " + x.ToString() + ", " + z.ToString());

        while (true)
        {
            //Debug.Log("변환 좌표 : " + x.ToString() + ", " + z.ToString());
            if (map[x, z]) break;

            if (x < tileXNum - 1) { x++; continue; }
            else if (z < tileZNum - 1) { x = 0; z++; continue; }
            else { x = 0; z = 0; }
        }

        float tileX = transform.lossyScale.x / tileXNum;
        float tileZ = transform.lossyScale.z / tileZNum;

        float posX = transform.position.x - transform.lossyScale.x * 0.5f + tileX * 0.5f;
        float posZ = transform.position.z - transform.lossyScale.z * 0.5f + tileZ * 0.5f;

        Vector3 pos = new Vector3(posX + x * tileX, 1, posZ + z * tileZ);

        return pos;
    }
}
