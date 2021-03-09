# 3인칭 액션 / 전일우


## 조작

커맨드 패턴으로 간단하게 구현했습니다.

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


각각의 키에 해당되는 인터페이스를 제공했습니다. 해당 인터페이스는 캐릭터의 액션 함수에 접근합니다.
필요 시, 키 세팅 부분을 추가하기 좋게 만들었습니다.

## 플레이어 캐릭터



    
