using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryNinja : Enemy
{
    [SerializeField] private GameObject sparks;
    [Header("ParryForce")]
    [SerializeField] private float parryForce;

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
            case enemyState.BLOCK:
                break;
        }
        base.UpdateEnemy();
        if (Input.GetKeyDown(KeyCode.N)) { ReceiveDamageEnemy(1, true); }
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

    public void EndBlock()
    {
        currentState = enemyState.RUNNING;
        animator.SetBool("Parry", false);
    }

    public override void ReceiveDamageEnemy(float amount, bool isFlipped)
    {
        base.PrepareReceiveDamage();
        ParryPlayer();

    }

    private void ParryPlayer()
    {
        currentState = enemyState.BLOCK;
        animator.SetBool("Parry", true);
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
        target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target.GetComponent<Rigidbody>().AddForce((target.transform.position - transform.position).normalized * parryForce, ForceMode.Impulse);

        GameObject _sparks = Instantiate(sparks);
        _sparks.transform.SetParent(transform, true);
        if(!GetComponent<SpriteRenderer>().flipX)
            _sparks.transform.localPosition = new Vector3(0.1f,0.06f,0);
        else
            _sparks.transform.localPosition = new Vector3(-0.1f, 0.06f, 0);
        _sparks.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

}

