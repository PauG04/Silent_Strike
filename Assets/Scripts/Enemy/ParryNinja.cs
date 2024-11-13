using UnityEngine;

public class ParryNinja : Enemy
{
    [SerializeField] private GameObject sparks;
    [Header("ParryForce")]
    [SerializeField] private float parryForce;

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
            case enemyState.BLOCK:
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

    public void EndBlock()
    {
        ChangeState(enemyState.RUNNING);
    }

    public override void ReceiveDamageEnemy(float amount, bool isFlipped)
    {
        base.PrepareReceiveDamage();
        if (isFlipped != spriteRenderer.flipX)
            ParryPlayer();
        else
            base.ReceiveDamageEnemy(amount, isFlipped);

    }

    private void ParryPlayer()
    {
        ChangeState(enemyState.BLOCK);
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
        target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target.GetComponent<Rigidbody>().AddForce((target.transform.position - transform.position).normalized * parryForce, ForceMode.Impulse);
        CreateSpark();
    }

    private void CreateSpark()
    {
        GameObject _sparks = Instantiate(sparks);
        _sparks.transform.SetParent(transform, true);
        if (!GetComponent<SpriteRenderer>().flipX)
        {
            _sparks.transform.localPosition = new Vector3(0.1f, 0.06f, 0);
            _sparks.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            _sparks.transform.localPosition = new Vector3(-0.1f, 0.06f, 0);
            _sparks.GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    public override void PlayerDeath()
    {
        base.PlayerDeath();
    }

}

