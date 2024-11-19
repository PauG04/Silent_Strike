using UnityEngine;

public class Boss : Enemy
{
    public enum attackState { NOATTACK, ATTACK1, ATTACK2, ATTACK3, JUMPATTACK, CHARGING1, CHARGING2, SHOUT };
    public attackState currentAttackState;

    [Header("Animations")]
    [SerializeField] private string[] chargeAnimation;
    [SerializeField] private RuntimeAnimatorController secondPhaseAnimator;

    [Header("BossStats")]
    [SerializeField] private float damage1WhioutFlame;
    [SerializeField] private float damage2WhioutFlame;
    [SerializeField] private float damage3WhioutFlame;
    [SerializeField] private float damageJumpWhioutFlame;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private float secondPhaseVelocity;

    private int normalAttacksDone;
    private bool secondPhaseActive;
    private int randomValue;
    private float initDashAttackForce;

    [SerializeField] private AudioClip screamSound;
    [SerializeField] private AudioClip attack02Sound;

    private void Start()
    {
        base.InitializeEnemy();
        currentAttackState = attackState.NOATTACK;
        randomValue = 0;
        normalAttacksDone = 0;
        secondPhaseActive = false;
        initDashAttackForce = dashAttackForce;
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
        rgbd.velocity = Vector3.zero;
        secondPhaseActive = true;
    }

    private void ScreamSound()
    {
        AudioManager.instance.Play2dOneShotSound(screamSound, "Sfx");
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
        for(int i = 0; i< chargeAnimation.Length; i++)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(chargeAnimation[i]))
                GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
        }
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
        AudioManager.instance.Play2dOneShotSound(attack02Sound, "Sfx", 0.9f);
    }

    public void ActiveThirdAttack()
    {
        ChangeBossState(attackState.ATTACK3);
        AudioManager.instance.Play2dOneShotSound(attackSound, "Sfx", 0.8f);
    }

    public override void ReceiveDamageEnemy(float amount, bool isFlipped)
    {
        base.PrepareReceiveDamage();
        base.ReceiveDamageEnemy(amount, isFlipped);
        ChangeBossState(attackState.NOATTACK);
    }

    public void InitSecondPhse()
    {
        colliderPosition.SetActive(true);
        damageColliderPosition.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
        ChangeBossState(attackState.NOATTACK);
        ChangeState(enemyState.RUNNING);
        speed = secondPhaseVelocity;
        current_speed = speed;
    }

    public void ChangeBossState(attackState state)
    {
        switch (currentAttackState)
        {
            case attackState.ATTACK1:
                animator.SetBool("AttackNormal", false);
                dashAttackForce = 0;
                break;
            case attackState.ATTACK2:
                break;
            case attackState.ATTACK3:
                animator.SetBool("AttackNormal3", false);
                dashAttackForce = initDashAttackForce;
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
                if(!secondPhaseActive)
                    damage = damage1WhioutFlame;
                else
                    damage = damage1WhioutFlame * damageMultiplier;
                break;
            case attackState.ATTACK2:
                animator.SetBool("AttackNormal2", true);
                if (!secondPhaseActive)
                    damage = damage2WhioutFlame;
                else
                    damage = damage2WhioutFlame * damageMultiplier;
                break;
            case attackState.ATTACK3:
                animator.SetBool("AttackNormal3", true);
                if (!secondPhaseActive)
                    damage = damage3WhioutFlame;
                else
                    damage = damage3WhioutFlame * damageMultiplier;
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
                if (!secondPhaseActive)
                    damage = damageJumpWhioutFlame;
                else
                    damage = damageJumpWhioutFlame * damageMultiplier;
                break;
            case attackState.SHOUT:
                animator.SetBool("Shout", true);
                break;
        }
        currentAttackState = state;
    }

    public override void PlayerDeath()
    {
        animator.SetBool("ChargingJump", false);
        animator.SetBool("ChargingNormal", false);
        animator.SetBool("AttackNormal", false);
        animator.SetBool("AttackJump", false);
        animator.SetBool("AttackNormal2", false);
        animator.SetBool("AttackNormal3", false);
        base.PlayerDeath();
    }
}
