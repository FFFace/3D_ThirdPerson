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
    }

    private void Update()
    {
    }

    public void MonsterEnqueue<T>(T monster) where T : Monster
    {
        Type type = typeof(T);

        monsterPool[type].Enqueue(monster);
        //switch (monster)
        //{
        //    case Skeleton skeleton:
        //        skeletonPool.Enqueue(skeleton);
        //        break;

        //    case Warrok warrok:
        //        warrokPool.Enqueue(warrok);
        //        break;

        //    default:
        //        Debug.Log("존재하지 않는 몬스터");
        //        break;
        //}
    }

    public Monster MonsterDequeue<T>() where T : Monster
    {
        Type type = typeof(T);

        if (monsterPool[type].Count > 0) return monsterPool[type].Dequeue();

        Monster monster = CreateMonster<T>();
        return monster;


        //T monster = new T();
        //switch (monster)
        //{
        //    case Skeleton skeleton:
        //        monster = skeletonPool.Dequeue() as T;
        //        break;

        //    case Warrok warrok:
        //        monster = warrokPool.Dequeue() as T;
        //        break;

        //    default:
        //        Debug.Log("존재하지 않는 몬스터");
        //        break;
        //}

        //return monster;
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

    //public Monster SkeletonDequeue()
    //{
    //    Monster monster = skeletonPool.Count > 0 ? skeletonPool.Dequeue() : CreateSkeleton();

    //    return monster;
    //}

    //private Monster CreateSkeleton()
    //{
    //    Monster monster = Instantiate(skeleton).GetComponent<Monster>();
    //    monster.gameObject.SetActive(false);
    //    return monster;
    //}
}
