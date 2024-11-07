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
        if (!canDash)
            canDash = true;
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

    protected override void ChangeToAttackColor()
    {
        base.ChangeToAttackColor();
    }
    protected override void Attack()
    {
        base.Attack();
    }

    protected override void AttackReady()
    {
        base.AttackReady();
    }

    public override void ReceiveDamageEnemy(float amount, bool isFlipped)
    {
        base.PrepareReceiveDamage();
        base.ReceiveDamageEnemy(amount, isFlipped);
    }
    private void Dash()
    {
        rgbd.AddForce(direction.normalized * dashForce, ForceMode.Impulse);
        canDash = false;
    }
}
