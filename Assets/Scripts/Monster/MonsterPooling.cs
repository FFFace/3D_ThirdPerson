using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPooling : MonoBehaviour
{
    public static MonsterPooling instance;

    private Dictionary<Type, Queue<Monster>> monsterPool = new Dictionary<Type, Queue<Monster>>();
    private Queue<Monster> skeletonPool = new Queue<Monster>();
    private Queue<Monster> warrokPool = new Queue<Monster>();
    private Queue<Monster> dragonPool = new Queue<Monster>();

    [SerializeField]
    private List<GameObject> monsters = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        monsterPool.Add(typeof(Skeleton), skeletonPool);
        monsterPool.Add(typeof(Warrok), warrokPool);
        monsterPool.Add(typeof(Dragon), dragonPool);
    }

    private void Update()
    {
    }

    public void MonsterEnqueue<T>(T monster) where T : Monster
    {
        Type type = typeof(T);

        monsterPool[type].Enqueue(monster);
    }

    public Monster MonsterDequeue<T>() where T : Monster
    {
        Type type = typeof(T);

        if (monsterPool[type].Count > 0) return monsterPool[type].Dequeue();

        Monster monster = CreateMonster<T>();
        return monster;
    }

    private Monster CreateMonster<T>() where T : Monster
    {
        Type type = typeof(T);
        Monster monster = null;

        foreach (var obj in monsters)
        {
            if (type == obj.gameObject.GetComponent<Monster>().GetType())
            {
                monster = Instantiate(obj).GetComponent<Monster>();
                monster.gameObject.SetActive(false);
                break;
            }
        }
        return monster;
    }
}
