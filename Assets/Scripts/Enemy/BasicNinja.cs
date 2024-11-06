using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class BasicNinja : Enemy
{
    private void Start()
    {
        base.InitializeEnemy();
        animator.SetBool("Idle", true);
        currentState = enemyState.GENERATING;
    }

    private void Update()
    {
        switch (currentState)
        {
            case enemyState.GENERATING:
                GeneratingState();
                break;
            case enemyState.RUNNING:
                RunningState();
                break;
            case enemyState.CHARGING:
                ChargingState();
                break;
            case enemyState.ATTACK:
                AttackState();
                break;
            case enemyState.RECOVERY:
                RecoveryState();
                break;
            case enemyState.HURT:
                break;
            case enemyState.DIE:
                DieState();
                break;
        }
        base.UpdateEnemy();
    }

    //State
    #region
    private void GeneratingState()
    {
        Generating();

    }

    private void RunningState()
    {
        SeekTarget();
    }

    private void ChargingState()
    {
        ChangeToAttackColor();
        ChargingAttack();
    }

    private void AttackState()
    {

    }

    private void RecoveryState()
    {
        WanderTarget();
        Recovery();

    }

    private void DieState()
    {
        Die();
    }

    #endregion


}
