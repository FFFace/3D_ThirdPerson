using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public static CharacterInfo instance;

    [SerializeField]
    public GameObject[] characters;
    public int characterNum { get; set; }

    private ISkill[] skills = new ISkill[3];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void SetSkills(ISkill[] _skills)
    {
        for (int i = 0; i < _skills.Length; i++)
        {
            if (i == 0) continue;

            skills[i - 1] = _skills[i];
        }
    }

    public ISkill GetSkills(int num)
    {
        if (num > skills.Length || num < 0)
        {
            Debug.LogError("인덱스 범위를 벗어남");
            return null;
        }

        return skills[num];
    }

    public GameObject SelectPlayer()
    {
        return characters[characterNum];
    }
}
