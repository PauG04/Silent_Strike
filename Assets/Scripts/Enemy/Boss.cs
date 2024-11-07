using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public enum attackState { NOATTACK, ATTACK1, ATTACK2, ATTACK3, JUMPATTACK, CHARGING1, CHARGING2, SHOUT };
    public attackState currentAttackState;

    [Header("Animations")]
    [SerializeField] private string[] chargeAnimation;
    [SerializeField] private RuntimeAnimatorController secondPhaseAnimator;

    private int normalAttacksDone;
    private bool secondPhaseActive;
    private int randomValue;

    private MaterialPropertyBlock mpb;


    private void Start()
    {
        base.InitializeEnemy();
        currentAttackState = attackState.NOATTACK;
        randomValue = 0;
        normalAttacksDone = 0;
        secondPhaseActive = false;

        mpb = new MaterialPropertyBlock();
        mpb.SetFloat("_OutlineThickness", 0.001f);
        GetComponent<SpriteRenderer>().SetPropertyBlock(mpb);
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
        }
        base.UpdateEnemy();

        if(!secondPhaseActive && currentHP <= maxHP / 2)
        {
            PrepareInitSecondPhase();
        }
    }

    private void PrepareInitSecondPhase()
    {
        ChangeBossState(attackState.SHOUT);
        colliderPosition.SetActive(false);
        damageColliderPosition.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        ChangeState(enemyState.IDLE);
        secondPhaseActive = true;
        mpb.SetFloat("_OutlineThickness", 0.0003f);
        GetComponent<SpriteRenderer>().SetPropertyBlock(mpb);
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
        if(randomValue == 0)
        {
            randomValue = UnityEngine.Random.Range(1, 5);
            if (randomValue == 1 || normalAttacksDone == 3)
            {
                ChangeBossState(attackState.CHARGING1);
            }
            else if (randomValue != 1)
            {
                ChangeBossState(attackState.CHARGING2);
            }

        }
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(chargeAnimation[0]) || animator.GetCurrentAnimatorStateInfo(0).IsName(chargeAnimation[1]))
            GetComponent<SpriteRenderer>().material.SetColor("_SpriteColor", Color.Lerp(GetComponent<SpriteRenderer>().color, Color.red, animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1));
    }

    protected override void Attack()
    {
        base.Attack();
        ChangeBossState(attackState.NOATTACK);
    }

    protected override void AttackReady()
    {
        base.AttackReady();
        if (randomValue == 1)
            ChangeBossState(attackState.JUMPATTACK);
        else
            ChangeBossState(attackState.ATTACK1);
    }

    public void ActiveSecondAttack()
    {
        ChangeBossState(attackState.ATTACK2);
    }

    public void ActiveThirdAttack()
    {
        ChangeBossState(attackState.ATTACK3);
    }

    public override void ReceiveDamageEnemy(float amount)
    {
        base.PrepareReceiveDamage();
        base.ReceiveDamageEnemy(amount);
        ChangeBossState(attackState.NOATTACK);
    }

    public void InitSecondPhse()
    {
        colliderPosition.SetActive(true);
        damageColliderPosition.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
        mpb.SetFloat("_OutlineThickness", 0.001f);
        GetComponent<SpriteRenderer>().SetPropertyBlock(mpb);
        ChangeBossState(attackState.NOATTACK);
        ChangeState(enemyState.RUNNING);
    }

    public void ChangeBossState(attackState state)
    {
        switch (currentAttackState)
        {
            case attackState.ATTACK1:
                animator.SetBool("AttackNormal", false);
                break;
            case attackState.ATTACK2:
                break;
            case attackState.ATTACK3:
                animator.SetBool("AttackNormal3", false);
                break;
            case attackState.CHARGING1:
                animator.SetBool("ChargingJump", false);
                randomValue = 0;
                break;
            case attackState.CHARGING2:
                animator.SetBool("ChargingNormal", false);
                randomValue = 0;
                break;
            case attackState.JUMPATTACK:
                animator.SetBool("AttackJump", false);
                break;
            case attackState.SHOUT:
                animator.SetBool("Shout", false);
                GetComponent<Animator>().runtimeAnimatorController = secondPhaseAnimator;
                break;
        }

        switch (state)
        {
            case attackState.NOATTACK:
                animator.SetBool("ChargingJump", false);
                animator.SetBool("ChargingNormal", false);
                animator.SetBool("AttackNormal", false);
                animator.SetBool("AttackJump", false);
                animator.SetBool("AttackNormal2", false);
                animator.SetBool("AttackNormal3", false);
                break;
            case attackState.ATTACK1:
                animator.SetBool("AttackNormal", true);
                break;
            case attackState.ATTACK2:
                animator.SetBool("AttackNormal2", true);
                break;
            case attackState.ATTACK3:
                animator.SetBool("AttackNormal3", true);
                break;
            case attackState.CHARGING1:
                animator.SetBool("ChargingJump", true);
                normalAttacksDone = 0;
                break;
            case attackState.CHARGING2:
                animator.SetBool("ChargingNormal", true);
                normalAttacksDone++;
                break;
            case attackState.JUMPATTACK:
                animator.SetBool("AttackJump", true);
                break;
            case attackState.SHOUT:
                animator.SetBool("Shout", true);
                break;
        }
        currentAttackState = state;
    }
}
