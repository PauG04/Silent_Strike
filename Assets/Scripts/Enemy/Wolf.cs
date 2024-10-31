using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy
{

    private void Start()
    {
        base.InitializeEnemy();
    }

    private void Update()
    {
        switch(currentState)
        {
            case enemyState.IDLE:
                break;
            case enemyState.RUNNING:
                SeekTarget();
                animator.SetBool("Running", true);
                break;
            case enemyState.ATTACK:
                Attack();
                break;
            case enemyState.HURT:
                break;
            case enemyState.DIE:
                break;
        }
        base.UpdateEnemy();
    }


}
