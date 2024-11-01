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
        WanderTarget();
        FlipSprite();
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

    private void FlipSprite()
    {
        if(transform.position.x > target.transform.position.x) 
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    private void Dash()
    {
        float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (canDash && animationTime >= 0.5f && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("WolfAttack"))
        {
            rgbd.AddForce(direction.normalized * dashForce, ForceMode.Impulse);
            canDash = false;
        }
    }
}
