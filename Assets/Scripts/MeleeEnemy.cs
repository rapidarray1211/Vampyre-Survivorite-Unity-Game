using UnityEngine;

public class MeleeEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        currentState = new StateChase();
    }
}
