using UnityEngine;

public class StateChase : State
{
    public override void Execute(Enemy enemy)
    {
        if (enemy.IsDead)
        {
            enemy.ChangeState(new StateDead());
            return;
        }

        if (enemy is BossEnemy boss && boss.IsWithinAttackRange())
        {
            boss.ChangeState(new StateTelegraph());
            return;
        }

        if (enemy is RangedEnemy rangedEnemy && rangedEnemy.IsWithinAimingRange())
        {
            enemy.ChangeState(new StateAttack());
            return;
        }

        if (!(enemy is RangedEnemy) && enemy.IsWithinAttackRange())
        {
            enemy.ChangeState(new StateAttack());
            return;
        }

        enemy.MoveTowardsPlayer();
    }
}
