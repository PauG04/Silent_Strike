using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    public enum enemyState { IDLE, RUNNING, CHARGING, ATTACK, RECOVERY, HURT, DIE };
    public enemyState currentState;

    [Header("MovementEnemy")]
    [SerializeField] private Vector3 separationDistance;
    [SerializeField] protected GameObject target;
    protected Vector3 direction;

    [Header("Forces")]
    [SerializeField] private float cohesionWeight;
    [SerializeField] private float separationWeight;
    [SerializeField] private float alligmentWeight;
    private Vector3 separationForce;

    [Header("Recovery")]
    [SerializeField] private float recoveryTime;
    private float currentRecoveryTime;

    [Header("Collider")]
    [SerializeField] private GameObject colliderPosition;


    protected void InitializeEnemy()
    {
        base.InitializeCharacter();
        currentRecoveryTime = 0;
        colliderPosition.transform.localPosition = new Vector3(-GetComponent<SpriteRenderer>().bounds.size.x / 2, 0 ,0);
    }

    protected void UpdateEnemy()
    {
        base.UpdateCharacter();
    }

    //Movement
    #region 
    protected void SeekTarget()
    {
        if (target != null)
        {
            Seek();
            CalculateForces();
            Rotate();
            MoveEnemy();
        }

    }

    private void Rotate()
    {
        if (rgbd.velocity.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            colliderPosition.transform.localPosition = new Vector3(GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
            colliderPosition.transform.localPosition = new Vector3(-GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }
    }

    private void MoveEnemy()
    {
        Vector3 combinedDirection = (direction.normalized + separationForce).normalized;
        Vector3 movement = combinedDirection * speed * Time.deltaTime;
        rgbd.AddForce(movement, ForceMode.Force);
    }

    private void Seek()
    {
        if (transform.position.x > target.transform.position.x + GetComponent<SpriteRenderer>().bounds.size.x / 2 ||
            transform.position.x < target.transform.position.x - GetComponent<SpriteRenderer>().bounds.size.x / 2)
        {
            Vector3 targetPosition = Vector3.zero;

            if (!GetComponent<SpriteRenderer>().flipX)
                targetPosition = new Vector3(target.transform.position.x + target.GetComponent<SpriteRenderer>().bounds.size.x / 2, transform.position.y, target.transform.position.z);
            else
                targetPosition = new Vector3(target.transform.position.x - target.GetComponent<SpriteRenderer>().bounds.size.x / 2, transform.position.y, target.transform.position.z);

            direction = targetPosition - transform.position;
        }
        else
        {
            direction = target.transform.position -  transform.position;
        }

    }
    #endregion

    //Forces
    #region 
    private void CalculateForces()
    {
        separationForce = Vector3.zero;
        Collider[] neighbours = GetNeighbours();

        if (neighbours.Length - 1 > 0)
        {
            CalculateSeparationFoce(neighbours);
            ApplyAlligment(neighbours);
            ApplyCohesion(neighbours);
        }
    }

    private void CalculateSeparationFoce(Collider[] neighbours)
    {
        foreach (var neighbour in neighbours)
        {
            Vector3 direction = neighbour.transform.position - transform.position;
            float distance = direction.magnitude;
            Vector3 away = -direction.normalized;

            if (distance > 0)
            {
                separationForce += away / distance * separationWeight;
            }
        }
    }

    private void ApplyAlligment(Collider[] neighbours)
    {
        Vector3 neighboursForward = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            neighboursForward += neighbour.transform.forward;
        }

        if (neighboursForward != Vector3.zero)
        {
            neighboursForward.Normalize();
        }

        separationForce += neighboursForward * alligmentWeight;
    }
    private void ApplyCohesion(Collider[] neighbours)
    {
        Vector3 avaragePosition = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            avaragePosition += neighbour.transform.position;
        }

        avaragePosition /= neighbours.Length;
        Vector3 cohesionDirection = (avaragePosition - transform.position).normalized;
        separationForce += cohesionDirection * cohesionWeight;
    }


    private Collider[] GetNeighbours()
    {
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        return Physics.OverlapBox(transform.position, separationDistance, Quaternion.identity, enemyMask);
    }
    #endregion

    //Attack
    #region
    protected void ChargingAttack()
    {
        animator.SetBool("Charging", true);
        rgbd.velocity = Vector3.zero;
    }

    protected void AttackReady(string animationName)
    {
        float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animationTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(animationName))
        {
            currentState = enemyState.ATTACK;
            animator.SetBool("Attack", true);
            animator.SetBool("Charging", false);
        }
    }

    protected void Attack(string animationName)
    {
        float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animationTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(animationName))
        {
            animator.SetBool("Attack", false);
            currentState = enemyState.RECOVERY;
        }
    }

    protected void Recovery()
    {
        currentRecoveryTime += Time.deltaTime;
        if(currentRecoveryTime >= recoveryTime)
        {
            currentState = enemyState.RUNNING;
            currentRecoveryTime = 0;
        }
    }
    #endregion

    //Damage
    #region
    protected void Hurt(string animationName)
    {
        float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animationTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(animationName))
        {
            currentState = enemyState.RUNNING;
            animator.SetBool("Hurt", false);
        }
    }

    protected void Die(string animationName)
    {
        Destroy(transform.GetChild(0).gameObject);
        Destroy(GetComponent<BoxCollider2D>());

        float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animationTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(animationName))
        {
            Destroy(gameObject);
        }
    }
    #endregion

    protected virtual void ReceiveDamage(float amount)
    {
        base.ReceiveDamage(amount);
        rgbd.velocity = Vector3.zero;
        if (currentHP <= 0)
        {
            currentState = enemyState.DIE;
            animator.SetBool("Hurt", true);
        }
        else
        {
            currentState = enemyState.HURT;
            animator.SetBool("Die", true);
        }     
    }

    public void ChangeState(enemyState state)
    {
        currentState = state;
    }

    public enemyState GetCurrentState()
    {
        return currentState;
    }

    public void SetTarget(GameObject _target)
    {
        target = _target;
    }
}
