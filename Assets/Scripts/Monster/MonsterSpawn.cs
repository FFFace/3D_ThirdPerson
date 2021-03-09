using System;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField]
    private bool[,] map;

    [SerializeField]
    private int tileXNum;
    [SerializeField]
    private int tileZNum;

    [SerializeField]
    private int chestNum;
    private int openChestNum = 0;

    [SerializeField]
    private bool isWarrok;
    private bool isBoss;

    private void Start()
    {
        MapSetting();
        isBoss = true;
    }

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
            }
        }
    }

    public void ChestOpen<T>(int summonMonsterNum) where T : Monster
    {
        SummonMonsters<T>(summonMonsterNum);

        if (openChestNum >= chestNum && isBoss)
        {
            Vector3 pos = GetMoveTile();
            pos.y = -20;
            Monster monster = MonsterPooling.instance.MonsterDequeue<Dragon>();
            monster.transform.position = pos;
            monster.SetMonsterRoom(this);
            monster.gameObject.SetActive(true);
            monster.SetSummonBoss(false);
            isBoss = false;
        }
    }

    private void SummonMonsters<T>(int summonMonsterNum) where T : Monster
    {
        int num = UnityEngine.Random.Range(Mathf.CeilToInt((float)summonMonsterNum / 2), summonMonsterNum + 1);
        Type type = typeof(T);

        for(int i=0; i<num; i++)
        {
            Vector3 pos = GetMoveTile();

            Monster monster = MonsterPooling.instance.MonsterDequeue<T>();
            monster.transform.position = pos;
            monster.SetMonsterRoom(this);
            monster.gameObject.SetActive(true);
            monster.SetSummonBoss(false);
        }
    }

    public Vector3 GetMoveTile()
    {
        int x = UnityEngine.Random.Range(0, tileXNum);
        int z = UnityEngine.Random.Range(0, tileZNum);

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

    public void ChestCount()
    {
        openChestNum++;
    }
}
