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
        AttackReady("NinjaPrepareAttack");
    }

    private void AttackState()
    {
        Attack("NinjaAttack");
    }

    private void RecoveryState()
    {
        WanderTarget();
        FlipSprite();
        Recovery();

    }
    private void HurtState()
    {
        Hurt("NinjaHurt");
    }

    private void DieState()
    {
        Die("NinjaDie");
    }

    #endregion

    private void FlipSprite()
    {
        if (transform.position.x > target.transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}
