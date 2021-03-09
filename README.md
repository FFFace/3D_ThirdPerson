# 3인칭 액션 / 전일우


## 조작

커맨드 패턴으로 간단하게 구현했습니다.

```
private KeyCommand mouseLeftClick;
private KeyCommand mouseRightClick;
...

private void InitData()
{
mouseLeftClick = new NomalAttackPressDown(character);
mouseLeftUp = new NomalAttackPressUp(character);
...
}

private void PlayerInput()
{
if (!isDead)
{
character.Move();

if (Input.GetMouseButton(0))
mouseLeftClick.command();
else if (Input.GetMouseButtonUp(0))
mouseLeftUp.command();
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
.
.
.
```
