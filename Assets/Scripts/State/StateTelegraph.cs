using UnityEngine;

public class StateTelegraph : State
{
    private float timer = 0f;
    private bool started = false;

    public override void Execute(Enemy enemy)
    {
        if (enemy.IsDead)
        {
            enemy.ChangeState(new StateDead());
            return;
        }

        if (enemy is BossEnemy boss)
        {
            if (!started)
            {
                boss.StartTelegraph();
                started = true;
            }

            timer += Time.deltaTime;

            if (timer >= boss.telegraphDuration)
            {
                boss.ChangeState(new StateAttack());
            }
        }
    }
}
