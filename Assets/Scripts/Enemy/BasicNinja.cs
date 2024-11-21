using UnityEngine;
using static Enemy;

public class BasicNinja : Enemy
{
    private void Start()
    {
        base.InitializeEnemy();
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
                break;
            case enemyState.RECOVERY:
                RecoveryState();
                break;
            case enemyState.HURT:
                break;
            case enemyState.DIE:
                DieState();
                break;
            case enemyState.FREEZED:
                Freeze();
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

    public override void ReceiveDamageEnemy(float amount, bool isFlipped, bool isKunai = false)
    {
        base.PrepareReceiveDamage();
        base.ReceiveDamageEnemy(amount, isFlipped);
    }

    public override void PlayerDeath()
    {
        base.PlayerDeath();
    }

}
