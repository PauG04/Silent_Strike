using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy
{
    [Header("Dash")]
    [SerializeField] private float dashForce;
    private bool canDash;

    private void Start()
    {
        base.InitializeEnemy();
        animator.SetBool("Idle", true);
        currentState = enemyState.GENERATING;
        canDash = false;
    }

    private void Update()
    {
        switch(currentState)
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
        ChargingAttack();
        ChangeToAttackColor();
        if (!canDash)
            canDash = true;
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


    private void Dash()
    {
        rgbd.AddForce(direction.normalized * dashForce, ForceMode.Impulse);
        canDash = false;
    }
}
