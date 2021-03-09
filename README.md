# 3인칭 액션 / 전일우


## 조작

```
private KeyCommand mouseLeftClick;
private KeyCommand mouseRightClick;
...

private void PlayerInput()
{
    if (!isDead)
    {
        character.Move();

        if (Input.GetMouseButton(0))
            mouseLeftClick.command();
...
}

public class NomalAttackPressDown : KeyCommand
{
    private Character character;
    public NomalAttackPressDown(Character _character) { character = _character; }
    public void command()
    {
        character.AttackPressDown();
    }
}

public class MainSkillPressDown : KeyCommand
{
    private Character character;
    public MainSkillPressDown(Character _character) { character = _character; }
    public void command()
    {
        character.MainSkillPressDown();
    }
}
...
```

```
public abstract class Character : MonoBehaviour
{
    ...
    protected IAttackAction normalAttack;
    protected ISkill subAttack;
    protected ISkill mainSkill;
    protected ISkill subSkill;
    ...
}
```

```
public class Archer : Character
{
    ...
    
    public override void AttackPressDown()
    {
        normalAttack = recharge;
        if (!isAction && !isAttack)
        {
            isAttack = true;
            normalAttack.Attack();
        }
    }
    
    ...
    
    public override void MainSkillPressDown()
    {
        if (mainSkill != null)
        {
            if (!isAction)
            {
                Debug.Log("MainSkill");
                mainSkill.SkillKeyDown();
                UIManager.instance.SetMainSkillCoolTime(mainSkill.isActive);
            }
        }
    }
    
    ...
```

전투를 하기 전 UI를 통해 캐릭터와 스킬을 선택합니다.

각각의 키에 해당되는 인터페이스를 제공했습니다. 해당 인터페이스는 캐릭터의 액션 함수에 접근합니다.
필요 시, 키 세팅 부분을 추가하기 좋게 만들었습니다.

## 몬스터 소환

```
public class MonsterPooling : MonoBehaviour
{
    public static MonsterPooling instance;

    private Dictionary<Type, Queue<Monster>> monsterPool = new Dictionary<Type, Queue<Monster>>();
    private Queue<Monster> skeletonPool = new Queue<Monster>();
    private Queue<Monster> warrokPool = new Queue<Monster>();
    private Queue<Monster> dragonPool = new Queue<Monster>();
    
    ...
    
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
```

```
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
```

몬스터를 소환할때 Pool의 몬스터를 우선적으로 사용하며 필요 시 몬스터를 추가적으로 할당합니다.
몬스터 풀링을 Generic으로 구현해 새로운 몬스터를 추가할 때 마다 드는 번거로움을 조금 줄여봤습니다.

미리 소환 및 이동이 가능한 장소를 파악해 해당 위치에 몬스터를 소환하거나 몬스터 AI에 활용할 수 있게 만들었습니다.
