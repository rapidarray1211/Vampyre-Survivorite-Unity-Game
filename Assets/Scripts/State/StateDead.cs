using UnityEngine;

public class StateDead : State
{
    public override void Execute(Enemy enemy)
    {
        enemy.Die();
    }
}
