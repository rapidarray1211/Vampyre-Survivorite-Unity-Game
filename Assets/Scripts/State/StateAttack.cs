using UnityEngine;

public class StateAttack : State
{
    public override void Execute(Enemy enemy)
    {
        if (enemy.IsDead)
        {
            enemy.ChangeState(new StateDead());
            return;
        }

        if (enemy is BossEnemy boss)
        {
            Debug.Log("Boss is attacking!"); // Debugging line

            boss.PerformAttack(); // ✅ Force the attack execution
            boss.ChangeState(new StateChase()); // ✅ Ensure it transitions back after attacking
            return;
        }
        if (enemy is RangedEnemy rangedEnemy && !rangedEnemy.IsWithinAttackRange())
        {
            enemy.ChangeState(new StateChase());
            return;
        }

        if (!(enemy is RangedEnemy) && !enemy.IsWithinAttackRange())
        {
            enemy.ChangeState(new StateChase());
            return;
        }

        enemy.AttackPlayer();
    }
}
