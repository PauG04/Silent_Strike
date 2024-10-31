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
        currentState = enemyState.IDLE;
        canDash = false;
    }

    private void Update()
    {
        switch(currentState)
        {
            case enemyState.IDLE:
                IdleState();
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
                HurtState();
                break;
            case enemyState.DIE:
                DieState();
                break;
        }
        base.UpdateEnemy();
    }

    //State
    #region
    private void IdleState()
    {

    }

    private void RunningState()
    {
        SeekTarget();
    }

    private void ChargingState()
    {
        ChargingAttack();
        AttackReady("WolfPrepareAttack");

        if(!canDash)
            canDash = true;
    }

    private void AttackState()
    {
        Dash();
        Attack("WolfAttack");
    }

    private void RecoveryState()
    {
        SeekTarget();
        Recovery();
    }
    private void HurtState()
    {
        Hurt("WolfHurt");
    }

    private void DieState()
    {
        Die("WolfDie");
    }

    #endregion
    private void Dash()
    {
        if(canDash)
        {
            rgbd.AddForce(direction.normalized * dashForce, ForceMode.Impulse);
            canDash = false;
        }

    }
}
